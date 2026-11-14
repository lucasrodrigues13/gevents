using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Project.Application.Cep;

namespace Project.Infrastructure.Cep;

public sealed class ViaCepConsulta(HttpClient http) : IConsultaCep
{
    public async Task<EnderecoCep?> ConsultarAsync(string cep, CancellationToken ct)
    {
        if (cep.Length != 8 || cep.Any(digito => !char.IsDigit(digito)))
        {
            return null;
        }

        using var resposta = await http.GetAsync($"ws/{cep}/json/", ct);
        if (!resposta.IsSuccessStatusCode)
        {
            return null;
        }

        var corpo = await resposta.Content.ReadFromJsonAsync<Corpo>(cancellationToken: ct);
        if (corpo is null || corpo.Erro || string.IsNullOrWhiteSpace(corpo.Uf))
        {
            return null;
        }

        return new EnderecoCep(corpo.Logradouro ?? "", corpo.Uf, corpo.Localidade ?? "", corpo.Bairro ?? "", "Brasil");
    }

    private sealed class Corpo
    {
        public string? Logradouro { get; set; }

        public string? Uf { get; set; }

        public string? Localidade { get; set; }

        public string? Bairro { get; set; }

        [JsonPropertyName("erro")]
        public bool Erro { get; set; }
    }
}
