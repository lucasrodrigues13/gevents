using Project.Domain.Convites;

namespace Project.Application.Convites;

public sealed record GrupoItem(Guid Id, string Nome);

public sealed record ConviteItem(
    Guid Id,
    string Nome,
    Guid Link,
    DateTime? AcessadaEm,
    DateTime? RespondidaEm,
    Guid? GrupoId = null,
    string? Observacao = null,
    Guid? ConvidadoPrincipalId = null,
    bool UsaTransfer = false,
    Guid? LocalTransferId = null,
    string? Telefone = null);

public sealed record LocalTransferItem(Guid Id, string Nome);

public sealed record ConvidadoItem(
    Guid Id,
    Guid ConviteId,
    string ConviteNome,
    string Nome,
    SituacaoConvidado Situacao,
    FaixaConvidado? Faixa,
    CustoConvidado? Custo,
    GeneroConvidado? Genero,
    string? Celular,
    string? Observacao,
    Guid? GrupoId,
    string? GrupoNome,
    bool Transfer,
    string? Origem,
    string? Ddi = null,
    string? Mesa = null,
    string? Rg = null,
    string? Cpf = null,
    string? TelefoneConvite = null,
    Guid? GrupoConviteId = null,
    string? ObservacaoConvite = null,
    bool TransferConvite = false);

public sealed record FiltroConvidados(
    string? Nome,
    SituacaoConvidado? Situacao,
    string? Celular,
    bool? Transfer,
    Guid? GrupoId,
    string? Observacao);

public sealed record PainelConvites(
    string EventoNome,
    bool Aberto,
    IReadOnlyList<GrupoItem> Grupos,
    IReadOnlyList<ConviteItem> Convites,
    IReadOnlyList<ConvidadoItem> Todos,
    IReadOnlyList<ConvidadoItem> Filtrados,
    bool TransferAtivo = false,
    IReadOnlyList<LocalTransferItem>? Locais = null);

public sealed record PresencaPublica(Guid Id, string Nome, SituacaoConvidado Situacao);

public sealed record ConvitePublico(string Nome, bool Respondido, IReadOnlyList<PresencaPublica> Convidados);

public interface IGrupoConsulta
{
    Task<IReadOnlyList<GrupoItem>> ListarAsync(string accountId, CancellationToken cancellationToken);

    Task<bool> PertenceAsync(string accountId, Guid grupoId, CancellationToken cancellationToken);
}

public interface IGrupoRepositorio
{
    Task AdicionarAsync(Grupo grupo, CancellationToken cancellationToken);

    Task SalvarAsync(CancellationToken cancellationToken);
}

public interface IConviteConsulta
{
    Task<Convite?> ObterAsync(string accountId, Guid id, CancellationToken cancellationToken);

    Task<Convite?> ObterPorLinkAsync(Guid link, CancellationToken cancellationToken);

    Task<IReadOnlyList<ConviteItem>> ListarAsync(string accountId, Guid eventoId, CancellationToken cancellationToken);

    Task<IReadOnlyList<ConvidadoItem>> ListarConvidadosAsync(string accountId, Guid eventoId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Convidado>> ListarConvidadosDoConviteAsync(Guid conviteId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Convite>> ListarDoEventoAsync(string accountId, Guid eventoId, CancellationToken cancellationToken);

    Task<Convidado?> ObterConvidadoAsync(string accountId, Guid convidadoId, CancellationToken cancellationToken);

    Task<IReadOnlyList<LocalTransferItem>> ListarLocaisAsync(string accountId, Guid eventoId, CancellationToken cancellationToken);

    Task<bool> LocalPertenceAsync(string accountId, Guid eventoId, Guid localId, CancellationToken cancellationToken);
}

public interface ILeitorPlanilha
{
    IReadOnlyList<IReadOnlyList<string>> Ler(Stream conteudo, string? nomeArquivo);

    byte[] Modelo();

    byte[] Criar(IReadOnlyList<IReadOnlyList<string>> linhas);
}

public interface ICodigoQr
{
    byte[] Png(string texto);
}

public interface IConviteRepositorio
{
    Task AdicionarAsync(Convite convite, CancellationToken cancellationToken);

    Task AdicionarAsync(Convidado convidado, CancellationToken cancellationToken);

    Task AdicionarAsync(LocalTransfer local, CancellationToken cancellationToken);

    Task SalvarAsync(CancellationToken cancellationToken);
}
