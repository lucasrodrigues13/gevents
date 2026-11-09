namespace Project.Domain.Eventos;

public sealed class Modelo
{
    private readonly List<ServicoModelo> _servicos = [];

    private Modelo()
    {
    }

    public Guid Id { get; private set; }

    public string AccountId { get; private set; } = "";

    public string Nome { get; private set; } = "";

    public TipoEvento Tipo { get; private set; }

    public IReadOnlyList<ServicoModelo> Servicos => _servicos;

    public static Modelo Criar(string accountId, string? nome, TipoEvento tipo)
    {
        if (string.IsNullOrWhiteSpace(accountId))
        {
            throw new RegraDeNegocioException("O modelo precisa de uma conta.");
        }

        if (!Enum.IsDefined(tipo))
        {
            throw new RegraDeNegocioException("O tipo do modelo não é aceito.");
        }

        return new Modelo
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            Nome = ExigirNome(nome, "Informe o nome do modelo."),
            Tipo = tipo
        };
    }

    public ServicoModelo AdicionarServico(string? nome)
    {
        var servico = ServicoModelo.Criar(Id, nome);
        _servicos.Add(servico);
        return servico;
    }

    public static string ExigirNome(string? nome, string mensagem)
    {
        var texto = nome?.Trim() ?? "";
        if (texto.Length == 0)
        {
            throw new RegraDeNegocioException(mensagem);
        }

        return texto;
    }
}

public sealed class ServicoModelo
{
    private readonly List<TarefaModelo> _tarefas = [];

    private ServicoModelo()
    {
    }

    public Guid Id { get; private set; }

    public Guid ModeloId { get; private set; }

    public string Nome { get; private set; } = "";

    public IReadOnlyList<TarefaModelo> Tarefas => _tarefas;

    public static ServicoModelo Criar(Guid modeloId, string? nome)
    {
        if (modeloId == Guid.Empty)
        {
            throw new RegraDeNegocioException("O serviço precisa de um modelo.");
        }

        return new ServicoModelo
        {
            Id = Guid.NewGuid(),
            ModeloId = modeloId,
            Nome = Modelo.ExigirNome(nome, "Informe o nome do serviço.")
        };
    }

    public TarefaModelo AdicionarTarefa(string? titulo)
    {
        var tarefa = TarefaModelo.Criar(Id, titulo);
        _tarefas.Add(tarefa);
        return tarefa;
    }
}

public sealed class TarefaModelo
{
    private TarefaModelo()
    {
    }

    public Guid Id { get; private set; }

    public Guid ServicoModeloId { get; private set; }

    public string Titulo { get; private set; } = "";

    public static TarefaModelo Criar(Guid servicoModeloId, string? titulo)
    {
        if (servicoModeloId == Guid.Empty)
        {
            throw new RegraDeNegocioException("A tarefa precisa de um serviço.");
        }

        return new TarefaModelo
        {
            Id = Guid.NewGuid(),
            ServicoModeloId = servicoModeloId,
            Titulo = Modelo.ExigirNome(titulo, "Informe o título da tarefa.")
        };
    }
}
