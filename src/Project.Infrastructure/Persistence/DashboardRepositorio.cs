using Microsoft.EntityFrameworkCore;
using Project.Application.Dashboards;
using Project.Domain.Eventos;

namespace Project.Infrastructure.Persistence;

public sealed class DashboardRepositorio : IDashboardConsulta
{
    private readonly AppDbContext _db;

    public DashboardRepositorio(AppDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardEventos> ObterEventosAsync(string accountId, CancellationToken cancellationToken)
    {
        var contagens = await _db.Eventos.AsNoTracking()
            .Where(evento => evento.AccountId == accountId)
            .GroupBy(evento => evento.Situacao)
            .Select(grupo => new { Situacao = grupo.Key, Total = grupo.Count() })
            .ToListAsync(cancellationToken);
        var porSituacao = contagens.ToDictionary(item => item.Situacao, item => item.Total);
        var lista = await _db.Eventos.AsNoTracking()
            .Where(evento => evento.AccountId == accountId && evento.Situacao == SituacaoEvento.EmAndamento)
            .OrderBy(evento => evento.Nome)
            .Select(evento => new EventoDoDashboard(evento.Id, evento.Nome, evento.Data))
            .ToListAsync(cancellationToken);
        return new DashboardEventos(
            porSituacao.GetValueOrDefault(SituacaoEvento.EmAndamento),
            porSituacao.GetValueOrDefault(SituacaoEvento.Concluido),
            porSituacao.GetValueOrDefault(SituacaoEvento.Cancelado),
            lista);
    }
}
