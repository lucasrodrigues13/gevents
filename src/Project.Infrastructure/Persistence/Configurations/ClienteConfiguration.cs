using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Clientes;
using Project.Domain.Eventos;

namespace Project.Infrastructure.Persistence.Configurations;

public sealed class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("clientes");
        builder.HasKey(cliente => cliente.Id);
        builder.Property(cliente => cliente.AccountId).HasMaxLength(450).IsRequired();
        builder.Property(cliente => cliente.Nome).IsRequired();
        builder.HasIndex(cliente => cliente.AccountId);
        builder.HasOne<Project.Infrastructure.Arquivos.ArquivoMetadado>()
            .WithMany()
            .HasForeignKey(cliente => cliente.ArquivoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class LinkCadastroClienteConfiguration : IEntityTypeConfiguration<LinkCadastroCliente>
{
    public void Configure(EntityTypeBuilder<LinkCadastroCliente> builder)
    {
        builder.ToTable("links_cadastro_cliente");
        builder.HasKey(link => link.Id);
        builder.Property(link => link.AccountId).HasMaxLength(450).IsRequired();
        builder.HasIndex(link => new { link.AccountId, link.EventoId }).IsUnique();
    }
}

public sealed class VinculoClienteConfiguration : IEntityTypeConfiguration<VinculoCliente>
{
    public void Configure(EntityTypeBuilder<VinculoCliente> builder)
    {
        builder.ToTable("vinculos_clientes");
        builder.HasKey(vinculo => vinculo.Id);
        builder.Property(vinculo => vinculo.AccountId).HasMaxLength(450).IsRequired();
        builder.HasIndex(vinculo => new { vinculo.AccountId, vinculo.ClienteId, vinculo.EventoId }).IsUnique();
        builder.HasOne<Cliente>()
            .WithMany()
            .HasForeignKey(vinculo => vinculo.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Evento>()
            .WithMany()
            .HasForeignKey(vinculo => vinculo.EventoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
