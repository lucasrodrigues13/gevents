namespace Project.Domain.Fornecedores;

public sealed class Fornecedor
{
    public Guid Id { get; set; }

    public string ContaId { get; set; } = "";

    public string Nome { get; set; } = "";

    public Guid? FotoId { get; set; }

    public string? RazaoSocial { get; set; }

    public string? Cnpj { get; set; }

    public string? Cpf { get; set; }

    public string? NomeResponsavel { get; set; }

    public decimal? PrecoDe { get; set; }

    public decimal? PrecoAte { get; set; }

    public string? Site { get; set; }

    public string? Facebook { get; set; }

    public string? Instagram { get; set; }

    public string? Youtube { get; set; }

    public string? Pinterest { get; set; }

    public string? Pais { get; set; }

    public string? Cep { get; set; }

    public string? Endereco { get; set; }

    public string? Numero { get; set; }

    public string? Estado { get; set; }

    public string? Cidade { get; set; }

    public string? Bairro { get; set; }

    public string? Complemento { get; set; }

    public string? Banco { get; set; }

    public string? Agencia { get; set; }

    public string? ContaBancaria { get; set; }

    public string? TitularConta { get; set; }

    public string? TipoConta { get; set; }

    public string? DocumentosBancarios { get; set; }

    public string? PixTipo { get; set; }

    public string? PixChave { get; set; }

    public string? Diferencial { get; set; }

    public bool DiferencialVisivelCliente { get; set; }

    public string? InformacoesGerais { get; set; }

    public string? Portfolio { get; set; }

    public List<TelefoneFornecedor> Telefones { get; set; } = [];

    public List<EmailFornecedor> Emails { get; set; } = [];

    public List<OrcamentoFornecedor> Orcamentos { get; set; } = [];

    public List<FornecedorServico> Servicos { get; set; } = [];

    public bool CadastroCompleto =>
        !string.IsNullOrWhiteSpace(Nome)
        && Servicos.Count > 0
        && Emails.Count > 0
        && Telefones.Count > 0;
}

public sealed class TelefoneFornecedor
{
    public Guid Id { get; set; }

    public Guid FornecedorId { get; set; }

    public string? Responsavel { get; set; }

    public string? Ddi { get; set; }

    public string Numero { get; set; } = "";

    public bool Movel { get; set; } = true;
}

public sealed class EmailFornecedor
{
    public Guid Id { get; set; }

    public Guid FornecedorId { get; set; }

    public string Endereco { get; set; } = "";
}

public sealed class OrcamentoFornecedor
{
    public Guid Id { get; set; }

    public Guid FornecedorId { get; set; }

    public string Titulo { get; set; } = "";

    public string? Link { get; set; }

    public Guid? AnexoId { get; set; }

    public bool VisivelClientes { get; set; }
}

public sealed class FornecedorServico
{
    public Guid FornecedorId { get; set; }

    public Guid ServicoContaId { get; set; }
}
