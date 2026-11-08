namespace Project.Domain.Convites;

public static class ListasDeConvidado
{
    public static bool TentarSituacao(string? texto, out SituacaoConvidado? situacao)
    {
        return Tentar(texto, out situacao);
    }

    public static bool TentarFaixa(string? texto, out FaixaConvidado? faixa)
    {
        return Tentar(texto, out faixa);
    }

    public static bool TentarCusto(string? texto, out CustoConvidado? custo)
    {
        return Tentar(texto, out custo);
    }

    public static bool TentarGenero(string? texto, out GeneroConvidado? genero)
    {
        return Tentar(texto, out genero);
    }

    public static bool PresencaPublica(string? texto, out SituacaoConvidado situacao)
    {
        if (TentarSituacao(texto, out SituacaoConvidado? lida)
            && lida is SituacaoConvidado.Confirmado or SituacaoConvidado.NaoComparecera)
        {
            situacao = lida.Value;
            return true;
        }

        situacao = default;
        return false;
    }

    private static bool Tentar<T>(string? texto, out T? valor)
        where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            valor = null;
            return true;
        }

        if (Enum.TryParse<T>(texto, ignoreCase: true, out var lido) && Enum.IsDefined(lido))
        {
            valor = lido;
            return true;
        }

        valor = null;
        return false;
    }
}
