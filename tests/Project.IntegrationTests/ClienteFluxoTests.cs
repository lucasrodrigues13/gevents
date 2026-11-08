using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;

namespace Project.IntegrationTests;

[Collection(PostgresCollection.Name)]
public sealed class ClienteFluxoTests : IAsyncLifetime
{
    private const string Password = "correta-senha";
    private readonly PostgresFixture _postgres;
    private GeventsFactory _factory = null!;

    public ClienteFluxoTests(PostgresFixture postgres)
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
    public async Task Cliente_vincula_eventos_da_propria_conta_sem_mexer_no_casal()
    {
        var dono = await ConfirmedClientAsync(NewEmail());
        var nome = Nome("Helena");
        var casal = Nome("Casal Original");
        var primeiro = Nome("Evento Um");
        var segundo = Nome("Evento Dois");
        var primeiroId = await CriarEventoAsync(dono, primeiro, casal);
        var segundoId = await CriarEventoAsync(dono, segundo, null);

        var semNome = await PostAsync(dono, "/Clientes/Criar", new Dictionary<string, string>
        {
            ["Input.Nome"] = "  ",
            ["Input.Telefone"] = Nome("telefone")
        });
        Assert.Contains("Informe o nome do cliente.", await HtmlAsync(semNome));

        var emailRuim = await PostAsync(dono, "/Clientes/Criar", new Dictionary<string, string>
        {
            ["Input.Nome"] = nome,
            ["Input.Email"] = "sem-arroba"
        });
        Assert.Contains("Informe um e-mail válido.", await HtmlAsync(emailRuim));
        Assert.DoesNotContain(nome, await HtmlAsync(await dono.GetAsync("/Clientes")));
        Assert.DoesNotContain("type=\"password\"", await HtmlAsync(await dono.GetAsync("/Clientes/Criar")));

        await PostAsync(dono, "/Clientes/Criar", new Dictionary<string, string> { ["Input.Nome"] = nome });
        var lista = await HtmlAsync(await dono.GetAsync("/Clientes?nome=" + Uri.EscapeDataString("helena")));
        Assert.Contains(nome, lista);
        var clienteId = Regex.Match(lista, $"Editar/([0-9a-fA-F-]{{36}})\"[^>]*>{Regex.Escape(nome)}").Groups[1].Value;

        await PostAsync(dono, $"/Eventos/Home/{primeiroId}?handler=Vincular", new Dictionary<string, string> { ["clienteId"] = clienteId });
        await PostAsync(dono, $"/Eventos/Home/{primeiroId}?handler=Vincular", new Dictionary<string, string> { ["clienteId"] = clienteId });
        var home = await HtmlAsync(await dono.GetAsync($"/Eventos/Home/{primeiroId}"));
        Assert.Single(Regex.Matches(home, "handler=Desvincular"));
        Assert.Contains(casal, home);

        var cliente = await HtmlAsync(await dono.GetAsync($"/Clientes/Editar/{clienteId}"));
        Assert.Contains(primeiro, cliente);
        Assert.DoesNotContain(casal, cliente);

        await PostAsync(dono, $"/Eventos/Home/{segundoId}?handler=Vincular", new Dictionary<string, string> { ["clienteId"] = clienteId });
        var dois = await HtmlAsync(await dono.GetAsync($"/Clientes/Editar/{clienteId}"));
        Assert.Contains(primeiro, dois);
        Assert.Contains(segundo, dois);

        await PostAsync(dono, $"/Eventos/Home/{primeiroId}?handler=Desvincular", new Dictionary<string, string> { ["clienteId"] = clienteId });
        Assert.DoesNotContain("handler=Desvincular", await HtmlAsync(await dono.GetAsync($"/Eventos/Home/{primeiroId}")));
        Assert.DoesNotContain(primeiro, await HtmlAsync(await dono.GetAsync($"/Clientes/Editar/{clienteId}")));
        Assert.Contains(casal, await HtmlAsync(await dono.GetAsync($"/Eventos/Home/{primeiroId}")));

        var outro = await ConfirmedClientAsync(NewEmail());
        Assert.DoesNotContain(nome, await HtmlAsync(await outro.GetAsync("/Clientes?nome=" + Uri.EscapeDataString("helena"))));
        var negado = await outro.PostAsync($"/Eventos/Home/{primeiroId}?handler=Vincular", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = Token(await HtmlAsync(await outro.GetAsync("/Clientes/Criar"))),
            ["clienteId"] = clienteId
        }));
        Assert.DoesNotContain("handler=Desvincular", await HtmlAsync(await dono.GetAsync($"/Eventos/Home/{primeiroId}")));
        Assert.Equal(HttpStatusCode.NotFound, negado.StatusCode);
    }

    private static async Task<Guid> CriarEventoAsync(HttpClient client, string nome, string? casal)
    {
        var campos = new Dictionary<string, string> { ["Input.Nome"] = nome };
        if (casal is not null)
        {
            campos["Input.CasalNome"] = casal;
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
        return $"cliente-{Guid.NewGuid():N}@example.com";
    }

    private static string Nome(string prefixo)
    {
        return $"{prefixo} {Guid.NewGuid():N}";
    }
}
