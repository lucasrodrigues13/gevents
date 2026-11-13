using System.Globalization;
using System.Text;

namespace Project.Domain.Convidados;

public static class Rotulos
{
    public static SituacaoConvidado? Situacao(string? texto)
    {
        var valor = Normalizar(texto);
        return valor switch
        {
            "pendente" => SituacaoConvidado.Pendente,
            "confirmado" => SituacaoConvidado.Confirmado,
            "nao comparecera" or "cancelado" => SituacaoConvidado.NaoComparecera,
            _ => null,
        };
    }

    public static GeneroConvidado? Genero(string? texto)
    {
        var valor = Normalizar(texto);
        return valor switch
        {
            "m" or "masculino" => GeneroConvidado.Masculino,
            "f" or "feminino" => GeneroConvidado.Feminino,
            "nao binario" => GeneroConvidado.NaoBinario,
            "nao informado" => GeneroConvidado.NaoInformado,
            _ => null,
        };
    }

    public static FaixaEtaria? Faixa(string? texto)
    {
        var valor = Normalizar(texto);
        return valor switch
        {
            "adulto" => FaixaEtaria.Adulto,
            "adolescente" => FaixaEtaria.Adolescente,
            "crianca" => FaixaEtaria.Crianca,
            "crianca de colo" => FaixaEtaria.CriancaDeColo,
            "idoso" => FaixaEtaria.Idoso,
            "nao informado" => FaixaEtaria.NaoInformado,
            _ => null,
        };
    }

    public static CustoConvidado? Custo(string? texto)
    {
        var valor = Normalizar(texto);
        return valor switch
        {
            "inteira" => CustoConvidado.Inteira,
            "meia" => CustoConvidado.Meia,
            "gratuita" or "free" => CustoConvidado.Gratuita,
            "nao informado" => CustoConvidado.NaoInformado,
            _ => null,
        };
    }

    public static string SituacaoTela(SituacaoConvidado situacao) => situacao switch
    {
        SituacaoConvidado.Pendente => "Pendente",
        SituacaoConvidado.Confirmado => "Confirmado",
        SituacaoConvidado.NaoComparecera => "Não comparecerá",
        _ => situacao.ToString(),
    };

    public static string RecusaNoIcone(SituacaoConvidado situacao) =>
        situacao == SituacaoConvidado.NaoComparecera ? "Cancelado" : SituacaoTela(situacao);

    private static string? Normalizar(string? texto)
    {
        var limpo = Comum.Texto.Limpo(texto)?.ToLowerInvariant();
        if (limpo is null)
        {
            return null;
        }

        var semAcento = limpo.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(semAcento.Length);
        foreach (var caractere in semAcento)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(caractere) != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(caractere);
            }
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}
