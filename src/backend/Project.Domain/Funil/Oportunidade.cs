using Project.Domain.Eventos;

namespace Project.Domain.Funil;

public sealed class Oportunidade
{
    public Guid Id { get; set; }

    public string ContaId { get; set; } = "";

    public string Nome { get; set; } = "";

    public EstagioOportunidade Estagio { get; set; }

    public string? Fonte { get; set; }

    public decimal? ValorProposta { get; set; }

    public string Responsavel { get; set; } = "";

    public string? ServicoProduto { get; set; }

    public Guid? ModeloServicoFunilId { get; set; }

    public TipoEvento? TipoEvento { get; set; }

    public int? QuantidadeConvidados { get; set; }

    public DateOnly? DataPrevista { get; set; }

    public string? Horarios { get; set; }

    public int? QuantidadePadrinhos { get; set; }

    public int? QuantidadeDamas { get; set; }

    public string? LocalCerimonia { get; set; }

    public string? EnderecoCerimonia { get; set; }

    public string? LocalRecepcao { get; set; }

    public string? EnderecoRecepcao { get; set; }

    public string? Uf { get; set; }

    public string? Cidade { get; set; }

    public string? MotivoDescarte { get; set; }

    public DateTimeOffset CriadoEm { get; set; }

    public DateTimeOffset EntradaNoEstagioEm { get; set; }

    public List<ContatoOportunidade> Contatos { get; set; } = [];
}

public sealed class ContatoOportunidade
{
    public Guid Id { get; set; }

    public Guid OportunidadeId { get; set; }

    public string Nome { get; set; } = "";

    public string? Telefone { get; set; }

    public string? Email { get; set; }
}
