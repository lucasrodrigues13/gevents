using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project.Domain.Clientes;
using Project.Domain.Convidados;
using Project.Domain.Eventos;
using Project.Domain.Perfil;
using Project.Domain.Tarefas;

namespace Project.Infrastructure.Persistencia;

public sealed class AppDbContext : IdentityDbContext
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        Mapa.Aplicar(modelBuilder);
    }
}
