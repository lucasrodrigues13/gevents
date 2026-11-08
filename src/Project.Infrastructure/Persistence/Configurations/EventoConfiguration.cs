using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Eventos;
using Project.Infrastructure.Arquivos;

namespace Project.Infrastructure.Persistence.Configurations;

public sealed class EventoConfiguration : IEntityTypeConfiguration<Evento>
{
    public void Configure(EntityTypeBuilder<Evento> builder)
    {
        builder.ToTable("eventos");
        builder.HasKey(evento => evento.Id);
        builder.Property(evento => evento.AccountId).HasMaxLength(450).IsRequired();
        builder.Property(evento => evento.Nome).IsRequired();
        builder.Property(evento => evento.Situacao).IsRequired();
        builder.Property(evento => evento.OrcamentoEstimado).HasPrecision(18, 2);
        builder.Property(evento => evento.Tags).HasColumnType("text[]");
        builder.HasIndex(evento => new { evento.AccountId, evento.Situacao });
        builder.HasOne<ArquivoMetadado>()
            .WithMany()
            .HasForeignKey(evento => evento.ArquivoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
