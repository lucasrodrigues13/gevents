namespace Project.Domain.Convidados;

public sealed class GrupoConvidados
{
    public Guid Id { get; set; }

    public string ContaId { get; set; } = "";

    public string Nome { get; set; } = "";
}

public sealed class Convite
{
    public Guid Id { get; set; }

    public Guid EventoId { get; set; }

    public string ContaId { get; set; } = "";

    public string Nome { get; set; } = "";

    public Guid? GrupoId { get; set; }

    public string? Observacao { get; set; }

    public Guid? PrincipalId { get; set; }

    public bool UsaTransfer { get; set; }

    public string? LocalOrigem { get; set; }

    public string Token { get; set; } = "";

    public DateTimeOffset? AcessadoEm { get; set; }

    public DateTimeOffset? RespondidoEm { get; set; }

    public List<Convidado> Convidados { get; set; } = [];
}

public sealed class Convidado
{
    public Guid Id { get; set; }

    public Guid ConviteId { get; set; }

    public string Nome { get; set; } = "";

    public string? Ddi { get; set; }

    public string? Celular { get; set; }

    public SituacaoConvidado Situacao { get; set; } = SituacaoConvidado.Pendente;

    public string? Mesa { get; set; }

    public GeneroConvidado? Genero { get; set; }

    public FaixaEtaria? Faixa { get; set; }

    public CustoConvidado? Custo { get; set; }

    public string? Rg { get; set; }

    public string? Cpf { get; set; }

    public string? Observacao { get; set; }
}
