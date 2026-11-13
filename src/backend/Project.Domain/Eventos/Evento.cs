namespace Project.Domain.Eventos;

public sealed class Evento
{
    public Guid Id { get; set; }

    public string ContaId { get; set; } = "";

    public Guid ModeloId { get; set; }

    public string Nome { get; set; } = "";

    public string? PrimeiroNome { get; set; }

    public string? SegundoNome { get; set; }

    public TipoEvento Tipo { get; set; }

    public SituacaoEvento Situacao { get; set; } = SituacaoEvento.EmAndamento;

    public DateOnly? Data { get; set; }

    public TimeOnly? Hora { get; set; }

    public decimal? OrcamentoEstimado { get; set; }

    public int? ConvidadosEstimados { get; set; }

    public TimeOnly? TempoFesta { get; set; }

    public TimeOnly? InicioCerimonia { get; set; }

    public TimeOnly? InicioRecepcao { get; set; }

    public int? QuantidadePadrinhos { get; set; }

    public int? QuantidadeDamas { get; set; }

    public string? SenhaSite { get; set; }

    public bool NotificarAssessor { get; set; } = true;

    public bool NotificarAssistentes { get; set; } = true;

    public bool NotificarClientes { get; set; } = true;

    public bool SalvarNaAgenda { get; set; }

    public Guid? ImagemId { get; set; }

    public bool TransferAtivo { get; set; }

    public LocalEvento Cerimonia { get; set; } = new();

    public LocalEvento Recepcao { get; set; } = new();

    public List<TagConta> Tags { get; set; } = [];

    public List<ServicoEvento> Servicos { get; set; } = [];

    public List<LocalTransfer> LocaisTransfer { get; set; } = [];
}

public sealed class LocalEvento
{
    public string? Nome { get; set; }

    public string? Pais { get; set; }

    public string? Cep { get; set; }

    public string? Endereco { get; set; }

    public string? Numero { get; set; }

    public string? Estado { get; set; }

    public string? Cidade { get; set; }

    public string? Bairro { get; set; }
}

public sealed class TagConta
{
    public Guid Id { get; set; }

    public string ContaId { get; set; } = "";

    public string Nome { get; set; } = "";

    public List<Evento> Eventos { get; set; } = [];
}

public sealed class ServicoEvento
{
    public Guid Id { get; set; }

    public Guid EventoId { get; set; }

    public string Nome { get; set; } = "";

    public string? FornecedorContaId { get; set; }

    public bool Contratado { get; set; }

    public decimal? ValorContratado { get; set; }

    public List<ParcelaServico> Parcelas { get; set; } = [];
}

public sealed class ParcelaServico
{
    public Guid Id { get; set; }

    public Guid ServicoId { get; set; }

    public int Numero { get; set; }

    public DateOnly Vencimento { get; set; }

    public decimal Valor { get; set; }

    public bool Entrada { get; set; }
}

public sealed class LocalTransfer
{
    public Guid Id { get; set; }

    public Guid EventoId { get; set; }

    public string Nome { get; set; } = "";
}

public sealed class CompromissoAgenda
{
    public Guid Id { get; set; }

    public string ContaId { get; set; } = "";

    public Guid EventoId { get; set; }

    public DateOnly? Data { get; set; }

    public TimeOnly? Hora { get; set; }
}
