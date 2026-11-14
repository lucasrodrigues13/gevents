using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project.Domain.Clientes;
using Project.Domain.Convidados;
using Project.Domain.Eventos;
using Project.Domain.Fornecedores;
using Project.Domain.Funil;
using Project.Domain.Modelos;
using Project.Domain.Perfil;
using Project.Domain.Tarefas;
using Project.Infrastructure.Conta;

namespace Project.Infrastructure.Persistencia;

public sealed class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<PerfilAssessoria> Perfis => Set<PerfilAssessoria>();

    public DbSet<ModeloEvento> Modelos => Set<ModeloEvento>();

    public DbSet<Evento> Eventos => Set<Evento>();

    public DbSet<TagConta> Tags => Set<TagConta>();

    public DbSet<Tarefa> Tarefas => Set<Tarefa>();

    public DbSet<Cliente> Clientes => Set<Cliente>();

    public DbSet<VinculoCliente> Vinculos => Set<VinculoCliente>();

    public DbSet<GrupoConvidados> Grupos => Set<GrupoConvidados>();

    public DbSet<Convite> Convites => Set<Convite>();

    public DbSet<Convidado> Convidados => Set<Convidado>();

    public DbSet<CompromissoAgenda> Compromissos => Set<CompromissoAgenda>();

    public DbSet<ArquivoRegistro> Arquivos => Set<ArquivoRegistro>();

    public DbSet<ModeloBriefing> ModelosBriefing => Set<ModeloBriefing>();

    public DbSet<ModeloRoteiro> ModelosRoteiro => Set<ModeloRoteiro>();

    public DbSet<ModeloCronograma> ModelosCronograma => Set<ModeloCronograma>();

    public DbSet<ModeloInspiracao> ModelosInspiracao => Set<ModeloInspiracao>();

    public DbSet<ModeloMensagem> ModelosMensagem => Set<ModeloMensagem>();

    public DbSet<ServicoConta> ServicosConta => Set<ServicoConta>();

    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();

    public DbSet<Oportunidade> Oportunidades => Set<Oportunidade>();

    public DbSet<LeadRsvp> LeadsRsvp => Set<LeadRsvp>();

    public DbSet<MetricasRsvpFunil> MetricasRsvpFunil => Set<MetricasRsvpFunil>();

    public DbSet<ContratoFunil> ContratosFunil => Set<ContratoFunil>();

    public DbSet<OrcamentoFunil> OrcamentosFunil => Set<OrcamentoFunil>();

    public DbSet<TarefaFunil> TarefasFunil => Set<TarefaFunil>();

    public DbSet<ModeloServicoFunil> ModelosServicoFunil => Set<ModeloServicoFunil>();

    public DbSet<ModeloOrcamentoFunil> ModelosOrcamentoFunil => Set<ModeloOrcamentoFunil>();

    public DbSet<ModeloContratoFunil> ModelosContratoFunil => Set<ModeloContratoFunil>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        Mapa.Aplicar(modelBuilder);
    }
}
