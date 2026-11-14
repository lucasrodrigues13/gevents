using Project.Domain.Eventos;

namespace Project.Application.Negocio;

public sealed record SituacaoEstatistica(SituacaoEvento Situacao, int Quantidade, decimal Percentual);

public sealed record MesEstatistica(int Mes, int Quantidade);

public sealed record RankingOrcamento(int Posicao, string Servico, decimal Percentual);

public sealed record RankingFornecedor(int Posicao, string Fornecedor, int Quantidade);

public sealed record RankingServico(int Posicao, string Servico, int Quantidade);

public sealed record ResumoEstatisticas(
    int TarefasSemana,
    int AniversariosHoje,
    string RotuloPagamentos,
    string RotuloTarefas,
    string RotuloAnexos,
    string RotuloEquipe,
    string RotuloAniversarios,
    string ContadorTarefas,
    string ContadorAniversarios);

public sealed record EstatisticasEventos(
    int Total,
    IReadOnlyList<SituacaoEstatistica> Situacoes,
    IReadOnlyList<MesEstatistica> EventosPorMes,
    IReadOnlyList<RankingOrcamento> RankingOrcamento,
    IReadOnlyList<RankingFornecedor> RankingFornecedores,
    IReadOnlyList<RankingServico> RankingServicos,
    ResumoEstatisticas Resumo);

public static class EstatisticasCalculo
{
    public static decimal Percentual(int quantidade, int total) =>
        total <= 0 ? 0m : decimal.Round(100m * quantidade / total, 2, MidpointRounding.AwayFromZero);

    public static decimal PercentualValor(decimal valor, decimal total) =>
        total <= 0 ? 0m : decimal.Round(100m * valor / total, 2, MidpointRounding.AwayFromZero);

    public static (DateOnly Inicio, DateOnly Fim) SemanaUtc(DateTimeOffset agoraUtc)
    {
        var hoje = DateOnly.FromDateTime(agoraUtc.UtcDateTime);
        var diasDesdeSegunda = ((int)hoje.DayOfWeek + 6) % 7;
        var inicio = hoje.AddDays(-diasDesdeSegunda);
        return (inicio, inicio.AddDays(6));
    }
}

public sealed class EstatisticasServico(IDados dados)
{
    public async Task<EstatisticasEventos> ObterAsync(string contaId, DateTimeOffset agoraUtc, CancellationToken ct)
    {
        var contagens = await dados.ContarEventosAsync(contaId, ct);
        var mapa = contagens.ToDictionary(item => item.Situacao, item => item.Quantidade);
        var total = mapa.Values.Sum();
        var situacoes = Enum.GetValues<SituacaoEvento>()
            .Select(situacao =>
            {
                var quantidade = mapa.GetValueOrDefault(situacao);
                return new SituacaoEstatistica(situacao, quantidade, EstatisticasCalculo.Percentual(quantidade, total));
            })
            .ToArray();

        var ano = agoraUtc.UtcDateTime.Year;
        var porMes = (await dados.ContarEventosPorMesAsync(contaId, ano, ct)).ToDictionary(item => item.Mes, item => item.Quantidade);
        var eventosPorMes = Enumerable.Range(1, 12)
            .Select(mes => new MesEstatistica(mes, porMes.GetValueOrDefault(mes)))
            .ToArray();

        var orcamento = await dados.ParticipacaoOrcamentoConcluidosAsync(contaId, ct);
        var totalOrcamento = orcamento.Sum(item => item.Valor);
        var rankingOrcamento = orcamento
            .OrderByDescending(item => item.Valor)
            .ThenBy(item => item.Nome, StringComparer.OrdinalIgnoreCase)
            .Select((item, indice) => new RankingOrcamento(
                indice + 1,
                item.Nome,
                EstatisticasCalculo.PercentualValor(item.Valor, totalOrcamento)))
            .ToArray();

        var rankingFornecedores = (await dados.ContarFornecedoresContratadosAsync(contaId, ct))
            .OrderByDescending(item => item.Quantidade)
            .ThenBy(item => item.Nome, StringComparer.OrdinalIgnoreCase)
            .Select((item, indice) => new RankingFornecedor(indice + 1, item.Nome, item.Quantidade))
            .ToArray();

        var rankingServicos = (await dados.ContarServicosUtilizadosAsync(contaId, ct))
            .OrderByDescending(item => item.Quantidade)
            .ThenBy(item => item.Nome, StringComparer.OrdinalIgnoreCase)
            .Select((item, indice) => new RankingServico(indice + 1, item.Nome, item.Quantidade))
            .ToArray();

        var (inicio, fim) = EstatisticasCalculo.SemanaUtc(agoraUtc);
        var tarefasSemana = await dados.ContarTarefasNaSemanaAsync(contaId, inicio, fim, ct);
        var resumo = new ResumoEstatisticas(
            tarefasSemana,
            0,
            "Pagamentos",
            "Tarefas",
            "Anexos",
            "Equipe escalada",
            "Lembretes de aniversários",
            "Para essa semana",
            "Para hoje");

        return new EstatisticasEventos(
            total,
            situacoes,
            eventosPorMes,
            rankingOrcamento,
            rankingFornecedores,
            rankingServicos,
            resumo);
    }
}
