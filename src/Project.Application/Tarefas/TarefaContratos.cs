using Project.Domain.Tarefas;

namespace Project.Application.Tarefas;

public sealed record TarefaItem(
    Guid Id,
    Guid EventoId,
    string Titulo,
    string? Descricao,
    DateOnly? Prazo,
    SituacaoTarefa Situacao);

public sealed record TarefaGeral(
    Guid Id,
    Guid EventoId,
    string EventoNome,
    string Titulo,
    DateOnly? Prazo,
    SituacaoTarefa Situacao);

public interface ITarefaConsulta
{
    Task<IReadOnlyList<TarefaItem>> ListarPorEventoAsync(string accountId, Guid eventoId, CancellationToken cancellationToken);

    Task<IReadOnlyList<TarefaGeral>> ListarPorContaAsync(string accountId, CancellationToken cancellationToken);

    Task<Tarefa?> ObterAsync(string accountId, Guid id, CancellationToken cancellationToken);
}

public interface ITarefaRepositorio
{
    Task AdicionarAsync(Tarefa tarefa, CancellationToken cancellationToken);

    Task SalvarAsync(CancellationToken cancellationToken);
}
