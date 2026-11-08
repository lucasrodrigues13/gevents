namespace Project.Infrastructure.Arquivos;

public interface IArquivoMetadadoRepositorio
{
    Task AdicionarAsync(ArquivoMetadado metadado, CancellationToken cancellationToken);

    Task<ArquivoMetadado?> ObterAsync(Guid id, CancellationToken cancellationToken);

    Task RemoverAsync(ArquivoMetadado metadado, CancellationToken cancellationToken);

    void Descartar(ArquivoMetadado metadado);
}
