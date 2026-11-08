namespace Project.Application.Arquivos;

public interface IArmazenamentoArquivo
{
    Task<Guid> SalvarImagemAsync(Stream conteudo, string nomeOriginal, CancellationToken cancellationToken);

    Task<ArquivoBaixado?> AbrirAsync(Guid id, CancellationToken cancellationToken);

    Task ExcluirAsync(Guid id, CancellationToken cancellationToken);
}

public sealed record ArquivoBaixado(Stream Conteudo, string NomeOriginal, string Mime) : IAsyncDisposable
{
    public ValueTask DisposeAsync()
    {
        return Conteudo.DisposeAsync();
    }
}
