namespace Project.Domain.Eventos;

public static class TiposDeEvento
{
    public static bool TentarLer(string? texto, out TipoEvento? tipo)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            tipo = null;
            return true;
        }

        if (Enum.TryParse<TipoEvento>(texto, ignoreCase: true, out var lido) && Enum.IsDefined(lido))
        {
            tipo = lido;
            return true;
        }

        tipo = null;
        return false;
    }
}
