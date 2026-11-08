namespace Project.Domain.Convites;

public sealed class Convite
{
    private Convite()
    {
    }

    public Guid Id { get; private set; }

    public Guid EventoId { get; private set; }

    public string AccountId { get; private set; } = "";

    public string Nome { get; private set; } = "";

    public Guid Link { get; private set; }

    public DateTime? AcessadaEm { get; private set; }

    public DateTime? RespondidaEm { get; private set; }

    public static Convite Criar(string accountId, Guid eventoId, string? nome)
    {
        if (string.IsNullOrWhiteSpace(accountId) || eventoId == Guid.Empty)
        {
            throw new RegraDeNegocioException("O convite precisa de um evento da conta.");
        }

        return new Convite
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            EventoId = eventoId,
            Nome = Grupo.ExigirNome(nome, "Informe o nome do convite."),
            Link = Guid.NewGuid()
        };
    }

    public void RegistrarAcesso(DateTime momentoUtc)
    {
        if (RespondidaEm is null && AcessadaEm is null)
        {
            AcessadaEm = momentoUtc;
        }
    }

    public bool RespostaRegistrada => RespondidaEm is not null;

    public void RegistrarResposta(DateTime momentoUtc)
    {
        if (RespondidaEm is not null)
        {
            throw new RegraDeNegocioException("A resposta já foi registrada.");
        }

        RespondidaEm = momentoUtc;
    }
}

public sealed record ConvidadoDados(
    string? Nome,
    FaixaConvidado? Faixa,
    CustoConvidado? Custo,
    GeneroConvidado? Genero,
    string? Celular,
    string? Observacao,
    Guid? GrupoId,
    bool Transfer,
    string? Origem);
