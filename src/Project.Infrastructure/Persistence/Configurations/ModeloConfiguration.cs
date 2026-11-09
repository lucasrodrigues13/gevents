using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Eventos;

namespace Project.Infrastructure.Persistence.Configurations;

public sealed class ModeloConfiguration : IEntityTypeConfiguration<Modelo>
{
    public void Configure(EntityTypeBuilder<Modelo> builder)
    {
        builder.ToTable("modelos");
        builder.HasKey(modelo => modelo.Id);
        builder.Property(modelo => modelo.AccountId).HasMaxLength(450).IsRequired();
        builder.Property(modelo => modelo.Nome).IsRequired();
        builder.HasIndex(modelo => modelo.AccountId);
        builder.HasMany(modelo => modelo.Servicos)
            .WithOne()
            .HasForeignKey(servico => servico.ModeloId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(modelo => modelo.Servicos).HasField("_servicos").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public sealed class ServicoModeloConfiguration : IEntityTypeConfiguration<ServicoModelo>
{
    public void Configure(EntityTypeBuilder<ServicoModelo> builder)
    {
        builder.ToTable("servicos_modelo");
        builder.HasKey(servico => servico.Id);
        builder.Property(servico => servico.Nome).IsRequired();
        builder.HasMany(servico => servico.Tarefas)
            .WithOne()
            .HasForeignKey(tarefa => tarefa.ServicoModeloId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(servico => servico.Tarefas).HasField("_tarefas").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public sealed class TarefaModeloConfiguration : IEntityTypeConfiguration<TarefaModelo>
{
    public void Configure(EntityTypeBuilder<TarefaModelo> builder)
    {
        builder.ToTable("tarefas_modelo");
        builder.HasKey(tarefa => tarefa.Id);
        builder.Property(tarefa => tarefa.Titulo).IsRequired();
    }
}

public sealed class ServicoEventoConfiguration : IEntityTypeConfiguration<ServicoEvento>
{
    public void Configure(EntityTypeBuilder<ServicoEvento> builder)
    {
        builder.ToTable("servicos_evento");
        builder.HasKey(servico => servico.Id);
        builder.Property(servico => servico.AccountId).HasMaxLength(450).IsRequired();
        builder.Property(servico => servico.Nome).IsRequired();
        builder.Property(servico => servico.Valor).HasPrecision(18, 2);
        builder.HasIndex(servico => new { servico.AccountId, servico.EventoId });
    }
}

public sealed class ParcelaEventoConfiguration : IEntityTypeConfiguration<ParcelaEvento>
{
    public void Configure(EntityTypeBuilder<ParcelaEvento> builder)
    {
        builder.ToTable("parcelas_evento");
        builder.HasKey(parcela => parcela.Id);
        builder.Property(parcela => parcela.AccountId).HasMaxLength(450).IsRequired();
        builder.Property(parcela => parcela.Valor).HasPrecision(18, 2);
        builder.HasIndex(parcela => new { parcela.AccountId, parcela.EventoId });
    }
}

public sealed class CompromissoConfiguration : IEntityTypeConfiguration<Compromisso>
{
    public void Configure(EntityTypeBuilder<Compromisso> builder)
    {
        builder.ToTable("compromissos");
        builder.HasKey(compromisso => compromisso.Id);
        builder.Property(compromisso => compromisso.AccountId).HasMaxLength(450).IsRequired();
        builder.Property(compromisso => compromisso.Nome).IsRequired();
        builder.HasIndex(compromisso => new { compromisso.AccountId, compromisso.EventoId });
    }
}
