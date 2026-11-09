using System.Globalization;
using System.Text;

namespace Project.Domain.Convites;

public sealed record LinhaImportada(
    string NomeConvite,
    bool LinhaDoConvite,
    string? Grupo,
    string? ObservacaoConvite,
    ConvidadoDados Dados,
    bool Principal);

public static class ImportacaoConvites
{
    public static readonly string[] Cabecalho =
    [
        "Nome do convite *",
        "DDI",
        "DDD + Telefone para confirmação de presença",
        "Grupo do convite",
        "Observação do convite",
        "Nome dos convidados *",
        "Gênero",
        "Faixa etária",
        "Custo/pagamento",
        "Situação",
        "Mesa"
    ];

    public static IReadOnlyList<LinhaImportada> Interpretar(IReadOnlyList<IReadOnlyList<string>> linhas)
    {
        var resultado = new List<LinhaImportada>();
        string? conviteAtual = null;
        foreach (var linha in linhas)
        {
            if (linha.Count == 0 || linha.All(string.IsNullOrWhiteSpace))
            {
                continue;
            }

            var nomeConvite = Celula(linha, 0);
            if (resultado.Count == 0 && conviteAtual is null && EhCabecalho(nomeConvite))
            {
                continue;
            }

            var linhaDoConvite = nomeConvite.Length > 0;
            if (!linhaDoConvite)
            {
                if (conviteAtual is null)
                {
                    throw new RegraDeNegocioException("Informe o nome do convite.");
                }

                nomeConvite = conviteAtual;
            }
            else
            {
                conviteAtual = nomeConvite;
            }

            var celular = Celula(linha, 2);
            var nomeConvidado = Grupo.ExigirNome(Celula(linha, 5), "Informe o nome do convidado.");
            var dados = new ConvidadoDados(
                nomeConvidado,
                Faixa(Celula(linha, 7)),
                Custo(Celula(linha, 8)),
                Genero(Celula(linha, 6)),
                celular.Length == 0 ? null : celular,
                null,
                null,
                false,
                null,
                Celula(linha, 1).Length == 0 ? null : Celula(linha, 1),
                Celula(linha, 10).Length == 0 ? null : Celula(linha, 10),
                null,
                null,
                Situacao(Celula(linha, 9)));
            resultado.Add(new LinhaImportada(
                nomeConvite,
                linhaDoConvite,
                linhaDoConvite && Celula(linha, 3).Length > 0 ? Celula(linha, 3) : null,
                linhaDoConvite && Celula(linha, 4).Length > 0 ? Celula(linha, 4) : null,
                dados,
                linhaDoConvite && celular.Length > 0));
        }

        return resultado;
    }

    private static bool EhCabecalho(string texto)
    {
        return texto.StartsWith("Nome do convite", StringComparison.OrdinalIgnoreCase);
    }

    private static string Celula(IReadOnlyList<string> linha, int indice)
    {
        return indice < linha.Count ? linha[indice].Trim() : "";
    }

    private static GeneroConvidado? Genero(string texto)
    {
        return Chave(texto) switch
        {
            "" => null,
            "m" or "masculino" => GeneroConvidado.Masculino,
            "f" or "feminino" => GeneroConvidado.Feminino,
            "naobinario" => GeneroConvidado.NaoBinario,
            _ => throw new RegraDeNegocioException("O valor informado não é aceito.")
        };
    }

    private static FaixaConvidado? Faixa(string texto)
    {
        return Chave(texto) switch
        {
            "" => null,
            "adulto" => FaixaConvidado.Adulto,
            "adolescente" => FaixaConvidado.Adolescente,
            "crianca" => FaixaConvidado.Crianca,
            "criancadecolo" => FaixaConvidado.CriancaDeColo,
            "idoso" => FaixaConvidado.Idoso,
            "naoinformado" => FaixaConvidado.NaoInformado,
            _ => throw new RegraDeNegocioException("O valor informado não é aceito.")
        };
    }

    private static CustoConvidado? Custo(string texto)
    {
        return Chave(texto) switch
        {
            "" => null,
            "inteira" => CustoConvidado.Inteira,
            "meia" => CustoConvidado.Meia,
            "gratuita" or "free" => CustoConvidado.Gratuita,
            "naoinformado" => CustoConvidado.NaoInformado,
            _ => throw new RegraDeNegocioException("O valor informado não é aceito.")
        };
    }

    private static SituacaoConvidado? Situacao(string texto)
    {
        return Chave(texto) switch
        {
            "" => null,
            "pendente" => SituacaoConvidado.Pendente,
            "confirmado" => SituacaoConvidado.Confirmado,
            "naocomparecera" or "cancelado" => SituacaoConvidado.NaoComparecera,
            _ => throw new RegraDeNegocioException("O valor informado não é aceito.")
        };
    }

    private static string Chave(string texto)
    {
        if (texto.Length == 0)
        {
            return "";
        }

        var semAcento = texto.Normalize(NormalizationForm.FormD);
        var letras = new StringBuilder(semAcento.Length);
        foreach (var caractere in semAcento)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(caractere) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsLetterOrDigit(caractere))
            {
                letras.Append(char.ToLowerInvariant(caractere));
            }
        }

        return letras.ToString();
    }
}
