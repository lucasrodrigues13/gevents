using Project.Application.Eventos;
using Project.Domain;
using Project.Domain.Clientes;

namespace Project.Application.Clientes;

public sealed class ClienteServico
{
    private readonly IEventoConsulta _eventos;
    private readonly IClienteConsulta _consulta;
    private readonly IClienteRepositorio _repositorio;

    public ClienteServico(IEventoConsulta eventos, IClienteConsulta consulta, IClienteRepositorio repositorio)
    {
        _eventos = eventos;
        _consulta = consulta;
        _repositorio = repositorio;
    }

    public Task<IReadOnlyList<ClienteItem>> ListarAsync(string accountId, string? nome, CancellationToken cancellationToken)
    {
        return _consulta.ListarAsync(accountId, nome, cancellationToken);
    }

    public async Task<ClienteItem?> ObterAsync(string accountId, Guid id, CancellationToken cancellationToken)
    {
        var clientes = await _consulta.ListarAsync(accountId, null, cancellationToken);
        return clientes.FirstOrDefault(cliente => cliente.Id == id);
    }

    public async Task<Guid> CriarAsync(string accountId, string? nome, string? telefone, string? email, CancellationToken cancellationToken)
    {
        var cliente = Cliente.Criar(accountId, nome, telefone, email);
        await _repositorio.AdicionarAsync(cliente, cancellationToken);
        await _repositorio.SalvarAsync(cancellationToken);
        return cliente.Id;
    }

    public async Task EditarAsync(string accountId, Guid id, string? nome, string? telefone, string? email, CancellationToken cancellationToken)
    {
        var cliente = await _consulta.ObterAsync(accountId, id, cancellationToken)
            ?? throw new RegraDeNegocioException("O cliente não está disponível.");
        cliente.Editar(nome, telefone, email);
        await _repositorio.SalvarAsync(cancellationToken);
    }

    public async Task VincularAsync(string accountId, Guid clienteId, Guid eventoId, CancellationToken cancellationToken)
    {
        if (await _eventos.ObterAsync(accountId, eventoId, cancellationToken) is null
            || await _consulta.ObterAsync(accountId, clienteId, cancellationToken) is null)
        {
            throw new RegraDeNegocioException("O cliente não está disponível.");
        }

        if (await _consulta.ExisteVinculoAsync(accountId, clienteId, eventoId, cancellationToken))
        {
            return;
        }

        await _repositorio.AdicionarAsync(VinculoCliente.Criar(accountId, clienteId, eventoId), cancellationToken);
        await _repositorio.SalvarAsync(cancellationToken);
    }

    public async Task DesvincularAsync(string accountId, Guid clienteId, Guid eventoId, CancellationToken cancellationToken)
    {
        var vinculo = await _consulta.ObterVinculoAsync(accountId, clienteId, eventoId, cancellationToken);
        if (vinculo is null)
        {
            return;
        }

        await _repositorio.RemoverAsync(vinculo, cancellationToken);
        await _repositorio.SalvarAsync(cancellationToken);
    }

    public Task<IReadOnlyList<ClienteResumo>> ListarDoEventoAsync(string accountId, Guid eventoId, CancellationToken cancellationToken)
    {
        return _consulta.ListarDoEventoAsync(accountId, eventoId, cancellationToken);
    }
}
