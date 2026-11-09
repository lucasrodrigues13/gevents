namespace Project.Domain.Clientes;

public static class PapeisDeCliente
{
    public static readonly string[] Todos =
    [
        "Anfitrião",
        "Aniversariante",
        "Avó",
        "Avô",
        "Comissão",
        "Formando(a)",
        "Mãe",
        "Noiva",
        "Noivo",
        "Pai",
        "Promotor",
        "Representante",
        "Tia",
        "Tio"
    ];

    public static bool TentarLer(string? texto, out string? papel)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            papel = null;
            return false;
        }

        papel = Todos.FirstOrDefault(item => item.Equals(texto.Trim(), StringComparison.OrdinalIgnoreCase));
        return papel is not null;
    }
}
