using Project.Application.Negocio;
using Project.Domain.Eventos;
using Project.Domain.Fornecedores;

namespace Project.Tests.Negocio;

public class EstatisticasBudgetTests
{
    [Fact]
    public void Percentual_com_total_zero_fica_zero()
    {
        Assert.Equal(0m, EstatisticasCalculo.Percentual(1, 0));
        Assert.Equal(0m, EstatisticasCalculo.PercentualValor(100m, 0m));
        Assert.Equal(50m, EstatisticasCalculo.Percentual(1, 2));
    }

    [Fact]
    public void Budget_soma_subitens_e_calcula_prova_real()
    {
        var evento = new Evento
        {
            OrcamentoEstimado = 10000m,
            ConvidadosEstimados = 100,
            Servicos =
            [
                new ServicoEvento
                {
                    Nome = "Buffet",
                    ValorEstimado = 1m,
                    ValorContratado = 2000m,
                    Subitens =
                    [
                        new SubitemServico { Descricao = "Prato", Quantidade = 2, ValorUnitario = 500m },
                        new SubitemServico { Descricao = "Bebida", Quantidade = 1, ValorUnitario = 300m },
                    ],
                },
                new ServicoEvento
                {
                    Nome = "Som",
                    ValorEstimado = 400m,
                },
            ],
        };

        Assert.Equal(1300m, BudgetCalculo.EstimadoDoServico(evento.Servicos[0]));
        Assert.Equal(400m, BudgetCalculo.EstimadoDoServico(evento.Servicos[1]));

        var (estimado, contratado, provaReal, custo) = BudgetCalculo.Resumo(evento);
        Assert.Equal(1700m, estimado);
        Assert.Equal(2000m, contratado);
        Assert.Equal(3700m, provaReal);
        Assert.Equal(37m, custo);
    }

    [Fact]
    public void Negociacao_avanca_e_volta_uma_etapa()
    {
        Assert.True(NegociacaoRegras.PodeAvancar(EtapaNegociacao.Recomendado));
        Assert.Equal(EtapaNegociacao.OrcamentoSolicitado, NegociacaoRegras.Avancar(EtapaNegociacao.Recomendado));
        Assert.Null(NegociacaoRegras.Avancar(EtapaNegociacao.Contratado));
        Assert.Equal(EtapaNegociacao.OrcamentoRecebido, NegociacaoRegras.Voltar(EtapaNegociacao.Contratado));
        Assert.Null(NegociacaoRegras.Voltar(EtapaNegociacao.Recomendado));
    }
}
