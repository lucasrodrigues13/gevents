namespace Project.Domain.Convites;

public sealed class LocalTransfer
{
    private LocalTransfer()
    {
    }

    public Guid Id { get; private set; }

    public Guid EventoId { get; private set; }

    public string AccountId { get; private set; } = "";

    public string Nome { get; private set; } = "";

    public static LocalTransfer Criar(string accountId, Guid eventoId, string? nome)
    {
        if (string.IsNullOrWhiteSpace(accountId) || eventoId == Guid.Empty)
        {
            throw new RegraDeNegocioException("O local precisa de um evento da conta.");
        }

        return new LocalTransfer
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            EventoId = eventoId,
            Nome = Grupo.ExigirNome(nome, "Informe o nome do local.")
        };
    }
}
