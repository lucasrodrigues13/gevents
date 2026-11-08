using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Project.Infrastructure.Identity;

namespace Project.IntegrationTests;

[CollectionDefinition(Name)]
public sealed class PostgresCollection : ICollectionFixture<PostgresFixture>
{
    public const string Name = "postgres";
}

[Collection(PostgresCollection.Name)]
public sealed class AccountTests : IAsyncLifetime
{
    private const string Password = "correta-senha";
    private readonly PostgresFixture _postgres;
    private GeventsFactory _factory = null!;

    public AccountTests(PostgresFixture postgres)
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
    public async Task Cadastro_valido_cria_usuario_e_envia_confirmacao()
    {
        var email = NewEmail();
        var client = _factory.CreateClient();

        var response = await PostAsync(client, "/Account/Register", email, Password);

        var body = await HtmlAsync(response);
        Assert.Contains("Enviamos o link de confirmação", body);
        var user = await FindAsync(email);
        Assert.NotNull(user);
        Assert.False(user.EmailConfirmed);
        Assert.Equal([Roles.User], await RolesOfAsync(user));
        Assert.Contains(_factory.Services.GetRequiredService<FakeEmailSender>().Sent, sent => sent.Email == email);
    }

    [Fact]
    public async Task Cadastro_duplicado_nao_cria_segunda_conta()
    {
        var email = NewEmail();
        var client = _factory.CreateClient();
        await PostAsync(client, "/Account/Register", email, Password);

        var again = await PostAsync(client, "/Account/Register", email.ToUpperInvariant(), Password);

        Assert.Contains("Já existe uma conta com este e-mail", await HtmlAsync(again));
        Assert.Single(await UsersByNormalizedEmailAsync(email));
    }

    [Fact]
    public async Task Login_antes_da_confirmacao_falha_e_depois_do_link_entra()
    {
        var email = NewEmail();
        var client = _factory.CreateClient();
        await PostAsync(client, "/Account/Register", email, Password);

        var blocked = await PostAsync(client, "/Account/Login", email, Password);
        Assert.Contains("Confirme o e-mail antes de entrar.", await HtmlAsync(blocked));

        await OpenLatestLinkAsync(client, email);
        var entered = await PostAsync(client, "/Account/Login", email, Password);
        Assert.Contains("Contato inicial", await HtmlAsync(entered));
    }

    [Fact]
    public async Task Link_de_confirmacao_invalido_nao_confirma()
    {
        var email = NewEmail();
        var client = _factory.CreateClient();
        await PostAsync(client, "/Account/Register", email, Password);
        var link = LatestLink(email);
        var broken = link.Replace("code=", "code=invalido", StringComparison.Ordinal);

        var response = await client.GetAsync(new Uri(broken).PathAndQuery);

        Assert.Contains("inválido ou expirou", await HtmlAsync(response));
        Assert.False((await FindAsync(email))!.EmailConfirmed);
    }

    [Fact]
    public async Task Login_com_senha_errada_nao_revela_se_o_email_existe()
    {
        var email = NewEmail();
        var client = _factory.CreateClient();
        await RegisterAndConfirmAsync(client, email);

        var wrong = await PostAsync(client, "/Account/Login", email, "outra-senha-errada");
        var missing = await PostAsync(client, "/Account/Login", NewEmail(), Password);
        var wrongBody = await HtmlAsync(wrong);
        var missingBody = await HtmlAsync(missing);

        Assert.Contains("E-mail ou senha incorretos.", wrongBody);
        Assert.Contains("E-mail ou senha incorretos.", missingBody);
    }

    [Fact]
    public async Task Usuario_recebe_403_na_area_admin_e_admin_entra()
    {
        var email = NewEmail();
        var userClient = _factory.CreateClient();
        await RegisterAndConfirmAsync(userClient, email);
        await PostAsync(userClient, "/Account/Login", email, Password);

        var denied = await userClient.GetAsync("/Admin");
        Assert.Equal(HttpStatusCode.Forbidden, denied.StatusCode);

        var adminEmail = NewEmail();
        await BootstrapAsync(adminEmail, Password);
        var adminClient = _factory.CreateClient();
        var entered = await PostAsync(adminClient, "/Account/Login", adminEmail, Password);
        Assert.Contains("Área administrativa", await HtmlAsync(entered));
    }

    [Fact]
    public async Task Cinco_senhas_erradas_bloqueiam_por_quinze_minutos()
    {
        var email = NewEmail();
        var client = _factory.CreateClient();
        await RegisterAndConfirmAsync(client, email);

        string body = "";
        for (var attempt = 0; attempt < 5; attempt++)
        {
            var response = await PostAsync(client, "/Account/Login", email, "outra-senha-errada");
            body = await HtmlAsync(response);
        }

        Assert.Contains("Muitas tentativas.", body);
        var user = await FindAsync(email);
        Assert.NotNull(user!.LockoutEnd);
        var remaining = user.LockoutEnd.Value - DateTimeOffset.UtcNow;
        Assert.InRange(remaining.TotalMinutes, 14, 16);
    }

    [Fact]
    public async Task Redefinicao_valida_troca_a_senha_e_o_pedido_nao_revela_a_conta()
    {
        var email = NewEmail();
        var client = _factory.CreateClient();
        await RegisterAndConfirmAsync(client, email);

        var known = await PostAsync(client, "/Account/ForgotPassword", email, "");
        var unknown = await PostAsync(client, "/Account/ForgotPassword", NewEmail(), "");
        Assert.Contains("Se existir uma conta", await HtmlAsync(known));
        Assert.Contains("Se existir uma conta", await HtmlAsync(unknown));

        var link = new Uri(LatestLink(email, SentEmailKind.PasswordReset));
        var page = await client.GetAsync(link.PathAndQuery);
        var html = await HtmlAsync(page);
        var token = Token(html);
        var reset = await client.PostAsync(link.PathAndQuery, new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Input.Email"] = email,
            ["Input.Code"] = QueryValue(link, "code"),
            ["Input.Password"] = "nova-senha-valida"
        }));

        Assert.Contains("Senha redefinida", await HtmlAsync(reset));
        var entered = await PostAsync(client, "/Account/Login", email, "nova-senha-valida");
        Assert.Contains("Contato inicial", await HtmlAsync(entered));
    }

    [Fact]
    public async Task Redefinicao_invalida_nao_troca_a_senha()
    {
        var email = NewEmail();
        var client = _factory.CreateClient();
        await RegisterAndConfirmAsync(client, email);
        await PostAsync(client, "/Account/ForgotPassword", email, "");
        var link = new Uri(LatestLink(email, SentEmailKind.PasswordReset));
        var html = await HtmlAsync(await client.GetAsync(link.PathAndQuery));

        var reset = await client.PostAsync(link.PathAndQuery, new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = Token(html),
            ["Input.Email"] = email,
            ["Input.Code"] = "token-invalido",
            ["Input.Password"] = "nova-senha-valida"
        }));

        Assert.Contains("inválido ou expirou", await HtmlAsync(reset));
        Assert.False(await CheckPasswordAsync(email, "nova-senha-valida"));
    }

    [Fact]
    public async Task Reenvio_gera_outro_link()
    {
        var email = NewEmail();
        var client = _factory.CreateClient();
        await PostAsync(client, "/Account/Register", email, Password);
        var before = _factory.Services.GetRequiredService<FakeEmailSender>().Sent.Count(sent => sent.Email == email);

        await PostAsync(client, "/Account/ResendConfirmation", email, "");

        var after = _factory.Services.GetRequiredService<FakeEmailSender>().Sent.Count(sent => sent.Email == email);
        Assert.True(after > before);
    }

    [Fact]
    public async Task Cadastro_acima_do_limite_responde_429()
    {
        await using var factory = new GeventsFactory(_postgres.ConnectionString, signupPerHour: 2);
        var client = factory.CreateClient();
        await PostAsync(client, "/Account/Register", NewEmail(), Password);
        await PostAsync(client, "/Account/Register", NewEmail(), Password);

        var blocked = await PostAsync(client, "/Account/Register", NewEmail(), Password);

        Assert.Equal(HttpStatusCode.TooManyRequests, blocked.StatusCode);
        Assert.True(blocked.Headers.Contains("Retry-After"));
    }

    [Fact]
    public async Task Falha_de_smtp_no_cadastro_nao_confirma_o_email()
    {
        var email = NewEmail();
        var sender = _factory.Services.GetRequiredService<FakeEmailSender>();
        sender.Fail = true;
        try
        {
            var client = _factory.CreateClient();
            var response = await PostAsync(client, "/Account/Register", email, Password);
            Assert.Contains("confirmação não foi enviada", await HtmlAsync(response));
            Assert.False((await FindAsync(email))!.EmailConfirmed);
        }
        finally
        {
            sender.Fail = false;
        }
    }

    [Fact]
    public async Task Tokens_de_confirmacao_e_redefinicao_tem_prazos_diferentes()
    {
        var confirmation = _factory.Services.GetRequiredService<IOptions<EmailConfirmationTokenProviderOptions>>().Value;
        var reset = _factory.Services.GetRequiredService<IOptions<DataProtectionTokenProviderOptions>>().Value;

        Assert.Equal(TimeSpan.FromHours(24), confirmation.TokenLifespan);
        Assert.Equal(TimeSpan.FromHours(1), reset.TokenLifespan);
    }

    [Fact]
    public async Task Bootstrap_e_idempotente_nao_troca_senha_e_nao_promove_usuario()
    {
        var adminEmail = NewEmail();
        await BootstrapAsync(adminEmail, Password);
        await BootstrapAsync(adminEmail, "outra-senha-admin");
        Assert.True(await CheckPasswordAsync(adminEmail, Password));
        Assert.False(await CheckPasswordAsync(adminEmail, "outra-senha-admin"));

        var userEmail = NewEmail();
        var client = _factory.CreateClient();
        await PostAsync(client, "/Account/Register", userEmail, Password);
        await Assert.ThrowsAsync<InvalidOperationException>(() => BootstrapAsync(userEmail, Password));
        Assert.Equal([Roles.User], await RolesOfAsync((await FindAsync(userEmail))!));
    }

    [Fact]
    public async Task Bootstrap_sem_credencial_falha()
    {
        using var scope = _factory.Services.CreateScope();
        var bootstrapper = new AdminBootstrapper(
            scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>(),
            scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(),
            Options.Create(new BootstrapOptions { Enabled = true }),
            scope.ServiceProvider.GetRequiredService<ILogger<AdminBootstrapper>>());

        await Assert.ThrowsAsync<InvalidOperationException>(() => bootstrapper.RunAsync(CancellationToken.None));
    }

    private async Task RegisterAndConfirmAsync(HttpClient client, string email)
    {
        await PostAsync(client, "/Account/Register", email, Password);
        await OpenLatestLinkAsync(client, email);
    }

    private async Task OpenLatestLinkAsync(HttpClient client, string email)
    {
        var link = new Uri(LatestLink(email));
        await client.GetAsync(link.PathAndQuery);
    }

    private string LatestLink(string email, SentEmailKind kind = SentEmailKind.Confirmation)
    {
        return _factory.Services.GetRequiredService<FakeEmailSender>().Sent
            .Last(sent => sent.Email == email && sent.Kind == kind)
            .Payload;
    }

    private async Task BootstrapAsync(string email, string password)
    {
        using var scope = _factory.Services.CreateScope();
        var bootstrapper = new AdminBootstrapper(
            scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>(),
            scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(),
            Options.Create(new BootstrapOptions { Enabled = true, Email = email, Password = password }),
            scope.ServiceProvider.GetRequiredService<ILogger<AdminBootstrapper>>());
        await bootstrapper.RunAsync(CancellationToken.None);
    }

    private async Task<IdentityUser?> FindAsync(string email)
    {
        using var scope = _factory.Services.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>().FindByEmailAsync(email);
    }

    private async Task<IList<string>> RolesOfAsync(IdentityUser user)
    {
        using var scope = _factory.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var stored = await manager.FindByIdAsync(user.Id);
        return await manager.GetRolesAsync(stored!);
    }

    private async Task<bool> CheckPasswordAsync(string email, string password)
    {
        using var scope = _factory.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var user = await manager.FindByEmailAsync(email);
        return user is not null && await manager.CheckPasswordAsync(user, password);
    }

    private async Task<List<IdentityUser>> UsersByNormalizedEmailAsync(string email)
    {
        using var scope = _factory.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var normalized = manager.NormalizeEmail(email);
        return manager.Users.Where(user => user.NormalizedEmail == normalized).ToList();
    }

    private static async Task<string> HtmlAsync(HttpResponseMessage response)
    {
        return WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
    }

    private static async Task<HttpResponseMessage> PostAsync(HttpClient client, string path, string email, string password)
    {
        var html = await HtmlAsync(await client.GetAsync(path));
        var fields = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = Token(html),
            ["Input.Email"] = email
        };
        if (password.Length > 0)
        {
            fields["Input.Password"] = password;
        }

        return await client.PostAsync(path, new FormUrlEncodedContent(fields));
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

    private static string QueryValue(Uri uri, string name)
    {
        var pair = uri.Query.TrimStart('?').Split('&').First(part => part.StartsWith(name + "=", StringComparison.Ordinal));
        return Uri.UnescapeDataString(pair[(name.Length + 1)..]);
    }

    private static string NewEmail()
    {
        return $"pessoa-{Guid.NewGuid():N}@example.com";
    }
}
