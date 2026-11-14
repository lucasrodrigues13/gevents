namespace Project.Domain.Funil;

public enum TipoTarefaFunil
{
    Reuniao = 0,
    Ligacao = 1,
    Email = 2,
    Tarefa = 3,
}

public sealed class TarefaFunil
{
    public Guid Id { get; set; }

    public string ContaId { get; set; } = "";

    public Guid OportunidadeId { get; set; }

    public string Titulo { get; set; } = "";

    public string? Observacao { get; set; }

    public TipoTarefaFunil Tipo { get; set; }

    public string Responsavel { get; set; } = "";

    public DateOnly Prazo { get; set; }

    public bool Completa { get; set; }

    public Guid? AnexoArquivoId { get; set; }
}
