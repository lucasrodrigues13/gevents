using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Project.IntegrationTests.Suporte;

namespace Project.IntegrationTests.Negocio;

[Collection(ColecaoApi.Nome)]
public class FunilFluxoTests
{
    private const string Senha = "abcdefgh";
    private readonly BancoFixture _banco;

    public FunilFluxoTests(BancoFixture banco) => _banco = banco;

    [Fact]
    public async Task Agenda_oportunidades_e_painel_isolam_por_conta()
    {
        var dono = await Entrar();
        var outra = await Entrar();

        var compromisso = await Ler(await dono.PostAsJsonAsync("/api/v1/agenda", new
        {
            descricao = "Reunião comercial",
            data = "2030-05-10",
        }));
        Assert.Equal("Reunião comercial", compromisso.GetProperty("descricao").GetString());
        Assert.Equal(HttpStatusCode.BadRequest, (await dono.PostAsJsonAsync("/api/v1/agenda", new { descricao = " ", data = "2030-05-11" })).StatusCode);
        Assert.Empty((await Ler(await outra.GetAsync("/api/v1/agenda?de=2030-05-01&ate=2030-05-31"))).EnumerateArray());

        var servico = await Ler(await dono.PostAsJsonAsync("/api/v1/modelos-servicos-funil", new
        {
            nome = "Assessoria completa",
            descricao = "Pacote",
            valor = 12000,
        }));
        var servicoId = servico.GetProperty("id").GetGuid();

        var oportunidade = await Ler(await dono.PostAsJsonAsync("/api/v1/oportunidades", new
        {
            nome = "Casamento Ana",
            estagio = "Contato inicial",
            valorProposta = 15000,
            modeloServicoFunilId = servicoId,
            contatos = new[] { new { nome = "Ana", telefone = "11999999999", email = "ana@ex.com" } },
        }));
        var oportunidadeId = oportunidade.GetProperty("id").GetGuid();
        Assert.Equal(HttpStatusCode.BadRequest, (await dono.PostAsJsonAsync("/api/v1/oportunidades", new
        {
            nome = "Sem contato",
            estagio = "Contato inicial",
            contatos = Array.Empty<object>(),
        })).StatusCode);

        await Ler(await dono.PostAsJsonAsync($"/api/v1/oportunidades/{oportunidadeId}/mover", new
        {
            estagio = "Descartado",
            motivoDescarte = "Preço",
        }));
        Assert.Equal(HttpStatusCode.BadRequest, (await dono.PostAsJsonAsync($"/api/v1/oportunidades/{oportunidadeId}/mover", new
        {
            estagio = "Descartado",
        })).StatusCode);

        var painel = await Ler(await dono.GetAsync("/api/v1/painel"));
        var funil = painel.GetProperty("funil").EnumerateArray().ToArray();
        Assert.Equal(4, funil.Length);
        Assert.Contains(funil, item => item.GetProperty("estagio").GetString() == "Descartado" && item.GetProperty("quantidade").GetInt32() == 1);
        Assert.Empty((await Ler(await outra.GetAsync("/api/v1/oportunidades"))).EnumerateArray());

        var modeloOrcamento = await Ler(await dono.PostAsJsonAsync("/api/v1/modelos-orcamento-funil", new
        {
            nome = "Padrão",
            texto = "Proposta para #nome_oportunidade",
        }));
        var modeloContrato = await Ler(await dono.PostAsJsonAsync("/api/v1/modelos-contrato-funil", new
        {
            nome = "Cláusulas",
            texto = "Contrato de #nome_oportunidade",
        }));

        await Ler(await dono.PostAsJsonAsync($"/api/v1/oportunidades/{oportunidadeId}/mover", new { estagio = "Contato inicial" }));

        var orcamento = await Ler(await dono.PostAsJsonAsync("/api/v1/orcamentos-funil", new
        {
            oportunidadeId,
            modeloId = modeloOrcamento.GetProperty("id").GetGuid(),
        }));
        Assert.Contains("Casamento Ana", orcamento.GetProperty("conteudo").GetString());

        var contrato = await Ler(await dono.PostAsJsonAsync("/api/v1/contratos-funil", new
        {
            oportunidadeId,
            modeloId = modeloContrato.GetProperty("id").GetGuid(),
        }));
        Assert.Contains("Casamento Ana", contrato.GetProperty("conteudo").GetString());

        var tarefa = await Ler(await dono.PostAsJsonAsync("/api/v1/tarefas-funil", new
        {
            titulo = "Ligar para Ana",
            oportunidadeId,
            tipo = "Ligacao",
            responsavel = "Organizer",
            prazo = "2030-05-20",
        }));
        await Ler(await dono.PostAsJsonAsync($"/api/v1/tarefas-funil/{tarefa.GetProperty("id").GetGuid()}/concluir", new { }));

        var relatorio = await Ler(await dono.GetAsync("/api/v1/relatorios-funil?inicio=2030-01-01&fim=2030-12-31"));
        Assert.True(relatorio.GetProperty("fechadosPerdidos").GetArrayLength() >= 2);
        Assert.Equal(HttpStatusCode.NotFound, (await outra.GetAsync($"/api/v1/oportunidades/{oportunidadeId}")).StatusCode);

        var contaId = ContaId(dono);
        var publico = _banco.Fabrica.CreateClient();
        var lead = await Ler(await publico.PostAsJsonAsync($"/api/v1/public/leads/{contaId}", new
        {
            nome = "Lead site",
            contatoNome = "Bruno",
            telefone = "11988887777",
            email = "bruno@ex.com",
        }));
        Assert.Equal("Contato inicial", lead.GetProperty("estagio").GetString());
        Assert.Equal("Lead site", lead.GetProperty("nome").GetString());
    }

    private static string ContaId(HttpClient cliente)
    {
        var token = cliente.DefaultRequestHeaders.Authorization!.Parameter!;
        var payload = token.Split('.')[1];
        var padded = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
        var json = JsonDocument.Parse(Convert.FromBase64String(padded.Replace('-', '+').Replace('_', '/')));
        return json.RootElement.GetProperty("sub").GetString()!;
    }

    private async Task<HttpClient> Entrar()
    {
        var email = $"funil-{Guid.NewGuid():N}@gevents.test";
        var cliente = _banco.Fabrica.CreateClient();
        Assert.Equal(HttpStatusCode.Created, (await cliente.PostAsJsonAsync("/api/v1/conta/cadastro", new { email, senha = Senha, confirmacao = Senha })).StatusCode);
        var mensagem = _banco.Fabrica.Email.Mensagens.Last(item => item.Email == email && item.Tipo == "confirmacao");
        var query = QueryHelpers.ParseQuery(new Uri(mensagem.Corpo).Query);
        Assert.Equal(HttpStatusCode.OK, (await cliente.PostAsJsonAsync("/api/v1/conta/confirmar-email", new { usuarioId = query["usuarioId"].ToString(), token = query["token"].ToString() })).StatusCode);
        var entrada = await cliente.PostAsJsonAsync("/api/v1/conta/entrar", new { email, senha = Senha });
        var sessao = await entrada.Content.ReadFromJsonAsync<Sessao>();
        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessao!.Token);
        return cliente;
    }

    private static async Task<JsonElement> Ler(HttpResponseMessage resposta)
    {
        var texto = await resposta.Content.ReadAsStringAsync();
        Assert.True(resposta.IsSuccessStatusCode, texto);
        using var documento = JsonDocument.Parse(texto);
        return documento.RootElement.Clone();
    }

    private sealed record Sessao(string Token, string Email, string[] Papeis);
}
