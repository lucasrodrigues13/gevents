using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Project.Application.Cep;
using Project.Infrastructure.Persistence;
using Project.Web;

namespace Project.IntegrationTests;

public sealed class GeventsFactory : WebApplicationFactory<Program>
{
    private readonly string _storageRoot = Path.Combine(Path.GetTempPath(), "gevents-tests", Guid.NewGuid().ToString("N"));
    private readonly string _connectionString;
    private readonly int _signupPerHour;
    private readonly int _loginPerMinute;
    private readonly int _passwordResetPerHour;
    private readonly int _resendPerHour;

    public GeventsFactory(
        string connectionString,
        int signupPerHour = 1000,
        int loginPerMinute = 1000,
        int passwordResetPerHour = 1000,
        int resendPerHour = 1000)
    {
        _connectionString = connectionString;
        _signupPerHour = signupPerHour;
        _loginPerMinute = loginPerMinute;
        _passwordResetPerHour = passwordResetPerHour;
        _resendPerHour = resendPerHour;
        Directory.CreateDirectory(_storageRoot);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:Default", _connectionString);
        builder.UseSetting("Bootstrap:Enabled", "false");
        builder.UseSetting("RateLimit:SignupPerHour", _signupPerHour.ToString());
        builder.UseSetting("RateLimit:LoginPerMinute", _loginPerMinute.ToString());
        builder.UseSetting("RateLimit:PasswordResetPerHour", _passwordResetPerHour.ToString());
        builder.UseSetting("RateLimit:ResendConfirmationPerHour", _resendPerHour.ToString());
        builder.UseSetting("Storage:Root", _storageRoot);
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IConsultaCep>();
            services.AddSingleton<IConsultaCep, ConsultaCepFixa>();
            services.RemoveAll<IEmailSender<IdentityUser>>();
            services.AddSingleton<FakeEmailSender>();
            services.AddSingleton<IEmailSender<IdentityUser>>(provider => provider.GetRequiredService<FakeEmailSender>());
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        using var scope = host.Services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        database.Database.Migrate();
        return host;
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        if (Directory.Exists(_storageRoot))
        {
            Directory.Delete(_storageRoot, true);
        }
    }
}

public sealed class ConsultaCepFixa : IConsultaCep
{
    public Task<EnderecoCep?> ConsultarAsync(string cep, CancellationToken cancellationToken)
    {
        var digitos = new string(cep.Where(char.IsDigit).ToArray());
        if (digitos == "30130000")
        {
            return Task.FromResult<EnderecoCep?>(new EnderecoCep("Avenida Afonso Pena", "MG", "Belo Horizonte", "Centro"));
        }

        return Task.FromResult<EnderecoCep?>(null);
    }
}
