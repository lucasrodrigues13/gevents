namespace Project.Domain.Eventos;

public sealed class ModeloEvento
{
    public Guid Id { get; set; }

    public string ContaId { get; set; } = "";

    public string Nome { get; set; } = "";

    public TipoEvento Tipo { get; set; }

    public List<ServicoModelo> Servicos { get; set; } = [];

    public List<TarefaModelo> Tarefas { get; set; } = [];
}

public sealed class ServicoModelo
{
    public Guid Id { get; set; }

    public Guid ModeloId { get; set; }

    public string Nome { get; set; } = "";
}

public sealed class TarefaModelo
{
    public Guid Id { get; set; }

    public Guid ModeloId { get; set; }

    public string Titulo { get; set; } = "";
}
