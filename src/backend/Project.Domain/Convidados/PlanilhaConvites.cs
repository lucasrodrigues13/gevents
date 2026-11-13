namespace Project.Domain.Convidados;

public static class PlanilhaConvites
{
    public static readonly string[] Colunas =
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
        "Mesa",
    ];

    public static bool CabecalhoValido(IReadOnlyList<string?> colunas)
    {
        if (colunas.Count < Colunas.Length)
        {
            return false;
        }

        for (var i = 0; i < Colunas.Length; i++)
        {
            if (!string.Equals(colunas[i]?.Trim(), Colunas[i], StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }
}
