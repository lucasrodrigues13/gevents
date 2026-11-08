using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Convites;
using Project.Domain.Eventos;

namespace Project.Infrastructure.Persistence.Configurations;

public sealed class GrupoConfiguration : IEntityTypeConfiguration<Grupo>
{
    public void Configure(EntityTypeBuilder<Grupo> builder)
    {
        builder.ToTable("grupos");
        builder.HasKey(grupo => grupo.Id);
        builder.Property(grupo => grupo.AccountId).HasMaxLength(450).IsRequired();
        builder.Property(grupo => grupo.Nome).IsRequired();
        builder.HasIndex(grupo => grupo.AccountId);
    }
}

public sealed class ConviteConfiguration : IEntityTypeConfiguration<Convite>
{
    public void Configure(EntityTypeBuilder<Convite> builder)
    {
        builder.ToTable("convites");
        builder.HasKey(convite => convite.Id);
        builder.Property(convite => convite.AccountId).HasMaxLength(450).IsRequired();
        builder.Property(convite => convite.Nome).IsRequired();
        builder.HasIndex(convite => convite.Link).IsUnique();
        builder.HasIndex(convite => convite.AccountId);
        builder.HasOne<Evento>()
            .WithMany()
            .HasForeignKey(convite => convite.EventoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class ConvidadoConfiguration : IEntityTypeConfiguration<Convidado>
{
    public void Configure(EntityTypeBuilder<Convidado> builder)
    {
        builder.ToTable("convidados");
        builder.HasKey(convidado => convidado.Id);
        builder.Property(convidado => convidado.Nome).IsRequired();
        builder.Property(convidado => convidado.Situacao).IsRequired();
        builder.HasOne<Convite>()
            .WithMany()
            .HasForeignKey(convidado => convidado.ConviteId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Grupo>()
            .WithMany()
            .HasForeignKey(convidado => convidado.GrupoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
