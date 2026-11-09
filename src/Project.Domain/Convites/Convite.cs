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

    public Guid? GrupoId { get; private set; }

    public string? Observacao { get; private set; }

    public Guid? ConvidadoPrincipalId { get; private set; }

    public bool UsaTransfer { get; private set; }

    public Guid? LocalTransferId { get; private set; }

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

    public static (Convite Convite, Convidado Convidado) CriarComConvidado(
        string accountId,
        Guid eventoId,
        string? nome,
        ConvidadoDados dados)
    {
        var convite = Criar(accountId, eventoId, nome);
        var convidado = Convidado.Criar(convite.Id, dados);
        return (convite, convidado);
    }

    public void DefinirGrupo(Guid? grupoId)
    {
        GrupoId = grupoId;
    }

    public void DefinirObservacao(string? texto)
    {
        Observacao = string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
    }

    public void DefinirPrincipal(Convidado convidado)
    {
        if (convidado.ConviteId != Id)
        {
            throw new RegraDeNegocioException("O principal precisa ser deste convite.");
        }

        if (string.IsNullOrWhiteSpace(convidado.Celular))
        {
            throw new RegraDeNegocioException("O principal precisa ter telefone.");
        }

        ConvidadoPrincipalId = convidado.Id;
    }

    public void LimparPrincipal()
    {
        ConvidadoPrincipalId = null;
    }

    public void DefinirTransfer(bool eventoLigado, bool usa, Guid? localId)
    {
        if (!eventoLigado || !usa)
        {
            UsaTransfer = false;
            LocalTransferId = null;
            return;
        }

        if (localId is null || localId == Guid.Empty)
        {
            throw new RegraDeNegocioException("Informe o local de origem do transfer.");
        }

        UsaTransfer = true;
        LocalTransferId = localId;
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
    string? Origem,
    string? Ddi = null,
    string? Mesa = null,
    string? Rg = null,
    string? Cpf = null,
    SituacaoConvidado? Situacao = null);
