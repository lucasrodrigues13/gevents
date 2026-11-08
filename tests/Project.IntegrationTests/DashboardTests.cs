using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;

namespace Project.IntegrationTests;

[Collection(PostgresCollection.Name)]
public sealed class DashboardTests : IAsyncLifetime
{
    private const string Password = "correta-senha";
    private readonly PostgresFixture _postgres;
    private GeventsFactory _factory = null!;

    public DashboardTests(PostgresFixture postgres)
    {
        _postgres = postgres;
    }

    public Task InitializeAsync()
    {
        _factory = new GeventsFactory(_postgres.ConnectionString);
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task Login_abre_os_dashboards_da_propria_conta()
    {
        var dono = await ConfirmedClientAsync(NewEmail());
        var vazio = await HtmlAsync(await dono.GetAsync("/Dashboards"));
        Assert.Contains("Dashboards", vazio);
        Assert.Contains("Perfil", vazio);
        Assert.Contains("Eventos", vazio);
        Assert.Contains("Tarefas", vazio);
        Assert.Contains("Clientes", vazio);
        Assert.Contains("Convidados", vazio);
        Assert.Contains("Em andamento", vazio);
        Assert.Contains(">0<", vazio);
        Assert.Contains("Não há evento para mostrar.", vazio);
        Assert.Contains("Contato inicial", vazio);
        Assert.Contains("Orçamento enviado", vazio);
        Assert.Contains("Contratado", vazio);
        Assert.Contains("Descartado", vazio);
        Assert.Equal(4, Regex.Matches(vazio, "Quantidade: 0").Count);
        Assert.Equal(4, Regex.Matches(vazio, "Valor: 0").Count);
        Assert.DoesNotContain("descoberta", vazio, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("orçamento pendente", vazio, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("orçamento aprovado", vazio, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("contrato pendente", vazio, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("contrato assinado", vazio, StringComparison.OrdinalIgnoreCase);

        var comData = Nome("Com data");
        var semData = Nome("Sem data");
        var concluido = Nome("Ja foi");
        var cancelado = Nome("Nao vai");
        var comDataId = await CriarEventoAsync(dono, comData, "2026-08-01");
        await CriarEventoAsync(dono, semData, null);
        var concluidoId = await CriarEventoAsync(dono, concluido, null);
        var canceladoId = await CriarEventoAsync(dono, cancelado, null);
        await PostAsync(dono, $"/Eventos/Home/{concluidoId}?handler=Concluir", await ConfirmacaoAsync(dono, concluidoId, "concluir"));
        await PostAsync(dono, $"/Eventos/Home/{canceladoId}?handler=Cancelar", await ConfirmacaoAsync(dono, canceladoId, "cancelar"));

        var painel = await HtmlAsync(await dono.GetAsync("/Dashboards"));
        Assert.Contains(">2<", painel);
        Assert.Contains(">1<", painel);
        Assert.Contains(comData, painel);
        Assert.Contains("01/08/2026", painel);
        Assert.Contains(semData, painel);
        Assert.DoesNotContain(concluido, painel);
        Assert.DoesNotContain(cancelado, painel);
        Assert.Contains($"/Eventos/Home/{comDataId}", painel);

        var aberto = await HtmlAsync(await dono.GetAsync($"/Eventos/Home/{comDataId}"));
        Assert.Contains(comData, aberto);

        var outro = await ConfirmedClientAsync(NewEmail());
        var alheio = await HtmlAsync(await outro.GetAsync("/Dashboards"));
        Assert.DoesNotContain(comData, alheio);
        Assert.Contains(">0<", alheio);
        Assert.Contains("Quantidade: 0", alheio);
    }

    private static async Task<Dictionary<string, string>> ConfirmacaoAsync(HttpClient client, Guid eventoId, string acao)
    {
        var html = await HtmlAsync(await client.GetAsync($"/Eventos/Home/{eventoId}?confirmar={acao}"));
        return new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = Token(html),
            ["confirmado"] = "sim"
        };
    }

    private static async Task<Guid> CriarEventoAsync(HttpClient client, string nome, string? data)
    {
        var campos = new Dictionary<string, string> { ["Input.Nome"] = nome };
        if (data is not null)
        {
            campos["Input.Data"] = data;
        }

        var resposta = await PostAsync(client, "/Eventos/Criar", campos);
        return Guid.Parse(resposta.RequestMessage!.RequestUri!.Segments.Last().Trim('/'));
    }

    private async Task<HttpClient> ConfirmedClientAsync(string email)
    {
        var client = _factory.CreateClient();
        await PostAsync(client, "/Account/Register", new Dictionary<string, string>
        {
            ["Input.Email"] = email,
            ["Input.Password"] = Password
        });
        var link = _factory.Services.GetRequiredService<FakeEmailSender>().Sent.Last(sent => sent.Email == email).Payload;
        await client.GetAsync(new Uri(link).PathAndQuery);
        await PostAsync(client, "/Account/Login", new Dictionary<string, string>
        {
            ["Input.Email"] = email,
            ["Input.Password"] = Password
        });
        return client;
    }

    private static async Task<HttpResponseMessage> PostAsync(HttpClient client, string path, Dictionary<string, string> fields)
    {
        if (!fields.ContainsKey("__RequestVerificationToken"))
        {
            var html = await HtmlAsync(await client.GetAsync(path.Split('?')[0]));
            fields["__RequestVerificationToken"] = Token(html);
        }

        return await client.PostAsync(path, new FormUrlEncodedContent(fields));
    }

    private static async Task<string> HtmlAsync(HttpResponseMessage response)
    {
        return WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
    }

    private static string Token(string html)
    {
        var match = Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
        if (!match.Success)
        {
            match = Regex.Match(html, "value=\"([^\"]+)\"[^>]*name=\"__RequestVerificationToken\"");
        }

        return match.Groups[1].Value;
    }

    private static string NewEmail()
    {
        return $"painel-{Guid.NewGuid():N}@example.com";
    }

    private static string Nome(string prefixo)
    {
        return $"{prefixo} {Guid.NewGuid():N}";
    }
}
