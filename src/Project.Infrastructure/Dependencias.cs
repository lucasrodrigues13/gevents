using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.Application.Conta;
using Project.Infrastructure.Conta;
using Project.Infrastructure.Persistencia;

namespace Project.Infrastructure;

public static class Dependencias
{
    public static IServiceCollection AddInfraestrutura(this IServiceCollection services, IConfiguration configuration)
    {
        var conexao = configuration.GetConnectionString("Default");
        if (string.IsNullOrWhiteSpace(conexao))
        {
            throw new InvalidOperationException("A conexão com o banco não está configurada.");
        }

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(conexao));

        services.AddIdentityCore<IdentityUser>(OpcoesDeConta.Aplicar)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager()
            .AddTokenProvider<ConfirmacaoEmailTokenProvider>(ConfirmacaoEmailTokenProvider.Nome)
            .AddTokenProvider<RedefinicaoSenhaTokenProvider>(RedefinicaoSenhaTokenProvider.Nome);

        services.Configure<JwtOpcoes>(configuration.GetSection("Jwt"));
        services.Configure<EmailOpcoes>(configuration.GetSection("Email"));
        services.Configure<BootstrapOpcoes>(configuration.GetSection("Bootstrap"));
        services.AddSingleton<IEmissorJwt, EmissorJwt>();
        services.AddScoped<IEmailSender<IdentityUser>, MailKitEmailSender>();
        services.AddScoped<GarantiaDePapeis>();
        services.AddScoped<BootstrapAdministrador>();

        return services;
    }
}
