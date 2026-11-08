using Microsoft.EntityFrameworkCore;
using Project.Application.Convites;
using Project.Domain.Convites;

namespace Project.Infrastructure.Persistence;

public sealed class GrupoRepositorio : IGrupoConsulta, IGrupoRepositorio
{
    private readonly AppDbContext _db;

    public GrupoRepositorio(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<GrupoItem>> ListarAsync(string accountId, CancellationToken cancellationToken)
    {
        return await _db.Grupos.AsNoTracking()
            .Where(grupo => grupo.AccountId == accountId)
            .OrderBy(grupo => grupo.Nome)
            .Select(grupo => new GrupoItem(grupo.Id, grupo.Nome))
            .ToListAsync(cancellationToken);
    }

    public Task<bool> PertenceAsync(string accountId, Guid grupoId, CancellationToken cancellationToken)
    {
        return _db.Grupos.AnyAsync(grupo => grupo.Id == grupoId && grupo.AccountId == accountId, cancellationToken);
    }

    public Task AdicionarAsync(Grupo grupo, CancellationToken cancellationToken)
    {
        _db.Grupos.Add(grupo);
        return Task.CompletedTask;
    }

    public Task SalvarAsync(CancellationToken cancellationToken)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }
}

public sealed class ConviteRepositorio : IConviteConsulta, IConviteRepositorio
{
    private readonly AppDbContext _db;

    public ConviteRepositorio(AppDbContext db)
    {
        _db = db;
    }

    public Task<Convite?> ObterAsync(string accountId, Guid id, CancellationToken cancellationToken)
    {
        return _db.Convites.FirstOrDefaultAsync(convite => convite.Id == id && convite.AccountId == accountId, cancellationToken);
    }

    public Task<Convite?> ObterPorLinkAsync(Guid link, CancellationToken cancellationToken)
    {
        return _db.Convites.FirstOrDefaultAsync(convite => convite.Link == link, cancellationToken);
    }

    public async Task<IReadOnlyList<ConviteItem>> ListarAsync(string accountId, Guid eventoId, CancellationToken cancellationToken)
    {
        return await _db.Convites.AsNoTracking()
            .Where(convite => convite.AccountId == accountId && convite.EventoId == eventoId)
            .OrderBy(convite => convite.Nome)
            .Select(convite => new ConviteItem(convite.Id, convite.Nome, convite.Link, convite.AcessadaEm, convite.RespondidaEm))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ConvidadoItem>> ListarConvidadosAsync(string accountId, Guid eventoId, CancellationToken cancellationToken)
    {
        var consulta =
            from convidado in _db.Convidados.AsNoTracking()
            join convite in _db.Convites.AsNoTracking() on convidado.ConviteId equals convite.Id
            join grupo in _db.Grupos.AsNoTracking() on convidado.GrupoId equals grupo.Id into grupos
            from grupo in grupos.DefaultIfEmpty()
            where convite.AccountId == accountId && convite.EventoId == eventoId
            orderby convite.Nome, convidado.Nome
            select new ConvidadoItem(
                convidado.Id,
                convite.Id,
                convite.Nome,
                convidado.Nome,
                convidado.Situacao,
                convidado.Faixa,
                convidado.Custo,
                convidado.Genero,
                convidado.Celular,
                convidado.Observacao,
                convidado.GrupoId,
                grupo != null ? grupo.Nome : null,
                convidado.Transfer,
                convidado.Origem);
        return await consulta.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Convidado>> ListarConvidadosDoConviteAsync(Guid conviteId, CancellationToken cancellationToken)
    {
        return await _db.Convidados
            .Where(convidado => convidado.ConviteId == conviteId)
            .OrderBy(convidado => convidado.Nome)
            .ToListAsync(cancellationToken);
    }

    public Task AdicionarAsync(Convite convite, CancellationToken cancellationToken)
    {
        _db.Convites.Add(convite);
        return Task.CompletedTask;
    }

    public Task AdicionarAsync(Convidado convidado, CancellationToken cancellationToken)
    {
        _db.Convidados.Add(convidado);
        return Task.CompletedTask;
    }

    public Task SalvarAsync(CancellationToken cancellationToken)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }
}
