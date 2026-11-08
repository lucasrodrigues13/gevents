using Project.Domain.Eventos;

namespace Project.Application.Eventos;

public sealed record FiltroEventos(
    SituacaoEvento Situacao,
    string? Nome,
    TipoEvento? Tipo,
    DateOnly? De,
    DateOnly? Ate,
    string? Tag);

public sealed record EventoResumo(Guid Id, string Nome, SituacaoEvento Situacao, DateOnly? Data, IReadOnlyList<string> Tags);

public sealed record IndicadoresEvento(
    int TarefasConcluidas,
    int ConvidadosPendentes,
    int ConvidadosConfirmados,
    int ConvidadosNaoComparecera);

public sealed record EventoDetalhe(
    Guid Id,
    string Nome,
    SituacaoEvento Situacao,
    DateOnly? Data,
    TimeOnly? Hora,
    TipoEvento? Tipo,
    IReadOnlyList<string> Tags,
    string? CasalNome,
    string? CasalPapel,
    string? CasalEmail,
    string? CasalCelular,
    decimal? OrcamentoEstimado,
    int? ConvidadosEstimados,
    string? LocalCerimonia,
    string? LocalRecepcao,
    Guid? ArquivoId,
    IndicadoresEvento Indicadores);

public sealed record EventoGravado(Guid Id, string? AvisoImagem);

public interface IEventoConsulta
{
    Task<IReadOnlyList<EventoResumo>> ListarAsync(string accountId, FiltroEventos filtro, CancellationToken cancellationToken);

    Task<Evento?> ObterAsync(string accountId, Guid id, CancellationToken cancellationToken);

    Task<IndicadoresEvento> ContarIndicadoresAsync(string accountId, Guid eventoId, CancellationToken cancellationToken);
}

public interface IEventoRepositorio
{
    Task AdicionarAsync(Evento evento, CancellationToken cancellationToken);

    Task SalvarAsync(CancellationToken cancellationToken);
}
