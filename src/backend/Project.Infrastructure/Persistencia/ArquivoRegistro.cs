namespace Project.Infrastructure.Persistencia;

public sealed class ArquivoRegistro
{
    public Guid Id { get; set; }

    public string ContaId { get; set; } = "";

    public string NomeOriginal { get; set; } = "";

    public string TipoConteudo { get; set; } = "";

    public long Tamanho { get; set; }

    public string CaminhoRelativo { get; set; } = "";
}
