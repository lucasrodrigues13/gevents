using System.Net.Mail;
using Project.Domain.Eventos;

namespace Project.Domain.Clientes;

public sealed class Cliente
{
    private Cliente()
    {
    }

    public Guid Id { get; private set; }

    public string AccountId { get; private set; } = "";

    public string Nome { get; private set; } = "";

    public string? Telefone { get; private set; }

    public string? Email { get; private set; }

    public string? Papel { get; private set; }

    public string? Ddi { get; private set; }

    public string? Celular { get; private set; }

    public DateOnly? Nascimento { get; private set; }

    public string? Cpf { get; private set; }

    public string? Rg { get; private set; }

    public string? Cnpj { get; private set; }

    public string? Profissao { get; private set; }

    public string? InscricaoMunicipal { get; private set; }

    public string? Religiao { get; private set; }

    public string? Observacao { get; private set; }

    public string? Facebook { get; private set; }

    public string? Instagram { get; private set; }

    public string? Pais { get; private set; }

    public string? Cep { get; private set; }

    public string? Logradouro { get; private set; }

    public string? Numero { get; private set; }

    public string? Estado { get; private set; }

    public string? Cidade { get; private set; }

    public string? Bairro { get; private set; }

    public string? Complemento { get; private set; }

    public Guid? ArquivoId { get; private set; }

    public static Cliente Criar(string accountId, ClienteDados dados)
    {
        if (string.IsNullOrWhiteSpace(accountId))
        {
            throw new RegraDeNegocioException("O cliente precisa de uma conta.");
        }

        var cliente = new Cliente
        {
            Id = Guid.NewGuid(),
            AccountId = accountId
        };
        cliente.Aplicar(dados);
        return cliente;
    }

    public void Editar(ClienteDados dados)
    {
        Aplicar(dados);
    }

    public void DefinirArquivo(Guid? arquivoId)
    {
        ArquivoId = arquivoId;
    }

    public EnderecoLocal Endereco()
    {
        return new EnderecoLocal(Pais, Cep, Logradouro, Numero, Estado, Cidade, Bairro);
    }

    private void Aplicar(ClienteDados dados)
    {
        var nome = dados.Nome?.Trim() ?? "";
        if (nome.Length == 0)
        {
            throw new RegraDeNegocioException("Informe o nome do cliente.");
        }

        var email = dados.Email?.Trim() ?? "";
        if (email.Length > 0 && !EmailValido(email))
        {
            throw new RegraDeNegocioException("Informe um e-mail válido.");
        }

        if (!PapeisDeCliente.TentarLer(dados.Papel, out var papel))
        {
            throw new RegraDeNegocioException("O papel do cliente não é aceito.");
        }

        if (email.Length == 0)
        {
            throw new RegraDeNegocioException("Informe o e-mail do cliente.");
        }

        var ddi = string.IsNullOrWhiteSpace(dados.Ddi) ? "" : dados.Ddi.Trim();
        var celular = dados.Celular?.Trim() ?? "";
        if (ddi.Length == 0 || celular.Length == 0)
        {
            throw new RegraDeNegocioException("Informe o celular do cliente.");
        }

        var cpf = Digitos(dados.Cpf);
        var cnpj = Digitos(dados.Cnpj);
        var endereco = (dados.Endereco ?? EnderecoLocal.Vazio).Normalizar();
        var cep = endereco.CepDigitos();
        if (cpf.Length > 0 && cpf.Length != 11)
        {
            throw new RegraDeNegocioException("O CPF precisa ter 11 dígitos.");
        }

        if (cnpj.Length > 0 && cnpj.Length != 14)
        {
            throw new RegraDeNegocioException("O CNPJ precisa ter 14 dígitos.");
        }

        if (cep is not null && cep.Length != 8)
        {
            throw new RegraDeNegocioException("O CEP precisa ter 8 dígitos.");
        }

        Nome = nome;
        Papel = papel;
        Email = email;
        Ddi = ddi;
        Celular = celular;
        Telefone = string.IsNullOrWhiteSpace(dados.TelefoneFixo) ? null : dados.TelefoneFixo.Trim();
        Nascimento = dados.Nascimento;
        Cpf = cpf.Length == 0 ? null : cpf;
        Rg = Limpar(dados.Rg);
        Cnpj = cnpj.Length == 0 ? null : cnpj;
        Profissao = Limpar(dados.Profissao);
        InscricaoMunicipal = Limpar(dados.InscricaoMunicipal);
        Religiao = Limpar(dados.Religiao);
        Observacao = Limpar(dados.Observacao);
        Facebook = Limpar(dados.Facebook);
        Instagram = Limpar(dados.Instagram);
        Pais = endereco.Pais;
        Cep = cep;
        Logradouro = endereco.Logradouro;
        Numero = endereco.Numero;
        Estado = endereco.Estado;
        Cidade = endereco.Cidade;
        Bairro = endereco.Bairro;
        Complemento = Limpar(dados.Complemento);
    }

    private static string Digitos(string? valor)
    {
        return valor is null ? "" : new string(valor.Where(char.IsDigit).ToArray());
    }

    private static string? Limpar(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    }

    private static bool EmailValido(string email)
    {
        return MailAddress.TryCreate(email, out var endereco)
            && endereco.Address.Equals(email, StringComparison.OrdinalIgnoreCase);
    }
}

public sealed class VinculoCliente
{
    private VinculoCliente()
    {
    }

    public Guid Id { get; private set; }

    public string AccountId { get; private set; } = "";

    public Guid ClienteId { get; private set; }

    public Guid EventoId { get; private set; }

    public static VinculoCliente Criar(string accountId, Guid clienteId, Guid eventoId)
    {
        if (string.IsNullOrWhiteSpace(accountId) || clienteId == Guid.Empty || eventoId == Guid.Empty)
        {
            throw new RegraDeNegocioException("O vínculo precisa da conta, do cliente e do evento.");
        }

        return new VinculoCliente
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            ClienteId = clienteId,
            EventoId = eventoId
        };
    }
}
