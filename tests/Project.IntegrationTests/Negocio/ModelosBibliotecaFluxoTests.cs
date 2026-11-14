using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Project.IntegrationTests.Suporte;

namespace Project.IntegrationTests.Negocio;

[Collection(ColecaoApi.Nome)]
public class ModelosBibliotecaFluxoTests
{
    private const string Senha = "abcdefgh";
    private readonly BancoFixture _banco;

    public ModelosBibliotecaFluxoTests(BancoFixture banco) => _banco = banco;

    [Fact]
    public async Task Bibliotecas_criam_clonam_excluem_e_isolam()
    {
        var dono = await Entrar();
        var outra = await Entrar();

        var briefing = await Ler(await dono.PostAsJsonAsync("/api/v1/modelos-briefing", new
        {
            nome = "Briefing casamento",
            secoes = new[]
            {
                new
                {
                    titulo = "Geral",
                    descricao = "Sobre o casal",
                    perguntas = new[] { new { enunciado = "Quantos convidados?", tipo = "TextoCurto" } },
                },
            },
        }));
        Assert.False(briefing.GetProperty("padrao").GetBoolean());
        var briefingId = briefing.GetProperty("id").GetGuid();
        var briefingClone = await Ler(await dono.PostAsJsonAsync($"/api/v1/modelos-briefing/{briefingId}/clonar", new { }));
        Assert.Equal(HttpStatusCode.OK, (await dono.DeleteAsync($"/api/v1/modelos-briefing/{briefingClone.GetProperty("id").GetGuid()}")).StatusCode);
        Assert.Empty((await Ler(await outra.GetAsync("/api/v1/modelos-briefing"))).EnumerateArray());

        var roteiro = await Ler(await dono.PostAsJsonAsync("/api/v1/modelos-roteiro", new
        {
            nome = "Cerimônia",
            secoes = new[] { new { titulo = "Entrada", horario = "16:00:00", musica = "Marcha", itens = new[] { new { texto = "Padrinhos" } } } },
        }));
        var roteiroClone = await Ler(await dono.PostAsJsonAsync($"/api/v1/modelos-roteiro/{roteiro.GetProperty("id").GetGuid()}/clonar", new { }));
        Assert.Equal(HttpStatusCode.OK, (await dono.DeleteAsync($"/api/v1/modelos-roteiro/{roteiroClone.GetProperty("id").GetGuid()}")).StatusCode);

        var cronograma = await Ler(await dono.PostAsJsonAsync("/api/v1/modelos-cronograma", new
        {
            titulo = "Semana do evento",
            descricao = "Checklist",
            itens = new[] { new { rotulo = "Ensaio", responsavel = "Equipe" } },
        }));
        var cronogramaClone = await Ler(await dono.PostAsJsonAsync($"/api/v1/modelos-cronograma/{cronograma.GetProperty("id").GetGuid()}/clonar", new { }));
        Assert.Equal(HttpStatusCode.OK, (await dono.DeleteAsync($"/api/v1/modelos-cronograma/{cronogramaClone.GetProperty("id").GetGuid()}")).StatusCode);

        var inspiracao = await Ler(await dono.PostAsJsonAsync("/api/v1/modelos-inspiracoes", new { nome = "Decoração" }));
        var inspiracaoClone = await Ler(await dono.PostAsJsonAsync($"/api/v1/modelos-inspiracoes/{inspiracao.GetProperty("id").GetGuid()}/clonar", new { }));
        Assert.Equal(HttpStatusCode.OK, (await dono.DeleteAsync($"/api/v1/modelos-inspiracoes/{inspiracaoClone.GetProperty("id").GetGuid()}")).StatusCode);

        var mensagem = await Ler(await dono.PostAsJsonAsync("/api/v1/modelos-mensagens", new
        {
            nome = "RSVP",
            texto = "Olá, confirme presença em #nome_evento",
        }));
        Assert.Contains(mensagem.GetProperty("parametros").EnumerateArray(), item => item.GetString() == "#nome_evento");
        var mensagemClone = await Ler(await dono.PostAsJsonAsync($"/api/v1/modelos-mensagens/{mensagem.GetProperty("id").GetGuid()}/clonar", new { }));
        Assert.Equal(HttpStatusCode.OK, (await dono.DeleteAsync($"/api/v1/modelos-mensagens/{mensagemClone.GetProperty("id").GetGuid()}")).StatusCode);
        Assert.Empty((await Ler(await outra.GetAsync("/api/v1/modelos-mensagens"))).EnumerateArray());

        var modelo = await Ler(await dono.PostAsJsonAsync("/api/v1/modelos", new { nome = "Casamento completo", tipo = "Casamento" }));
        var modeloId = modelo.GetProperty("id").GetGuid();
        await Ler(await dono.PutAsJsonAsync($"/api/v1/modelos/{modeloId}", new
        {
            nome = "Casamento completo",
            tipo = "Casamento",
            permissoes = new { gerais = true, planejamento = true, tarefas = true, servicosFornecedores = true, convites = true },
            servicos = new[]
            {
                new
                {
                    nome = "Buffet",
                    valorEstimado = 5000,
                    itens = new[] { new { tipo = "Tarefa", texto = "Prova do menu" } },
                },
            },
            tarefas = new[] { new { titulo = "Contratar banda", diasAntes = 30 } },
            vinculos = Array.Empty<object>(),
        }));
        var clone = await Ler(await dono.PostAsJsonAsync($"/api/v1/modelos/{modeloId}/clonar", new { }));
        Assert.Contains("cópia", clone.GetProperty("nome").GetString(), StringComparison.OrdinalIgnoreCase);
        Assert.Equal(HttpStatusCode.NotFound, (await outra.GetAsync($"/api/v1/modelos/{modeloId}")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await dono.DeleteAsync($"/api/v1/modelos/{clone.GetProperty("id").GetGuid()}")).StatusCode);
    }

    private async Task<HttpClient> Entrar()
    {
        var email = $"bib-{Guid.NewGuid():N}@gevents.test";
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
