using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Eventos;
using Project.Domain.Tarefas;

namespace Project.Infrastructure.Persistence.Configurations;

public sealed class TarefaConfiguration : IEntityTypeConfiguration<Tarefa>
{
    public void Configure(EntityTypeBuilder<Tarefa> builder)
    {
        builder.ToTable("tarefas");
        builder.HasKey(tarefa => tarefa.Id);
        builder.Property(tarefa => tarefa.AccountId).HasMaxLength(450).IsRequired();
        builder.Property(tarefa => tarefa.Titulo).IsRequired();
        builder.Property(tarefa => tarefa.Situacao).IsRequired();
        builder.HasIndex(tarefa => tarefa.AccountId);
        builder.HasOne<Evento>()
            .WithMany()
            .HasForeignKey(tarefa => tarefa.EventoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
