namespace Project.Application.Cep;

public sealed record EnderecoCep(string Logradouro, string Estado, string Cidade, string Bairro);

public interface IConsultaCep
{
    Task<EnderecoCep?> ConsultarAsync(string cep, CancellationToken cancellationToken);
}
