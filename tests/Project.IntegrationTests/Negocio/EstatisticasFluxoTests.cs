using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Project.IntegrationTests.Suporte;

namespace Project.IntegrationTests.Negocio;

[Collection(ColecaoApi.Nome)]
public class EstatisticasFluxoTests
{
    private const string Senha = "abcdefgh";
    private readonly BancoFixture _banco;

    public EstatisticasFluxoTests(BancoFixture banco) => _banco = banco;

    [Fact]
    public async Task Estatisticas_contam_situacoes_e_isolam_conta()
    {
        var dono = await Entrar();
        var outra = await Entrar();
        var modelo = await Ler(await dono.PostAsJsonAsync("/api/v1/modelos", new { nome = "Stats", tipo = "Aniversario" }));
        var modeloId = modelo.GetProperty("id").GetGuid();

        var andamento = await Ler(await dono.PostAsJsonAsync("/api/v1/eventos", new
        {
            modeloId,
            primeiroNome = "Andamento",
            data = $"{DateTime.UtcNow.Year}-03-10",
        }));
        var concluido = await Ler(await dono.PostAsJsonAsync("/api/v1/eventos", new
        {
            modeloId,
            primeiroNome = "Concluido",
            data = $"{DateTime.UtcNow.Year}-04-10",
        }));
        var cancelado = await Ler(await dono.PostAsJsonAsync("/api/v1/eventos", new
        {
            modeloId,
            primeiroNome = "Cancelado",
            data = $"{DateTime.UtcNow.Year}-05-10",
        }));
        await dono.PostAsync($"/api/v1/eventos/{concluido.GetProperty("id").GetGuid()}/concluir", JsonBody());
        await dono.PostAsync($"/api/v1/eventos/{cancelado.GetProperty("id").GetGuid()}/cancelar", JsonBody());

        var stats = await Ler(await dono.GetAsync("/api/v1/eventos/estatisticas"));
        Assert.True(stats.GetProperty("total").GetInt32() >= 3);
        Assert.Contains(
            stats.GetProperty("situacoes").EnumerateArray(),
            item => item.GetProperty("situacao").GetString() == "EmAndamento" && item.GetProperty("quantidade").GetInt32() >= 1);
        Assert.Contains(
            stats.GetProperty("situacoes").EnumerateArray(),
            item => item.GetProperty("situacao").GetString() == "Concluido" && item.GetProperty("quantidade").GetInt32() >= 1);
        Assert.Contains(
            stats.GetProperty("situacoes").EnumerateArray(),
            item => item.GetProperty("situacao").GetString() == "Cancelado" && item.GetProperty("quantidade").GetInt32() >= 1);
        Assert.Equal(12, stats.GetProperty("eventosPorMes").GetArrayLength());
        Assert.Equal("Para essa semana", stats.GetProperty("resumo").GetProperty("contadorTarefas").GetString());
        Assert.Equal(0, stats.GetProperty("resumo").GetProperty("aniversariosHoje").GetInt32());

        var statsOutra = await Ler(await outra.GetAsync("/api/v1/eventos/estatisticas"));
        Assert.Equal(0, statsOutra.GetProperty("total").GetInt32());
        Assert.All(statsOutra.GetProperty("situacoes").EnumerateArray(), item =>
        {
            Assert.Equal(0, item.GetProperty("quantidade").GetInt32());
            Assert.Equal(0, item.GetProperty("percentual").GetDecimal());
        });
        Assert.Empty(statsOutra.GetProperty("rankingOrcamento").EnumerateArray());
        _ = andamento;
    }

    private static StringContent JsonBody() =>
        new("{}", System.Text.Encoding.UTF8, "application/json");

    private async Task<HttpClient> Entrar()
    {
        var email = $"stats-{Guid.NewGuid():N}@gevents.test";
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
