using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project.Domain.Clientes;
using Project.Domain.Convites;
using Project.Domain.Eventos;
using Project.Domain.Perfis;
using Project.Domain.Tarefas;
using Project.Infrastructure.Arquivos;

namespace Project.Infrastructure.Persistence;

public sealed class AppDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Perfil> Perfis => Set<Perfil>();

    public DbSet<Evento> Eventos => Set<Evento>();

    public DbSet<ArquivoMetadado> Arquivos => Set<ArquivoMetadado>();

    public DbSet<Tarefa> Tarefas => Set<Tarefa>();

    public DbSet<Grupo> Grupos => Set<Grupo>();

    public DbSet<Convite> Convites => Set<Convite>();

    public DbSet<Convidado> Convidados => Set<Convidado>();

    public DbSet<LocalTransfer> LocaisTransfer => Set<LocalTransfer>();

    public DbSet<Cliente> Clientes => Set<Cliente>();

    public DbSet<VinculoCliente> Vinculos => Set<VinculoCliente>();

    public DbSet<LinkCadastroCliente> LinksCadastro => Set<LinkCadastroCliente>();

    public DbSet<Modelo> Modelos => Set<Modelo>();

    public DbSet<ServicoModelo> ServicosModelo => Set<ServicoModelo>();

    public DbSet<TarefaModelo> TarefasModelo => Set<TarefaModelo>();

    public DbSet<ServicoEvento> ServicosEvento => Set<ServicoEvento>();

    public DbSet<ParcelaEvento> ParcelasEvento => Set<ParcelaEvento>();

    public DbSet<Compromisso> Compromissos => Set<Compromisso>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
