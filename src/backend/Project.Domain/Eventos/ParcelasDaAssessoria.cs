namespace Project.Domain.Eventos;

public enum BaseEntrada
{
    Valor,
    Porcentagem,
}

public sealed record ParcelaCalculada(int Numero, DateOnly Vencimento, decimal Valor, bool Entrada);

public static class ParcelasDaAssessoria
{
    public const string NomeServico = "Assessoria e Organização";

    public static IReadOnlyList<ParcelaCalculada> Calcular(
        decimal total,
        int quantidade,
        DateOnly primeiraParcela,
        bool comEntrada,
        BaseEntrada? baseEntrada,
        decimal? valorInformado,
        DateOnly? dataEntrada)
    {
        if (total <= 0 || quantidade <= 0)
        {
            return [];
        }

        decimal entrada = 0;
        if (comEntrada)
        {
            if (baseEntrada is null || valorInformado is null || dataEntrada is null || valorInformado <= 0)
            {
                return [];
            }

            entrada = baseEntrada == BaseEntrada.Porcentagem
                ? decimal.Round(total * valorInformado.Value / 100m, 2, MidpointRounding.AwayFromZero)
                : decimal.Round(valorInformado.Value, 2, MidpointRounding.AwayFromZero);

            if (entrada <= 0 || entrada > total)
            {
                return [];
            }
        }

        var parcelas = new List<ParcelaCalculada>();
        if (comEntrada && dataEntrada is not null)
        {
            parcelas.Add(new ParcelaCalculada(0, dataEntrada.Value, entrada, true));
        }

        var restante = total - entrada;
        var centavos = (int)decimal.Round(restante * 100m, 0, MidpointRounding.AwayFromZero);
        var baseCentavos = centavos / quantidade;
        var sobra = centavos - (baseCentavos * quantidade);
        for (var i = 0; i < quantidade; i++)
        {
            var cents = baseCentavos + (i == quantidade - 1 ? sobra : 0);
            parcelas.Add(new ParcelaCalculada(i + 1, primeiraParcela.AddMonths(i), cents / 100m, false));
        }

        return parcelas;
    }
}
