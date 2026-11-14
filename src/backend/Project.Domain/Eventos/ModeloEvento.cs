namespace Project.Domain.Eventos;

public sealed class ModeloEvento
{
    public Guid Id { get; set; }

    public string ContaId { get; set; } = "";

    public string Nome { get; set; } = "";

    public TipoEvento Tipo { get; set; }

    public bool PermiteGerais { get; set; } = true;

    public bool PermitePlanejamento { get; set; } = true;

    public bool PermiteTarefas { get; set; } = true;

    public bool PermiteServicosFornecedores { get; set; } = true;

    public bool PermiteConvites { get; set; } = true;

    public List<ServicoModelo> Servicos { get; set; } = [];

    public List<TarefaModelo> Tarefas { get; set; } = [];

    public List<VinculoModeloEvento> Vinculos { get; set; } = [];
}

public sealed class ServicoModelo
{
    public Guid Id { get; set; }

    public Guid ModeloId { get; set; }

    public string Nome { get; set; } = "";

    public decimal? ValorEstimado { get; set; }

    public List<ItemServicoModelo> Itens { get; set; } = [];
}

public enum TipoItemServicoModelo
{
    Tarefa = 0,
    Fornecedor = 1,
    Alinhamento = 2,
    Comparativo = 3,
}

public sealed class ItemServicoModelo
{
    public Guid Id { get; set; }

    public Guid ServicoId { get; set; }

    public TipoItemServicoModelo Tipo { get; set; }

    public string Texto { get; set; } = "";
}

public sealed class TarefaModelo
{
    public Guid Id { get; set; }

    public Guid ModeloId { get; set; }

    public string Titulo { get; set; } = "";

    public int? DiasAntes { get; set; }
}

public enum TipoVinculoModelo
{
    Briefing = 0,
    Roteiro = 1,
    Inspiracao = 2,
    Mensagem = 3,
}

public sealed class VinculoModeloEvento
{
    public Guid Id { get; set; }

    public Guid ModeloId { get; set; }

    public TipoVinculoModelo Tipo { get; set; }

    public Guid BibliotecaId { get; set; }
}
