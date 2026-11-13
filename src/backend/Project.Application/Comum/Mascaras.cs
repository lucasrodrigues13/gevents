namespace Project.Application.Comum;

public static class Mascaras
{
    public static string? Cep(string? valor)
    {
        var digitos = SoDigitos(valor);
        if (digitos is null)
        {
            return null;
        }

        if (digitos.Length != 8)
        {
            return "";
        }

        return $"{digitos[..5]}-{digitos[5..]}";
    }

    public static string? Cpf(string? valor) => Mascara(valor, 11, "{0}.{1}.{2}-{3}", [3, 3, 3, 2]);

    public static string? Cnpj(string? valor) => Mascara(valor, 14, "{0}.{1}.{2}/{3}-{4}", [2, 3, 3, 4, 2]);

    public static string? SoDigitos(string? valor)
    {
        var texto = valor?.Trim();
        if (string.IsNullOrEmpty(texto))
        {
            return null;
        }

        var digitos = new string(texto.Where(char.IsDigit).ToArray());
        return digitos.Length == 0 ? null : digitos;
    }

    private static string? Mascara(string? valor, int tamanho, string formato, int[] partes)
    {
        var digitos = SoDigitos(valor);
        if (digitos is null)
        {
            return null;
        }

        if (digitos.Length != tamanho)
        {
            return "";
        }

        var pedacos = new object[partes.Length];
        var cursor = 0;
        for (var i = 0; i < partes.Length; i++)
        {
            pedacos[i] = digitos.Substring(cursor, partes[i]);
            cursor += partes[i];
        }

        return string.Format(System.Globalization.CultureInfo.InvariantCulture, formato, pedacos);
    }
}
