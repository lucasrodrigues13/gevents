namespace Project.Application.Arquivos;

public sealed record ArquivoGuardado(Guid Id, string NomeOriginal, string TipoConteudo);

public sealed record ArquivoAberto(string NomeOriginal, string TipoConteudo, Stream Conteudo) : IDisposable
{
    public void Dispose() => Conteudo.Dispose();
}

public interface IArmazenamentoArquivo
{
    Task<Comum.Resultado<ArquivoGuardado>> GuardarImagemAsync(
        Stream conteudo,
        string? nomeOriginal,
        string contaId,
        CancellationToken ct);

    Task<ArquivoAberto?> AbrirAsync(Guid id, string contaId, CancellationToken ct);

    Task DescartarAsync(Guid id, CancellationToken ct);
}
