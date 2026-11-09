using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Project.Application.Cep;

namespace Project.Infrastructure.Cep;

public sealed class CepOptions
{
    public const string SectionName = "Cep";

    public string? BaseUrl { get; set; }
}

public sealed class ConsultaCepHttp : IConsultaCep
{
    private readonly HttpClient _http;
    private readonly CepOptions _options;

    public ConsultaCepHttp(HttpClient http, IOptions<CepOptions> options)
    {
        _http = http;
        _options = options.Value;
    }

    public async Task<EnderecoCep?> ConsultarAsync(string cep, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.BaseUrl))
        {
            return null;
        }

        var digitos = new string(cep.Where(char.IsDigit).ToArray());
        if (digitos.Length != 8)
        {
            return null;
        }

        try
        {
            var resposta = await _http.GetFromJsonAsync<CepJson>($"{_options.BaseUrl.TrimEnd('/')}/ws/{digitos}/json/", cancellationToken);
            if (resposta is null || resposta.Erro || string.IsNullOrWhiteSpace(resposta.Logradouro))
            {
                return null;
            }

            return new EnderecoCep(resposta.Logradouro, resposta.Uf ?? "", resposta.Localidade ?? "", resposta.Bairro ?? "");
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (TaskCanceledException)
        {
            return null;
        }
    }

    private sealed class CepJson
    {
        [JsonPropertyName("logradouro")]
        public string? Logradouro { get; set; }

        [JsonPropertyName("bairro")]
        public string? Bairro { get; set; }

        [JsonPropertyName("localidade")]
        public string? Localidade { get; set; }

        [JsonPropertyName("uf")]
        public string? Uf { get; set; }

        [JsonPropertyName("erro")]
        public bool Erro { get; set; }
    }
}
