using Project.Domain.Eventos;

namespace Project.Domain.Clientes;

public sealed record ClienteDados(
    string? Nome,
    string? Papel,
    string? Email,
    string? Ddi,
    string? Celular,
    string? TelefoneFixo,
    DateOnly? Nascimento,
    string? Cpf,
    string? Rg,
    string? Cnpj,
    string? Profissao,
    string? InscricaoMunicipal,
    string? Religiao,
    string? Observacao,
    string? Facebook,
    string? Instagram,
    EnderecoLocal? Endereco,
    string? Complemento);
