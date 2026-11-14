namespace Project.Domain.Modelos;

public sealed class ModeloCronograma
{
    public Guid Id { get; set; }

    public string ContaId { get; set; } = "";

    public string Titulo { get; set; } = "";

    public string? Descricao { get; set; }

    public List<ItemCronograma> Itens { get; set; } = [];
}

public sealed class ItemCronograma
{
    public Guid Id { get; set; }

    public Guid ModeloId { get; set; }

    public string? Rotulo { get; set; }

    public DateOnly? Data { get; set; }

    public TimeOnly? Hora { get; set; }

    public string? Responsavel { get; set; }

    public string? RecadoFornecedores { get; set; }

    public int Ordem { get; set; }
}
