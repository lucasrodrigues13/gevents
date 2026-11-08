using Microsoft.EntityFrameworkCore;
using Project.Application.Perfis;
using Project.Domain.Perfis;

namespace Project.Infrastructure.Persistence;

public sealed class PerfilRepositorio : IPerfilConsulta, IPerfilRepositorio
{
    private readonly AppDbContext _db;

    public PerfilRepositorio(AppDbContext db)
    {
        _db = db;
    }

    public Task<Perfil?> ObterPorContaAsync(string accountId, CancellationToken cancellationToken)
    {
        return _db.Perfis.FirstOrDefaultAsync(perfil => perfil.AccountId == accountId, cancellationToken);
    }

    public Task AdicionarAsync(Perfil perfil, CancellationToken cancellationToken)
    {
        _db.Perfis.Add(perfil);
        return Task.CompletedTask;
    }

    public Task SalvarAsync(CancellationToken cancellationToken)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }
}
