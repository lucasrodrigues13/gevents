using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Project.IntegrationTests.Suporte;

namespace Project.IntegrationTests.Negocio;

[Collection(ColecaoApi.Nome)]
public class FornecedoresFluxoTests
{
    private const string Senha = "abcdefgh";
    private readonly BancoFixture _banco;

    public FornecedoresFluxoTests(BancoFixture banco) => _banco = banco;

    [Fact]
    public async Task Fornecedor_negociacao_budget_e_isolamento()
    {
        var dono = await Entrar();
        var outra = await Entrar();

        var servicoConta = await Ler(await dono.PostAsJsonAsync("/api/v1/servicos-conta", new { nome = "Banda" }));
        var servicoContaId = servicoConta.GetProperty("id").GetGuid();
        Assert.Equal(HttpStatusCode.BadRequest, (await dono.PostAsJsonAsync("/api/v1/fornecedores", new { nome = "Sem serviço" })).StatusCode);

        var fornecedor = await Ler(await dono.PostAsJsonAsync("/api/v1/fornecedores", new
        {
            nome = "Banda Teste",
            servicoContaIds = new[] { servicoContaId },
            emails = new[] { new { endereco = "banda@exemplo.com" } },
            telefones = new[] { new { numero = "11999990000", movel = true } },
        }));
        var fornecedorId = fornecedor.GetProperty("id").GetGuid();
        Assert.True(fornecedor.GetProperty("completo").GetBoolean());
        Assert.Empty((await Ler(await outra.GetAsync("/api/v1/fornecedores"))).EnumerateArray());
        Assert.Equal(HttpStatusCode.NotFound, (await outra.GetAsync($"/api/v1/fornecedores/{fornecedorId}")).StatusCode);

        var modelo = await Ler(await dono.PostAsJsonAsync("/api/v1/modelos", new { nome = "Festa", tipo = "Aniversario" }));
        var evento = await Ler(await dono.PostAsJsonAsync("/api/v1/eventos", new
        {
            modeloId = modelo.GetProperty("id").GetGuid(),
            primeiroNome = "Helena",
            orcamentoEstimado = 20000,
            convidadosEstimados = 100,
        }));
        var eventoId = evento.GetProperty("id").GetGuid();

        var comServico = await Ler(await dono.PostAsJsonAsync($"/api/v1/eventos/{eventoId}/servicos", new { nome = "Banda" }));
        var servicoId = comServico.GetProperty("servicos").EnumerateArray()
            .Single(item => item.GetProperty("nome").GetString() == "Banda")
            .GetProperty("id").GetGuid();

        var comFornecedor = await Ler(await dono.PostAsJsonAsync(
            $"/api/v1/eventos/{eventoId}/servicos/{servicoId}/fornecedores",
            new { fornecedorId, etapa = "Recomendado" }));
        var vinculoId = comFornecedor.GetProperty("servicos").EnumerateArray()
            .Single(item => item.GetProperty("id").GetGuid() == servicoId)
            .GetProperty("fornecedores").EnumerateArray().Single().GetProperty("id").GetGuid();

        await Ler(await dono.PostAsJsonAsync($"/api/v1/eventos/{eventoId}/servicos/{servicoId}/fornecedores/{vinculoId}/avancar", new { }));
        await Ler(await dono.PostAsJsonAsync($"/api/v1/eventos/{eventoId}/servicos/{servicoId}/fornecedores/{vinculoId}/avancar", new { }));
        var contratado = await Ler(await dono.PostAsJsonAsync(
            $"/api/v1/eventos/{eventoId}/servicos/{servicoId}/fornecedores/{vinculoId}/contratar",
            new
            {
                valor = 5000,
                quantidadeParcelas = 2,
                primeiraParcela = "2027-01-10",
                comEntrada = false,
            }));
        var servicoContratado = contratado.GetProperty("servicos").EnumerateArray()
            .Single(item => item.GetProperty("id").GetGuid() == servicoId);
        Assert.True(servicoContratado.GetProperty("contratado").GetBoolean());
        Assert.Equal(2, servicoContratado.GetProperty("parcelas").GetArrayLength());
        Assert.Equal("Contratado", servicoContratado.GetProperty("fornecedores")[0].GetProperty("etapa").GetString());

        var comSubitem = await Ler(await dono.PutAsJsonAsync(
            $"/api/v1/eventos/{eventoId}/servicos/{servicoId}/estimado",
            new
            {
                subitens = new[]
                {
                    new { descricao = "Show", quantidade = 1, valorUnitario = 3000 },
                    new { descricao = "Som extra", quantidade = 2, valorUnitario = 250 },
                },
            }));
        Assert.Equal(3500, comSubitem.GetProperty("servicos").EnumerateArray()
            .Single(item => item.GetProperty("id").GetGuid() == servicoId)
            .GetProperty("valorEstimado").GetDecimal());

        var budget = await Ler(await dono.GetAsync($"/api/v1/eventos/{eventoId}/budget"));
        Assert.Equal(20000, budget.GetProperty("budget").GetDecimal());
        Assert.Equal(3500, budget.GetProperty("estimado").GetDecimal());
        Assert.Equal(5000, budget.GetProperty("contratado").GetDecimal());
        Assert.Equal(8500, budget.GetProperty("provaReal").GetDecimal());
        Assert.Equal(85, budget.GetProperty("custoPorConvidado").GetDecimal());
        Assert.Equal(HttpStatusCode.NotFound, (await outra.GetAsync($"/api/v1/eventos/{eventoId}/budget")).StatusCode);
    }

    private async Task<HttpClient> Entrar()
    {
        var email = $"forn-{Guid.NewGuid():N}@gevents.test";
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
