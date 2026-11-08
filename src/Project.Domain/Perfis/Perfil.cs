namespace Project.Domain.Perfis;

public sealed class Perfil
{
    private Perfil()
    {
    }

    public Guid Id { get; private set; }

    public string AccountId { get; private set; } = "";

    public string? NomeEmpresa { get; private set; }

    public string? Responsavel { get; private set; }

    public string? Documento { get; private set; }

    public string? Celular { get; private set; }

    public DateOnly? Nascimento { get; private set; }

    public string? Endereco { get; private set; }

    public static Perfil Criar(string accountId)
    {
        if (string.IsNullOrWhiteSpace(accountId))
        {
            throw new RegraDeNegocioException("A conta do perfil é obrigatória.");
        }

        return new Perfil
        {
            Id = Guid.NewGuid(),
            AccountId = accountId
        };
    }

    public void Atualizar(
        string? nomeEmpresa,
        string? responsavel,
        string? documento,
        string? celular,
        DateOnly? nascimento,
        string? endereco,
        DateOnly hoje)
    {
        if (nascimento is not null && nascimento.Value > hoje)
        {
            throw new RegraDeNegocioException("A data de nascimento não pode ser futura.");
        }

        NomeEmpresa = EmptyToNull(nomeEmpresa);
        Responsavel = EmptyToNull(responsavel);
        Documento = EmptyToNull(documento);
        Celular = EmptyToNull(celular);
        Nascimento = nascimento;
        Endereco = EmptyToNull(endereco);
    }

    private static string? EmptyToNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
