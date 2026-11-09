using Microsoft.EntityFrameworkCore;
using Project.Application.Eventos;
using Project.Domain.Eventos;

namespace Project.Infrastructure.Persistence;

public sealed class ModeloRepositorio : IModeloConsulta, IModeloRepositorio
{
    private readonly AppDbContext _db;

    public ModeloRepositorio(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ModeloResumo>> ListarAsync(string accountId, CancellationToken cancellationToken)
    {
        var modelos = await _db.Modelos.AsNoTracking()
            .Where(modelo => modelo.AccountId == accountId)
            .OrderBy(modelo => modelo.Nome)
            .ToListAsync(cancellationToken);
        return modelos.Select(modelo => new ModeloResumo(modelo.Id, modelo.Nome, modelo.Tipo)).ToArray();
    }

    public Task<Modelo?> ObterAsync(string accountId, Guid id, CancellationToken cancellationToken)
    {
        return _db.Modelos
            .Include(modelo => modelo.Servicos)
            .ThenInclude(servico => servico.Tarefas)
            .FirstOrDefaultAsync(modelo => modelo.Id == id && modelo.AccountId == accountId, cancellationToken);
    }

    public Task AdicionarAsync(Modelo modelo, CancellationToken cancellationToken)
    {
        _db.Modelos.Add(modelo);
        return Task.CompletedTask;
    }

    public Task RegistrarServicoAsync(ServicoModelo servico, CancellationToken cancellationToken)
    {
        _db.ServicosModelo.Add(servico);
        return Task.CompletedTask;
    }

    public Task RegistrarTarefaAsync(TarefaModelo tarefa, CancellationToken cancellationToken)
    {
        _db.TarefasModelo.Add(tarefa);
        return Task.CompletedTask;
    }

    public Task SalvarAsync(CancellationToken cancellationToken)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }
}
