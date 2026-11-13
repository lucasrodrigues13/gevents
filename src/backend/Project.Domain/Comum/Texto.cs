namespace Project.Domain.Comum;

public static class Texto
{
    public static string? Limpo(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return null;
        }

        return valor.Trim();
    }

    public static bool Vazio(string? valor) => string.IsNullOrWhiteSpace(valor);

    public static bool EmailValido(string? valor)
    {
        var texto = Limpo(valor);
        if (texto is null || texto.Contains(' '))
        {
            return false;
        }

        var arroba = texto.IndexOf('@');
        if (arroba <= 0 || arroba != texto.LastIndexOf('@'))
        {
            return false;
        }

        var dominio = texto[(arroba + 1)..];
        return dominio.Contains('.') && !dominio.StartsWith('.') && !dominio.EndsWith('.');
    }
}
