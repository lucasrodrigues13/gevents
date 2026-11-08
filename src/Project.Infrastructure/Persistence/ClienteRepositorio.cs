using Microsoft.EntityFrameworkCore;
using Project.Application.Clientes;
using Project.Domain.Clientes;

namespace Project.Infrastructure.Persistence;

public sealed class ClienteRepositorio : IClienteConsulta, IClienteRepositorio
{
    private readonly AppDbContext _db;

    public ClienteRepositorio(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ClienteItem>> ListarAsync(string accountId, string? nome, CancellationToken cancellationToken)
    {
        var consulta = _db.Clientes.AsNoTracking().Where(cliente => cliente.AccountId == accountId);
        if (!string.IsNullOrWhiteSpace(nome))
        {
            var texto = nome.Trim().Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
            consulta = consulta.Where(cliente => EF.Functions.ILike(cliente.Nome, "%" + texto + "%", "\\"));
        }

        var clientes = await consulta.OrderBy(cliente => cliente.Nome).ToListAsync(cancellationToken);
        var vinculos = await (
            from vinculo in _db.Vinculos.AsNoTracking()
            join evento in _db.Eventos.AsNoTracking() on vinculo.EventoId equals evento.Id
            where vinculo.AccountId == accountId && evento.AccountId == accountId
            select new { vinculo.ClienteId, evento.Id, evento.Nome }).ToListAsync(cancellationToken);

        return clientes.Select(cliente => new ClienteItem(
            cliente.Id,
            cliente.Nome,
            cliente.Telefone,
            cliente.Email,
            vinculos.Where(item => item.ClienteId == cliente.Id)
                .Select(item => new EventoVinculado(item.Id, item.Nome))
                .ToArray())).ToArray();
    }

    public Task<Cliente?> ObterAsync(string accountId, Guid id, CancellationToken cancellationToken)
    {
        return _db.Clientes.FirstOrDefaultAsync(cliente => cliente.Id == id && cliente.AccountId == accountId, cancellationToken);
    }

    public Task<bool> ExisteVinculoAsync(string accountId, Guid clienteId, Guid eventoId, CancellationToken cancellationToken)
    {
        return _db.Vinculos.AnyAsync(
            vinculo => vinculo.AccountId == accountId && vinculo.ClienteId == clienteId && vinculo.EventoId == eventoId,
            cancellationToken);
    }

    public async Task<IReadOnlyList<ClienteResumo>> ListarDoEventoAsync(string accountId, Guid eventoId, CancellationToken cancellationToken)
    {
        var consulta =
            from vinculo in _db.Vinculos.AsNoTracking()
            join cliente in _db.Clientes.AsNoTracking() on vinculo.ClienteId equals cliente.Id
            where vinculo.AccountId == accountId && vinculo.EventoId == eventoId && cliente.AccountId == accountId
            orderby cliente.Nome
            select new ClienteResumo(cliente.Id, cliente.Nome);
        return await consulta.ToListAsync(cancellationToken);
    }

    public Task<VinculoCliente?> ObterVinculoAsync(string accountId, Guid clienteId, Guid eventoId, CancellationToken cancellationToken)
    {
        return _db.Vinculos.FirstOrDefaultAsync(
            vinculo => vinculo.AccountId == accountId && vinculo.ClienteId == clienteId && vinculo.EventoId == eventoId,
            cancellationToken);
    }

    public Task AdicionarAsync(Cliente cliente, CancellationToken cancellationToken)
    {
        _db.Clientes.Add(cliente);
        return Task.CompletedTask;
    }

    public Task AdicionarAsync(VinculoCliente vinculo, CancellationToken cancellationToken)
    {
        _db.Vinculos.Add(vinculo);
        return Task.CompletedTask;
    }

    public Task RemoverAsync(VinculoCliente vinculo, CancellationToken cancellationToken)
    {
        _db.Vinculos.Remove(vinculo);
        return Task.CompletedTask;
    }

    public Task SalvarAsync(CancellationToken cancellationToken)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }
}
