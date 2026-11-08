namespace Project.Infrastructure.Arquivos;

public sealed class ArquivoMetadado
{
    public Guid Id { get; set; }

    public string NomeOriginal { get; set; } = "";

    public string CaminhoRelativo { get; set; } = "";

    public string Mime { get; set; } = "";

    public long Tamanho { get; set; }

    public string Categoria { get; set; } = "";

    public DateTime GravadoEmUtc { get; set; }
}
