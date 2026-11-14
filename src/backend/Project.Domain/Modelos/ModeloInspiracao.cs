namespace Project.Domain.Modelos;

public sealed class ModeloInspiracao
{
    public Guid Id { get; set; }

    public string ContaId { get; set; } = "";

    public string Nome { get; set; } = "";

    public List<ItemInspiracao> Itens { get; set; } = [];
}

public sealed class ItemInspiracao
{
    public Guid Id { get; set; }

    public Guid GaleriaId { get; set; }

    public Guid? ArquivoId { get; set; }

    public string? Url { get; set; }

    public int Ordem { get; set; }
}
