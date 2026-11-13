using Project.Application.Cep;

namespace Project.IntegrationTests.Suporte;

public sealed class CepDeTeste : IConsultaCep
{
    public Task<EnderecoCep?> ConsultarAsync(string cep, CancellationToken ct) =>
        Task.FromResult(cep == "01001000"
            ? new EnderecoCep("Praça da Sé", "SP", "São Paulo", "Sé")
            : null);
}
