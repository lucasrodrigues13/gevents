namespace Project.Domain.Funil;

public enum SituacaoContratoFunil
{
    EmAndamento = 0,
    Assinado = 1,
    Cancelado = 2,
}

public sealed class ContratoFunil
{
    public Guid Id { get; set; }

    public string ContaId { get; set; } = "";

    public Guid OportunidadeId { get; set; }

    public string Conteudo { get; set; } = "";

    public SituacaoContratoFunil Situacao { get; set; }

    public DateTimeOffset CriadoEm { get; set; }

    public DateTimeOffset? EnviadoEm { get; set; }

    public DateTimeOffset? AssinadoEm { get; set; }

    public Guid? ModeloId { get; set; }
}
