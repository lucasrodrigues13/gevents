using Microsoft.AspNetCore.Http;
using Project.Domain.Clientes;
using Project.Domain.Eventos;

namespace Project.Web.Clientes;

public sealed class ClienteInput
{
    public string? Nome { get; set; }

    public string? Papel { get; set; }

    public string? Email { get; set; }

    public string? Ddi { get; set; } = "+55";

    public string? Celular { get; set; }

    public string? TelefoneFixo { get; set; }

    public DateOnly? Nascimento { get; set; }

    public string? Cpf { get; set; }

    public string? Rg { get; set; }

    public string? Cnpj { get; set; }

    public string? Profissao { get; set; }

    public string? InscricaoMunicipal { get; set; }

    public string? Religiao { get; set; }

    public string? Observacao { get; set; }

    public string? Facebook { get; set; }

    public string? Instagram { get; set; }

    public string? Pais { get; set; }

    public string? Cep { get; set; }

    public string? Logradouro { get; set; }

    public string? Numero { get; set; }

    public string? Estado { get; set; }

    public string? Cidade { get; set; }

    public string? Bairro { get; set; }

    public string? Complemento { get; set; }

    public IFormFile? Imagem { get; set; }

    public ClienteDados ParaDados()
    {
        return new ClienteDados(
            Nome,
            Papel,
            Email,
            Ddi,
            Celular,
            TelefoneFixo,
            Nascimento,
            Cpf,
            Rg,
            Cnpj,
            Profissao,
            InscricaoMunicipal,
            Religiao,
            Observacao,
            Facebook,
            Instagram,
            new EnderecoLocal(Pais, Cep, Logradouro, Numero, Estado, Cidade, Bairro),
            Complemento);
    }

    public (Stream? Conteudo, string? Nome) AbrirImagem()
    {
        if (Imagem is null || (Imagem.Length == 0 && string.IsNullOrWhiteSpace(Imagem.FileName)))
        {
            return (null, null);
        }

        return (Imagem.OpenReadStream(), Imagem.FileName);
    }

    public static ClienteInput De(Cliente cliente)
    {
        return new ClienteInput
        {
            Nome = cliente.Nome,
            Papel = cliente.Papel,
            Email = cliente.Email,
            Ddi = cliente.Ddi ?? "+55",
            Celular = cliente.Celular,
            TelefoneFixo = cliente.Telefone,
            Nascimento = cliente.Nascimento,
            Cpf = cliente.Cpf,
            Rg = cliente.Rg,
            Cnpj = cliente.Cnpj,
            Profissao = cliente.Profissao,
            InscricaoMunicipal = cliente.InscricaoMunicipal,
            Religiao = cliente.Religiao,
            Observacao = cliente.Observacao,
            Facebook = cliente.Facebook,
            Instagram = cliente.Instagram,
            Pais = cliente.Pais,
            Cep = cliente.Cep,
            Logradouro = cliente.Logradouro,
            Numero = cliente.Numero,
            Estado = cliente.Estado,
            Cidade = cliente.Cidade,
            Bairro = cliente.Bairro,
            Complemento = cliente.Complemento
        };
    }
}
