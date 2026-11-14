using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.Application.Arquivos;
using Project.Application.Cep;
using Project.Application.Conta;
using Project.Application.Convidados;
using Project.Application.Negocio;
using Project.Infrastructure.Arquivos;
using Project.Infrastructure.Cep;
using Project.Infrastructure.Conta;
using Project.Infrastructure.Convidados;
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

        services.AddIdentityCore<ApplicationUser>(OpcoesDeConta.Aplicar)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager()
            .AddTokenProvider<ConfirmacaoEmailTokenProvider>(ConfirmacaoEmailTokenProvider.Nome)
            .AddTokenProvider<RedefinicaoSenhaTokenProvider>(RedefinicaoSenhaTokenProvider.Nome);

        services.Configure<JwtOpcoes>(configuration.GetSection("Jwt"));
        services.Configure<EmailOpcoes>(configuration.GetSection("Email"));
        services.Configure<BootstrapOpcoes>(configuration.GetSection("Bootstrap"));
        services.Configure<ArmazenamentoOpcoes>(configuration.GetSection("Storage"));
        services.AddSingleton<IEmissorJwt, EmissorJwt>();
        services.AddScoped<IDados, DadosEf>();
        services.AddScoped<PerfilServico>();
        services.AddScoped<ModeloServico>();
        services.AddScoped<EstatisticasServico>();
        services.AddScoped<BriefingServico>();
        services.AddScoped<RoteiroServico>();
        services.AddScoped<CronogramaServico>();
        services.AddScoped<InspiracaoServico>();
        services.AddScoped<MensagemServico>();
        services.AddScoped<FornecedorServico>();
        services.AddScoped<PainelServico>();
        services.AddScoped<EventoServico>();
        services.AddScoped<TarefaServico>();
        services.AddScoped<ClienteServico>();
        services.AddScoped<ConviteServico>();
        services.AddScoped<AgendaServico>();
        services.AddScoped<OportunidadeServico>();
        services.AddScoped<LeadRsvpServico>();
        services.AddScoped<ContratoFunilServico>();
        services.AddScoped<OrcamentoFunilServico>();
        services.AddScoped<TarefaFunilServico>();
        services.AddScoped<RelatorioFunilServico>();
        services.AddScoped<ModeloServicoFunilServico>();
        services.AddScoped<ModeloOrcamentoFunilServico>();
        services.AddScoped<ModeloContratoFunilServico>();
        services.AddScoped<IContaCliente, ContaCliente>();
        services.AddScoped<IArmazenamentoArquivo, ArmazenamentoArquivo>();
        services.AddHttpClient<IConsultaCep, ViaCepConsulta>(cliente =>
        {
            cliente.BaseAddress = new Uri("https://viacep.com.br/");
            cliente.Timeout = TimeSpan.FromSeconds(8);
        });
        services.AddSingleton<IPlanilhaConvites, PlanilhaExcel>();
        services.AddSingleton<IGeradorQr, GeradorQr>();
        services.AddScoped<IEmailSender<ApplicationUser>, MailKitEmailSender>();
        services.AddScoped<GarantiaDePapeis>();
        services.AddScoped<BootstrapAdministrador>();

        return services;
    }
}
