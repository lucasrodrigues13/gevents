using Microsoft.EntityFrameworkCore;
using Project.Application.Eventos;
using Project.Domain.Convites;
using Project.Domain.Eventos;
using Project.Domain.Tarefas;

namespace Project.Infrastructure.Persistence;

public sealed class EventoRepositorio : IEventoConsulta, IEventoRepositorio
{
    private readonly AppDbContext _db;

    public EventoRepositorio(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<EventoResumo>> ListarAsync(string accountId, FiltroEventos filtro, CancellationToken cancellationToken)
    {
        var consulta = _db.Eventos.AsNoTracking().Where(evento => evento.AccountId == accountId && evento.Situacao == filtro.Situacao);

        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            var nome = EscapeLike(filtro.Nome.Trim());
            consulta = consulta.Where(evento => EF.Functions.ILike(evento.Nome, "%" + nome + "%", "\\"));
        }

        if (filtro.Tipo is not null)
        {
            consulta = consulta.Where(evento => evento.Tipo == filtro.Tipo);
        }

        if (filtro.De is not null || filtro.Ate is not null)
        {
            consulta = consulta.Where(evento => evento.Data != null);
            if (filtro.De is not null)
            {
                consulta = consulta.Where(evento => evento.Data >= filtro.De);
            }

            if (filtro.Ate is not null)
            {
                consulta = consulta.Where(evento => evento.Data <= filtro.Ate);
            }
        }

        if (!string.IsNullOrWhiteSpace(filtro.Tag))
        {
            var tag = filtro.Tag.Trim().ToLower();
            consulta = consulta.Where(evento => evento.Tags.Any(item => item.ToLower() == tag));
        }

        var eventos = await consulta.OrderBy(evento => evento.Data).ThenBy(evento => evento.Nome).ToListAsync(cancellationToken);
        return eventos
            .Select(evento => new EventoResumo(evento.Id, evento.Nome, evento.Situacao, evento.Data, evento.Tags.ToArray()))
            .ToArray();
    }

    public Task<Evento?> ObterAsync(string accountId, Guid id, CancellationToken cancellationToken)
    {
        return _db.Eventos.FirstOrDefaultAsync(evento => evento.Id == id && evento.AccountId == accountId, cancellationToken);
    }

    public async Task<IndicadoresEvento> ContarIndicadoresAsync(string accountId, Guid eventoId, CancellationToken cancellationToken)
    {
        var concluidas = await _db.Tarefas.CountAsync(
            tarefa => tarefa.AccountId == accountId && tarefa.EventoId == eventoId && tarefa.Situacao == SituacaoTarefa.Concluida,
            cancellationToken);
        var presencas = await (
            from convidado in _db.Convidados
            join convite in _db.Convites on convidado.ConviteId equals convite.Id
            where convite.AccountId == accountId && convite.EventoId == eventoId
            group convidado by convidado.Situacao
            into grupo
            select new { Situacao = grupo.Key, Total = grupo.Count() }).ToListAsync(cancellationToken);
        var porSituacao = presencas.ToDictionary(item => item.Situacao, item => item.Total);
        return new IndicadoresEvento(
            concluidas,
            porSituacao.GetValueOrDefault(SituacaoConvidado.Pendente),
            porSituacao.GetValueOrDefault(SituacaoConvidado.Confirmado),
            porSituacao.GetValueOrDefault(SituacaoConvidado.NaoComparecera));
    }

    public async Task<IReadOnlyList<string>> ListarCopiasAsync(string accountId, Guid eventoId, CancellationToken cancellationToken)
    {
        var servicos = await _db.ServicosEvento.AsNoTracking()
            .Where(servico => servico.AccountId == accountId && servico.EventoId == eventoId)
            .Select(servico => servico.Nome)
            .ToListAsync(cancellationToken);
        var tarefas = await _db.Tarefas.AsNoTracking()
            .Where(tarefa => tarefa.AccountId == accountId && tarefa.EventoId == eventoId && tarefa.ServicoEventoId != null)
            .Select(tarefa => tarefa.Titulo)
            .ToListAsync(cancellationToken);
        return servicos.Concat(tarefas).ToArray();
    }

    public Task<Compromisso?> ObterCompromissoAsync(string accountId, Guid eventoId, CancellationToken cancellationToken)
    {
        return _db.Compromissos.AsNoTracking()
            .FirstOrDefaultAsync(compromisso => compromisso.AccountId == accountId && compromisso.EventoId == eventoId, cancellationToken);
    }

    public Task AdicionarAsync(Evento evento, CancellationToken cancellationToken)
    {
        _db.Eventos.Add(evento);
        return Task.CompletedTask;
    }

    public Task AdicionarCopiaAsync(
        IReadOnlyList<ServicoEvento> servicos,
        IReadOnlyList<Tarefa> tarefas,
        Compromisso? compromisso,
        CancellationToken cancellationToken)
    {
        if (servicos.Count > 0)
        {
            _db.ServicosEvento.AddRange(servicos);
        }

        if (tarefas.Count > 0)
        {
            _db.Tarefas.AddRange(tarefas);
        }

        if (compromisso is not null)
        {
            _db.Compromissos.Add(compromisso);
        }

        return Task.CompletedTask;
    }

    public Task<bool> TemAssessoriaAsync(string accountId, Guid eventoId, CancellationToken cancellationToken)
    {
        return _db.ServicosEvento.AnyAsync(
            servico => servico.AccountId == accountId && servico.EventoId == eventoId && servico.Nome == "Assessoria e Organização",
            cancellationToken);
    }

    public Task AdicionarAssessoriaAsync(ServicoEvento servico, IReadOnlyList<ParcelaEvento> parcelas, CancellationToken cancellationToken)
    {
        _db.ServicosEvento.Add(servico);
        _db.ParcelasEvento.AddRange(parcelas);
        return Task.CompletedTask;
    }

    public Task SalvarAsync(CancellationToken cancellationToken)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }

    private static string EscapeLike(string valor)
    {
        return valor.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("%", "\\%", StringComparison.Ordinal)
            .Replace("_", "\\_", StringComparison.Ordinal);
    }
}
