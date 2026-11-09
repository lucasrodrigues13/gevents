namespace Project.Domain.Eventos;

public sealed record EnderecoLocal(
    string? Pais,
    string? Cep,
    string? Logradouro,
    string? Numero,
    string? Estado,
    string? Cidade,
    string? Bairro)
{
    public static EnderecoLocal Vazio { get; } = new(null, null, null, null, null, null, null);

    public EnderecoLocal Normalizar()
    {
        return new EnderecoLocal(
            Limpar(Pais),
            Limpar(Cep),
            Limpar(Logradouro),
            Limpar(Numero),
            Limpar(Estado),
            Limpar(Cidade),
            Limpar(Bairro));
    }

    public string? CepDigitos()
    {
        if (string.IsNullOrWhiteSpace(Cep))
        {
            return null;
        }

        var digitos = new string(Cep.Where(char.IsDigit).ToArray());
        return digitos.Length == 0 ? null : digitos;
    }

    private static string? Limpar(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    }
}
