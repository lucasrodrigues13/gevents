using Project.Application.Eventos;
using Project.Domain;
using Project.Domain.Eventos;
using Project.Domain.Tarefas;

namespace Project.Application.Tarefas;

public sealed class TarefaServico
{
    private readonly IEventoConsulta _eventos;
    private readonly ITarefaConsulta _consulta;
    private readonly ITarefaRepositorio _repositorio;

    public TarefaServico(IEventoConsulta eventos, ITarefaConsulta consulta, ITarefaRepositorio repositorio)
    {
        _eventos = eventos;
        _consulta = consulta;
        _repositorio = repositorio;
    }

    public async Task<IReadOnlyList<TarefaItem>?> ListarPorEventoAsync(string accountId, Guid eventoId, CancellationToken cancellationToken)
    {
        var evento = await _eventos.ObterAsync(accountId, eventoId, cancellationToken);
        if (evento is null)
        {
            return null;
        }

        return await _consulta.ListarPorEventoAsync(accountId, eventoId, cancellationToken);
    }

    public Task<IReadOnlyList<TarefaGeral>> ListarPorContaAsync(string accountId, CancellationToken cancellationToken)
    {
        return _consulta.ListarPorContaAsync(accountId, cancellationToken);
    }

    public async Task<TarefaItem?> ObterAsync(string accountId, Guid id, CancellationToken cancellationToken)
    {
        var tarefa = await _consulta.ObterAsync(accountId, id, cancellationToken);
        return tarefa is null ? null : ParaItem(tarefa);
    }

    public async Task<Guid> CriarAsync(
        string accountId,
        Guid eventoId,
        string? titulo,
        string? descricao,
        DateOnly? prazo,
        CancellationToken cancellationToken)
    {
        await ExigirEventoAbertoAsync(accountId, eventoId, cancellationToken);
        var tarefa = Tarefa.Criar(accountId, eventoId, titulo, descricao, prazo);
        await _repositorio.AdicionarAsync(tarefa, cancellationToken);
        await _repositorio.SalvarAsync(cancellationToken);
        return tarefa.Id;
    }

    public async Task SalvarAsync(
        string accountId,
        Guid id,
        string? titulo,
        string? descricao,
        DateOnly? prazo,
        SituacaoTarefa situacao,
        CancellationToken cancellationToken)
    {
        var tarefa = await _consulta.ObterAsync(accountId, id, cancellationToken)
            ?? throw new RegraDeNegocioException("A tarefa não está disponível.");
        await ExigirEventoAbertoAsync(accountId, tarefa.EventoId, cancellationToken);
        tarefa.Editar(titulo, descricao, prazo);
        tarefa.MudarSituacao(situacao);
        await _repositorio.SalvarAsync(cancellationToken);
    }

    private async Task ExigirEventoAbertoAsync(string accountId, Guid eventoId, CancellationToken cancellationToken)
    {
        var evento = await _eventos.ObterAsync(accountId, eventoId, cancellationToken)
            ?? throw new RegraDeNegocioException("O evento não está disponível.");
        if (evento.Situacao != SituacaoEvento.EmAndamento)
        {
            throw new RegraDeNegocioException("O evento não está em andamento.");
        }
    }

    private static TarefaItem ParaItem(Tarefa tarefa)
    {
        return new TarefaItem(tarefa.Id, tarefa.EventoId, tarefa.Titulo, tarefa.Descricao, tarefa.Prazo, tarefa.Situacao);
    }
}
