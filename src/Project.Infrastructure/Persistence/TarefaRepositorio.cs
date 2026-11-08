using Microsoft.EntityFrameworkCore;
using Project.Application.Tarefas;
using Project.Domain.Tarefas;

namespace Project.Infrastructure.Persistence;

public sealed class TarefaRepositorio : ITarefaConsulta, ITarefaRepositorio
{
    private readonly AppDbContext _db;

    public TarefaRepositorio(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<TarefaItem>> ListarPorEventoAsync(string accountId, Guid eventoId, CancellationToken cancellationToken)
    {
        var tarefas = await _db.Tarefas.AsNoTracking()
            .Where(tarefa => tarefa.AccountId == accountId && tarefa.EventoId == eventoId)
            .OrderBy(tarefa => tarefa.Titulo)
            .ToListAsync(cancellationToken);
        return tarefas.Select(ParaItem).ToArray();
    }

    public async Task<IReadOnlyList<TarefaGeral>> ListarPorContaAsync(string accountId, CancellationToken cancellationToken)
    {
        var consulta =
            from tarefa in _db.Tarefas.AsNoTracking()
            join evento in _db.Eventos.AsNoTracking() on tarefa.EventoId equals evento.Id
            where tarefa.AccountId == accountId && evento.AccountId == accountId
            orderby evento.Nome, tarefa.Titulo
            select new TarefaGeral(tarefa.Id, tarefa.EventoId, evento.Nome, tarefa.Titulo, tarefa.Prazo, tarefa.Situacao);
        return await consulta.ToListAsync(cancellationToken);
    }

    public Task<Tarefa?> ObterAsync(string accountId, Guid id, CancellationToken cancellationToken)
    {
        return _db.Tarefas.FirstOrDefaultAsync(tarefa => tarefa.Id == id && tarefa.AccountId == accountId, cancellationToken);
    }

    public Task AdicionarAsync(Tarefa tarefa, CancellationToken cancellationToken)
    {
        _db.Tarefas.Add(tarefa);
        return Task.CompletedTask;
    }

    public Task SalvarAsync(CancellationToken cancellationToken)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }

    private static TarefaItem ParaItem(Tarefa tarefa)
    {
        return new TarefaItem(tarefa.Id, tarefa.EventoId, tarefa.Titulo, tarefa.Descricao, tarefa.Prazo, tarefa.Situacao);
    }
}
