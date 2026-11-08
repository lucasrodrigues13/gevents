namespace Project.Application.Dashboards;

public sealed record EventoDoDashboard(Guid Id, string Nome, DateOnly? Data);

public sealed record DashboardEventos(
    int EmAndamento,
    int Concluidos,
    int Cancelados,
    IReadOnlyList<EventoDoDashboard> ListaEmAndamento);

public sealed record CartaoFunil(string Estagio, int Quantidade, decimal Valor);

public interface IDashboardConsulta
{
    Task<DashboardEventos> ObterEventosAsync(string accountId, CancellationToken cancellationToken);
}

public sealed class DashboardServico
{
    private readonly IDashboardConsulta _consulta;

    public DashboardServico(IDashboardConsulta consulta)
    {
        _consulta = consulta;
    }

    public Task<DashboardEventos> ObterEventosAsync(string accountId, CancellationToken cancellationToken)
    {
        return _consulta.ObterEventosAsync(accountId, cancellationToken);
    }

    public IReadOnlyList<CartaoFunil> ObterFunil()
    {
        // Oportunidade ainda não tem tabela. Sem linhas, quantidade e valor são zero.
        return
        [
            new CartaoFunil("Contato inicial", 0, 0),
            new CartaoFunil("Orçamento enviado", 0, 0),
            new CartaoFunil("Contratado", 0, 0),
            new CartaoFunil("Descartado", 0, 0)
        ];
    }
}
