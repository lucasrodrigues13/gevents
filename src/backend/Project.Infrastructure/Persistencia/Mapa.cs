using Microsoft.EntityFrameworkCore;
using Project.Domain.Clientes;
using Project.Domain.Convidados;
using Project.Domain.Eventos;
using Project.Domain.Perfil;
using Project.Domain.Tarefas;

namespace Project.Infrastructure.Persistencia;

public static class Mapa
{
    public static void Aplicar(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PerfilAssessoria>().HasIndex(perfil => perfil.ContaId).IsUnique();

        modelBuilder.Entity<ModeloEvento>().Property(modelo => modelo.Tipo).HasConversion<string>();
        modelBuilder.Entity<ModeloEvento>().HasMany(modelo => modelo.Servicos).WithOne().HasForeignKey(servico => servico.ModeloId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ModeloEvento>().HasMany(modelo => modelo.Tarefas).WithOne().HasForeignKey(tarefa => tarefa.ModeloId).OnDelete(DeleteBehavior.Cascade);

        var evento = modelBuilder.Entity<Evento>();
        evento.Property(item => item.Tipo).HasConversion<string>();
        evento.Property(item => item.Situacao).HasConversion<string>();
        evento.Property(item => item.OrcamentoEstimado).HasPrecision(12, 2);
        evento.OwnsOne(item => item.Cerimonia, local => local.Property<bool>("Persistido").HasDefaultValue(true));
        evento.OwnsOne(item => item.Recepcao, local => local.Property<bool>("Persistido").HasDefaultValue(true));
        evento.HasMany(item => item.Servicos).WithOne().HasForeignKey(servico => servico.EventoId).OnDelete(DeleteBehavior.Cascade);
        evento.HasMany(item => item.LocaisTransfer).WithOne().HasForeignKey(local => local.EventoId).OnDelete(DeleteBehavior.Cascade);
        evento.HasMany(item => item.Tags).WithMany(tag => tag.Eventos);

        modelBuilder.Entity<ServicoEvento>().Property(servico => servico.ValorContratado).HasPrecision(12, 2);
        modelBuilder.Entity<ServicoEvento>().HasMany(servico => servico.Parcelas).WithOne().HasForeignKey(parcela => parcela.ServicoId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ParcelaServico>().Property(parcela => parcela.Valor).HasPrecision(12, 2);

        modelBuilder.Entity<TagConta>().HasIndex(tag => new { tag.ContaId, tag.Nome }).IsUnique();

        var tarefa = modelBuilder.Entity<Tarefa>();
        tarefa.Property(item => item.Situacao).HasConversion<string>();
        tarefa.HasOne<Evento>().WithMany().HasForeignKey(item => item.EventoId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Cliente>().Property(cliente => cliente.Papel).HasConversion<string>();
        modelBuilder.Entity<VinculoCliente>().HasKey(vinculo => new { vinculo.ClienteId, vinculo.EventoId });
        modelBuilder.Entity<VinculoCliente>().HasOne<Cliente>().WithMany(cliente => cliente.Vinculos).HasForeignKey(vinculo => vinculo.ClienteId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<VinculoCliente>().HasOne<Evento>().WithMany().HasForeignKey(vinculo => vinculo.EventoId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Convite>().HasIndex(convite => convite.Token).IsUnique();
        modelBuilder.Entity<Convite>().HasOne<Evento>().WithMany().HasForeignKey(convite => convite.EventoId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Convite>().HasOne<GrupoConvidados>().WithMany().HasForeignKey(convite => convite.GrupoId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Convite>().HasMany(convite => convite.Convidados).WithOne().HasForeignKey(convidado => convidado.ConviteId).OnDelete(DeleteBehavior.Cascade);

        var convidado = modelBuilder.Entity<Convidado>();
        convidado.Property(item => item.Situacao).HasConversion<string>();
        convidado.Property(item => item.Genero).HasConversion<string>();
        convidado.Property(item => item.Faixa).HasConversion<string>();
        convidado.Property(item => item.Custo).HasConversion<string>();

        modelBuilder.Entity<CompromissoAgenda>().HasIndex(compromisso => compromisso.EventoId).IsUnique();
        modelBuilder.Entity<CompromissoAgenda>().HasOne<Evento>().WithMany().HasForeignKey(compromisso => compromisso.EventoId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ArquivoRegistro>().Property(arquivo => arquivo.CaminhoRelativo).IsRequired();
    }
}
