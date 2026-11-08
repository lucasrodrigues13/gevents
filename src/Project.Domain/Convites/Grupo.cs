namespace Project.Domain.Convites;

public sealed class Grupo
{
    private Grupo()
    {
    }

    public Guid Id { get; private set; }

    public string AccountId { get; private set; } = "";

    public string Nome { get; private set; } = "";

    public static Grupo Criar(string accountId, string? nome)
    {
        if (string.IsNullOrWhiteSpace(accountId))
        {
            throw new RegraDeNegocioException("O grupo precisa de uma conta.");
        }

        return new Grupo
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            Nome = ExigirNome(nome, "Informe o nome do grupo.")
        };
    }

    public static string ExigirNome(string? nome, string mensagem)
    {
        var texto = nome?.Trim() ?? "";
        if (texto.Length == 0)
        {
            throw new RegraDeNegocioException(mensagem);
        }

        return texto;
    }
}
