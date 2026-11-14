namespace Project.Domain.Funil;

public enum SituacaoOrcamentoFunil
{
    EmAndamento = 0,
    Respondido = 1,
    Cancelado = 2,
}

public sealed class OrcamentoFunil
{
    public Guid Id { get; set; }

    public string ContaId { get; set; } = "";

    public Guid OportunidadeId { get; set; }

    public string Conteudo { get; set; } = "";

    public decimal? Valor { get; set; }

    public SituacaoOrcamentoFunil Situacao { get; set; }

    public DateTimeOffset CriadoEm { get; set; }

    public DateTimeOffset? EnviadoEm { get; set; }

    public DateTimeOffset? RespondidoEm { get; set; }

    public Guid? ModeloId { get; set; }
}
