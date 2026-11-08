using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Perfis;

namespace Project.Infrastructure.Persistence.Configurations;

public sealed class PerfilConfiguration : IEntityTypeConfiguration<Perfil>
{
    public void Configure(EntityTypeBuilder<Perfil> builder)
    {
        builder.ToTable("perfis");
        builder.HasKey(perfil => perfil.Id);
        builder.Property(perfil => perfil.AccountId).HasMaxLength(450).IsRequired();
        builder.HasIndex(perfil => perfil.AccountId).IsUnique();
    }
}
