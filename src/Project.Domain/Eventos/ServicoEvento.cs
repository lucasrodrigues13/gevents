namespace Project.Domain.Eventos;

public enum BaseEntrada
{
    Valor = 0,
    Porcentagem = 1
}

public sealed class ServicoEvento
{
    private ServicoEvento()
    {
    }

    public Guid Id { get; private set; }

    public string AccountId { get; private set; } = "";

    public Guid EventoId { get; private set; }

    public string Nome { get; private set; } = "";

    public bool Contratado { get; private set; }

    public string? Fornecedor { get; private set; }

    public decimal? Valor { get; private set; }

    public static ServicoEvento Copiar(string accountId, Guid eventoId, string nome)
    {
        return new ServicoEvento
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            EventoId = eventoId,
            Nome = nome,
            Contratado = false
        };
    }

    public static ServicoEvento ContratarAssessoria(string accountId, Guid eventoId, string fornecedor, decimal valor)
    {
        if (valor <= 0)
        {
            throw new RegraDeNegocioException("Informe um valor maior que zero.");
        }

        return new ServicoEvento
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            EventoId = eventoId,
            Nome = "Assessoria e Organização",
            Contratado = true,
            Fornecedor = string.IsNullOrWhiteSpace(fornecedor) ? "Conta" : fornecedor.Trim(),
            Valor = valor
        };
    }
}

public sealed class ParcelaEvento
{
    private ParcelaEvento()
    {
    }

    public Guid Id { get; private set; }

    public string AccountId { get; private set; } = "";

    public Guid EventoId { get; private set; }

    public Guid ServicoEventoId { get; private set; }

    public int Numero { get; private set; }

    public DateOnly Vencimento { get; private set; }

    public decimal Valor { get; private set; }

    public bool Entrada { get; private set; }

    public static IReadOnlyList<ParcelaEvento> Gerar(
        string accountId,
        Guid eventoId,
        Guid servicoId,
        decimal valorTotal,
        int quantidade,
        DateOnly primeira,
        decimal? entrada,
        BaseEntrada? baseEntrada,
        DateOnly? dataEntrada)
    {
        if (valorTotal <= 0)
        {
            throw new RegraDeNegocioException("Informe um valor maior que zero.");
        }

        if (quantidade <= 0)
        {
            throw new RegraDeNegocioException("Informe a quantidade de parcelas.");
        }

        var valorEntrada = 0m;
        if (entrada is not null)
        {
            if (baseEntrada is null || dataEntrada is null)
            {
                throw new RegraDeNegocioException("Informe a base e a data da entrada.");
            }

            valorEntrada = baseEntrada == BaseEntrada.Porcentagem
                ? decimal.Round(valorTotal * entrada.Value / 100m, 2, MidpointRounding.AwayFromZero)
                : entrada.Value;
            if (valorEntrada < 0 || valorEntrada > valorTotal)
            {
                throw new RegraDeNegocioException("A entrada não pode passar do valor total.");
            }
        }

        var saldo = valorTotal - valorEntrada;
        var parcelas = new List<ParcelaEvento>();
        if (valorEntrada > 0)
        {
            parcelas.Add(Criar(accountId, eventoId, servicoId, 0, dataEntrada!.Value, valorEntrada, true));
        }

        var baseValor = decimal.Round(saldo / quantidade, 2, MidpointRounding.AwayFromZero);
        var acumulado = 0m;
        for (var indice = 0; indice < quantidade; indice++)
        {
            var valor = indice == quantidade - 1 ? saldo - acumulado : baseValor;
            acumulado += valor;
            parcelas.Add(Criar(accountId, eventoId, servicoId, indice + 1, primeira.AddMonths(indice), valor, false));
        }

        return parcelas;
    }

    private static ParcelaEvento Criar(
        string accountId,
        Guid eventoId,
        Guid servicoId,
        int numero,
        DateOnly vencimento,
        decimal valor,
        bool entrada)
    {
        return new ParcelaEvento
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            EventoId = eventoId,
            ServicoEventoId = servicoId,
            Numero = numero,
            Vencimento = vencimento,
            Valor = valor,
            Entrada = entrada
        };
    }
}
