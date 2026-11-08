using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Infrastructure.Arquivos;

namespace Project.Infrastructure.Persistence.Configurations;

public sealed class ArquivoMetadadoConfiguration : IEntityTypeConfiguration<ArquivoMetadado>
{
    public void Configure(EntityTypeBuilder<ArquivoMetadado> builder)
    {
        builder.ToTable("arquivos");
        builder.HasKey(arquivo => arquivo.Id);
        builder.Property(arquivo => arquivo.NomeOriginal).IsRequired();
        builder.Property(arquivo => arquivo.CaminhoRelativo).IsRequired();
        builder.Property(arquivo => arquivo.Mime).IsRequired();
        builder.Property(arquivo => arquivo.Categoria).IsRequired();
    }
}
