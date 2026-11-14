using Project.Domain.Eventos;
using Project.Domain.Fornecedores;

namespace Project.Application.Negocio;

public static class NegociacaoRegras
{
    public static bool PodeAvancar(EtapaNegociacao etapa) => etapa < EtapaNegociacao.Contratado;

    public static bool PodeVoltar(EtapaNegociacao etapa) => etapa > EtapaNegociacao.Recomendado;

    public static EtapaNegociacao? Avancar(EtapaNegociacao etapa) =>
        PodeAvancar(etapa) ? etapa + 1 : null;

    public static EtapaNegociacao? Voltar(EtapaNegociacao etapa) =>
        PodeVoltar(etapa) ? etapa - 1 : null;
}

public static class BudgetCalculo
{
    public static decimal SomaSubitens(IEnumerable<SubitemServico> subitens) =>
        subitens.Sum(item => item.Quantidade * item.ValorUnitario);

    public static decimal EstimadoDoServico(ServicoEvento servico) =>
        servico.Subitens.Count > 0
            ? SomaSubitens(servico.Subitens)
            : servico.ValorEstimado ?? 0m;

    public static decimal ContratadoDoServico(ServicoEvento servico) =>
        servico.ValorContratado ?? 0m;

    public static (decimal Estimado, decimal Contratado, decimal ProvaReal, decimal? CustoPorConvidado) Resumo(
        Evento evento)
    {
        var estimado = evento.Servicos.Sum(EstimadoDoServico);
        var contratado = evento.Servicos.Sum(ContratadoDoServico);
        var provaReal = estimado + contratado;
        decimal? custo = evento.ConvidadosEstimados is > 0
            ? decimal.Round(provaReal / evento.ConvidadosEstimados.Value, 2, MidpointRounding.AwayFromZero)
            : null;
        return (estimado, contratado, provaReal, custo);
    }
}
