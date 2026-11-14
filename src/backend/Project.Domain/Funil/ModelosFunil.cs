namespace Project.Domain.Funil;

public sealed class ModeloServicoFunil
{
    public Guid Id { get; set; }

    public string ContaId { get; set; } = "";

    public string Nome { get; set; } = "";

    public string? Descricao { get; set; }

    public decimal? Valor { get; set; }
}

public sealed class ModeloOrcamentoFunil
{
    public Guid Id { get; set; }

    public string ContaId { get; set; } = "";

    public string Nome { get; set; } = "";

    public string Texto { get; set; } = "";

    public string? CorPrimaria { get; set; }

    public string? CorSecundaria { get; set; }
}

public sealed class ModeloContratoFunil
{
    public Guid Id { get; set; }

    public string ContaId { get; set; } = "";

    public string Nome { get; set; } = "";

    public string Texto { get; set; } = "";
}
