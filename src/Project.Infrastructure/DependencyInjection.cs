using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.Application.Arquivos;
using Project.Application.Cep;
using Project.Application.Clientes;
using Project.Application.Convites;
using Project.Application.Dashboards;
using Project.Application.Eventos;
using Project.Application.Perfis;
using Project.Application.Tarefas;
using Project.Infrastructure.Arquivos;
using Project.Infrastructure.Cep;
using Project.Infrastructure.Email;
using Project.Infrastructure.Identity;
using Project.Infrastructure.Convites;
using Project.Infrastructure.Persistence;

namespace Project.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.AllowedForNewUsers = true;
                options.Tokens.EmailConfirmationTokenProvider = EmailConfirmationTokenProviderOptions.ProviderName;
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<EmailConfirmationTokenProvider<IdentityUser>>(EmailConfirmationTokenProviderOptions.ProviderName);

        services.Configure<DataProtectionTokenProviderOptions>(options =>
            options.TokenLifespan = TimeSpan.FromHours(1));
        services.Configure<EmailConfirmationTokenProviderOptions>(options =>
            options.TokenLifespan = TimeSpan.FromHours(24));

        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
        services.AddSingleton<IEmailSender<IdentityUser>, MailKitEmailSender>();

        services.Configure<BootstrapOptions>(configuration.GetSection(BootstrapOptions.SectionName));
        services.AddScoped<AdminBootstrapper>();
        services.AddHostedService<AdminBootstrapHostedService>();
        services.AddScoped<PerfilRepositorio>();
        services.AddScoped<IPerfilConsulta>(provider => provider.GetRequiredService<PerfilRepositorio>());
        services.AddScoped<IPerfilRepositorio>(provider => provider.GetRequiredService<PerfilRepositorio>());
        services.AddScoped<PerfilServico>();

        services.Configure<StorageOptions>(configuration.GetSection(StorageOptions.SectionName));
        services.AddScoped<ArquivoMetadadoRepositorio>();
        services.AddScoped<IArquivoMetadadoRepositorio>(provider => provider.GetRequiredService<ArquivoMetadadoRepositorio>());
        services.AddScoped<IArmazenamentoArquivo, ArmazenamentoArquivo>();
        services.AddScoped<EventoRepositorio>();
        services.AddScoped<IEventoConsulta>(provider => provider.GetRequiredService<EventoRepositorio>());
        services.AddScoped<IEventoRepositorio>(provider => provider.GetRequiredService<EventoRepositorio>());
        services.AddScoped<EventoServico>();
        services.AddScoped<ModeloRepositorio>();
        services.AddScoped<IModeloConsulta>(provider => provider.GetRequiredService<ModeloRepositorio>());
        services.AddScoped<IModeloRepositorio>(provider => provider.GetRequiredService<ModeloRepositorio>());
        services.AddScoped<ModeloServico>();
        services.Configure<CepOptions>(configuration.GetSection(CepOptions.SectionName));
        services.AddHttpClient<IConsultaCep, ConsultaCepHttp>();
        services.AddScoped<TarefaRepositorio>();
        services.AddScoped<ITarefaConsulta>(provider => provider.GetRequiredService<TarefaRepositorio>());
        services.AddScoped<ITarefaRepositorio>(provider => provider.GetRequiredService<TarefaRepositorio>());
        services.AddScoped<TarefaServico>();
        services.AddScoped<GrupoRepositorio>();
        services.AddScoped<IGrupoConsulta>(provider => provider.GetRequiredService<GrupoRepositorio>());
        services.AddScoped<IGrupoRepositorio>(provider => provider.GetRequiredService<GrupoRepositorio>());
        services.AddScoped<GrupoServico>();
        services.AddScoped<ConviteRepositorio>();
        services.AddScoped<IConviteConsulta>(provider => provider.GetRequiredService<ConviteRepositorio>());
        services.AddScoped<IConviteRepositorio>(provider => provider.GetRequiredService<ConviteRepositorio>());
        services.AddScoped<ConviteServico>();
        services.AddSingleton<ILeitorPlanilha, LeitorPlanilhaConvite>();
        services.AddSingleton<ICodigoQr, CodigoQrConvite>();
        services.AddScoped<ClienteRepositorio>();
        services.AddScoped<IClienteConsulta>(provider => provider.GetRequiredService<ClienteRepositorio>());
        services.AddScoped<IClienteRepositorio>(provider => provider.GetRequiredService<ClienteRepositorio>());
        services.AddScoped<ClienteServico>();
        services.AddScoped<DashboardRepositorio>();
        services.AddScoped<IDashboardConsulta>(provider => provider.GetRequiredService<DashboardRepositorio>());
        services.AddScoped<DashboardServico>();

        return services;
    }
}
