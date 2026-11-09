namespace Project.Domain.Clientes;

public sealed class LinkCadastroCliente
{
    private LinkCadastroCliente()
    {
    }

    public Guid Id { get; private set; }

    public string AccountId { get; private set; } = "";

    public Guid EventoId { get; private set; }

    public static LinkCadastroCliente Criar(string accountId, Guid eventoId)
    {
        if (string.IsNullOrWhiteSpace(accountId) || eventoId == Guid.Empty)
        {
            throw new RegraDeNegocioException("O link precisa de um evento da conta.");
        }

        return new LinkCadastroCliente
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            EventoId = eventoId
        };
    }
}
