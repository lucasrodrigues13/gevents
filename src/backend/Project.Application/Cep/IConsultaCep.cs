namespace Project.Application.Cep;

public sealed record EnderecoCep(string Endereco, string Estado, string Cidade, string Bairro, string Pais = "Brasil");

public interface IConsultaCep
{
    Task<EnderecoCep?> ConsultarAsync(string cep, CancellationToken ct);
}
