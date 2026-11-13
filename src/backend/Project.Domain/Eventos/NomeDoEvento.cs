namespace Project.Domain.Eventos;

public static class NomeDoEvento
{
    public static string? Montar(TipoEvento tipo, string? primeiro, string? segundo)
    {
        var a = Comum.Texto.Limpo(primeiro);
        if (a is null)
        {
            return null;
        }

        if (tipo != TipoEvento.Casamento)
        {
            return a;
        }

        var b = Comum.Texto.Limpo(segundo);
        if (b is null)
        {
            return null;
        }

        return $"{a} & {b}";
    }
}
