using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Project.Application.Cep;

namespace Project.IntegrationTests.Suporte;

public sealed class FabricaApi : WebApplicationFactory<Program>
{
    private readonly string _conexao;
    private readonly bool _bootstrap;
    private readonly string? _adminEmail;
    private readonly string? _adminSenha;
    private readonly int _cadastroPorHora;
    private readonly int _loginPorMinuto;
    private readonly int _recuperacaoPorHora;
    private readonly int _reenvioPorHora;

    public FabricaApi(
        string conexao,
        EmailCapturado? email = null,
        bool bootstrap = false,
        string? adminEmail = null,
        string? adminSenha = null,
        int cadastroPorHora = 1000,
        int loginPorMinuto = 1000,
        int recuperacaoPorHora = 1000,
        int reenvioPorHora = 1000)
    {
        _conexao = conexao;
        Email = email ?? new EmailCapturado();
        Logs = new ColetorLog();
        _bootstrap = bootstrap;
        _adminEmail = adminEmail;
        _adminSenha = adminSenha;
        _cadastroPorHora = cadastroPorHora;
        _loginPorMinuto = loginPorMinuto;
        _recuperacaoPorHora = recuperacaoPorHora;
        _reenvioPorHora = reenvioPorHora;
    }

    public EmailCapturado Email { get; }

    public ColetorLog Logs { get; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:Default", _conexao);
        builder.UseSetting("Jwt:Secret", "segredo-de-teste-com-32-caracteres");
        builder.UseSetting("Jwt:Issuer", "gevents-test");
        builder.UseSetting("Jwt:Audience", "gevents-test");
        builder.UseSetting("Jwt:ExpiryHours", "8");
        builder.UseSetting("PublicUrl", "http://localhost:4200");
        builder.UseSetting("DataProtection:KeysPath", Path.Combine(Path.GetTempPath(), "gevents-chaves-teste"));
        builder.UseSetting("Email:Password", "smtp-segredo-nao-logar");
        builder.UseSetting("Bootstrap:Enabled", _bootstrap ? "true" : "false");
        builder.UseSetting("Bootstrap:AdminEmail", _adminEmail ?? "");
        builder.UseSetting("Bootstrap:AdminPassword", _adminSenha ?? "");
        builder.UseSetting("RateLimit:CadastroPorHora", _cadastroPorHora.ToString());
        builder.UseSetting("RateLimit:LoginPorMinuto", _loginPorMinuto.ToString());
        builder.UseSetting("RateLimit:RecuperacaoPorHora", _recuperacaoPorHora.ToString());
        builder.UseSetting("RateLimit:ReenvioPorHora", _reenvioPorHora.ToString());
        builder.UseSetting("Swagger:Enabled", "false");
        builder.UseSetting("Storage:Root", Path.Combine(Path.GetTempPath(), "gevents-arquivos-teste", Guid.NewGuid().ToString("N")));
        builder.UseSetting("Storage:MaxImageBytes", "5242880");
        builder.ConfigureLogging(logging => logging.AddProvider(Logs));
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IEmailSender<IdentityUser>>();
            services.AddSingleton<IEmailSender<IdentityUser>>(Email);
            services.RemoveAll<IConsultaCep>();
            services.AddSingleton<IConsultaCep, CepDeTeste>();
        });
    }
}
