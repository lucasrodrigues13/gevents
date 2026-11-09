using Project.Domain.Clientes;

namespace Project.Application.Clientes;

public sealed record ClienteItem(Guid Id, string Nome, string? Telefone, string? Email, IReadOnlyList<EventoVinculado> Eventos);

public sealed record EventoVinculado(Guid Id, string Nome);

public sealed record ClienteResumo(Guid Id, string Nome);

public interface IClienteConsulta
{
    Task<IReadOnlyList<ClienteItem>> ListarAsync(string accountId, string? nome, CancellationToken cancellationToken);

    Task<Cliente?> ObterAsync(string accountId, Guid id, CancellationToken cancellationToken);

    Task<bool> ExisteVinculoAsync(string accountId, Guid clienteId, Guid eventoId, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClienteResumo>> ListarDoEventoAsync(string accountId, Guid eventoId, CancellationToken cancellationToken);

    Task<VinculoCliente?> ObterVinculoAsync(string accountId, Guid clienteId, Guid eventoId, CancellationToken cancellationToken);

    Task<LinkCadastroCliente?> ObterLinkAsync(string accountId, Guid eventoId, CancellationToken cancellationToken);

    Task<LinkCadastroCliente?> ObterLinkAsync(Guid id, CancellationToken cancellationToken);
}

public interface IClienteRepositorio
{
    Task AdicionarAsync(Cliente cliente, CancellationToken cancellationToken);

    Task AdicionarAsync(VinculoCliente vinculo, CancellationToken cancellationToken);

    Task AdicionarAsync(LinkCadastroCliente link, CancellationToken cancellationToken);

    Task RemoverAsync(VinculoCliente vinculo, CancellationToken cancellationToken);

    Task SalvarAsync(CancellationToken cancellationToken);
}
