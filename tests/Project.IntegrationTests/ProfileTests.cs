using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Project.IntegrationTests;

[Collection(PostgresCollection.Name)]
public sealed class ProfileTests : IAsyncLifetime
{
    private const string Password = "correta-senha";
    private readonly PostgresFixture _postgres;
    private GeventsFactory _factory = null!;

    public ProfileTests(PostgresFixture postgres)
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
    public async Task Salvar_e_reabrir_mantem_os_campos_e_o_email()
    {
        var email = NewEmail();
        var client = await ConfirmedClientAsync(email);

        var saved = await PostProfileAsync(client, "Casa Nova", "12.345.678/0001-90", "rua sem número");
        Assert.Contains("Perfil salvo.", await HtmlAsync(saved));

        var again = await HtmlAsync(await client.GetAsync("/Perfil"));
        Assert.Contains("Casa Nova", again);
        Assert.Contains("12.345.678/0001-90", again);
        Assert.Contains("rua sem número", again);
        Assert.Contains(email, again);

        var replaced = await PostProfileAsync(client, "Casa Antiga", "123", "outra rua");
        Assert.Contains("Perfil salvo.", await HtmlAsync(replaced));
        var reloaded = await HtmlAsync(await client.GetAsync("/Perfil"));
        Assert.Contains("Casa Antiga", reloaded);
        Assert.DoesNotContain("Casa Nova", reloaded);
        Assert.Contains(email, reloaded);
    }

    [Fact]
    public async Task Data_futura_nao_grava()
    {
        var email = NewEmail();
        var client = await ConfirmedClientAsync(email);

        var response = await PostProfileAsync(client, "Casa", "123", "rua", "2099-01-01");

        Assert.Contains("não pode ser futura", await HtmlAsync(response));
        var page = await HtmlAsync(await client.GetAsync("/Perfil"));
        Assert.DoesNotContain("Casa", page);
    }

    [Fact]
    public async Task Duas_contas_nao_leem_o_mesmo_perfil()
    {
        var first = await ConfirmedClientAsync(NewEmail());
        var second = await ConfirmedClientAsync(NewEmail());
        await PostProfileAsync(first, "Primeira", "1", "rua 1");
        await PostProfileAsync(second, "Segunda", "2", "rua 2");

        Assert.Contains("Primeira", await HtmlAsync(await first.GetAsync("/Perfil")));
        Assert.DoesNotContain("Segunda", await HtmlAsync(await first.GetAsync("/Perfil")));
        Assert.Contains("Segunda", await HtmlAsync(await second.GetAsync("/Perfil")));
    }

    private async Task<HttpClient> ConfirmedClientAsync(string email)
    {
        var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true
        });
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

    private static Task<HttpResponseMessage> PostProfileAsync(
        HttpClient client,
        string company,
        string document,
        string address,
        string? birth = null)
    {
        var fields = new Dictionary<string, string>
        {
            ["Input.NomeEmpresa"] = company,
            ["Input.Documento"] = document,
            ["Input.Endereco"] = address
        };
        if (birth is not null)
        {
            fields["Input.Nascimento"] = birth;
        }

        return PostAsync(client, "/Perfil", fields);
    }

    private static async Task<HttpResponseMessage> PostAsync(HttpClient client, string path, Dictionary<string, string> fields)
    {
        var html = await HtmlAsync(await client.GetAsync(path));
        fields["__RequestVerificationToken"] = Token(html);
        return await client.PostAsync(path, new FormUrlEncodedContent(fields));
    }

    private static async Task<string> HtmlAsync(HttpResponseMessage response)
    {
        return WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
    }

    private static string Token(string html)
    {
        var match = Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
        return match.Groups[1].Value;
    }

    private static string NewEmail()
    {
        return $"perfil-{Guid.NewGuid():N}@example.com";
    }
}
