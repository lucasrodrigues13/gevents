using Project.Application.Comum;
using Project.Domain.Funil;

namespace Project.Application.Negocio;

public sealed record LinhaRelatorioFunil(string Chave, int Quantidade, decimal Valor);

public sealed record RegiaoRelatorioFunil(string Uf, string Cidade, int Quantidade, decimal Valor);

public sealed record RelatorioRsvpFunil(
    int Visualizacoes,
    int CliquesBotao,
    int Contatados,
    int Descartados,
    int Geradas,
    decimal Conversao,
    int ConversoesAnteriores);

public sealed record CatalogoRelatoriosFunil(
    IReadOnlyList<LinhaRelatorioFunil> FechadosPerdidos,
    IReadOnlyList<LinhaRelatorioFunil> PorFonte,
    IReadOnlyList<LinhaRelatorioFunil> PorServicoProduto,
    IReadOnlyList<LinhaRelatorioFunil> PorEstagio,
    IReadOnlyList<RegiaoRelatorioFunil> PorRegiao,
    IReadOnlyList<LinhaRelatorioFunil> PorResponsavel,
    IReadOnlyList<LinhaRelatorioFunil> PorDataPrevista,
    IReadOnlyList<LinhaRelatorioFunil> PorMotivosDescarte,
    RelatorioRsvpFunil OportunidadesRsvp);

public sealed class RelatorioFunilServico(IDados dados)
{
    public async Task<Resultado<CatalogoRelatoriosFunil>> ObterAsync(
        string contaId,
        DateOnly? inicio,
        DateOnly? fim,
        CancellationToken ct)
    {
        if (inicio is not DateOnly de || fim is not DateOnly ate)
        {
            return Resultado<CatalogoRelatoriosFunil>.Erro(Codigos.Validacao, "Informe o período.");
        }

        if (ate < de)
        {
            return Resultado<CatalogoRelatoriosFunil>.Erro(Codigos.Validacao, "O fim não pode ser anterior ao início.");
        }

        var inicioUtc = new DateTimeOffset(de.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        var fimUtc = new DateTimeOffset(ate.ToDateTime(TimeOnly.MaxValue), TimeSpan.Zero);

        var porEntrada = await dados.ListarOportunidadesPorEntradaEstagioAsync(contaId, inicioUtc, fimUtc, ct);
        var porCriacao = await dados.ListarOportunidadesPorCriacaoAsync(contaId, inicioUtc, fimUtc, ct);

        var fechados = porEntrada.Where(o => o.Estagio == EstagioOportunidade.Contratado).ToArray();
        var perdidos = porEntrada.Where(o => o.Estagio == EstagioOportunidade.Descartado).ToArray();

        var catalogo = new CatalogoRelatoriosFunil(
            [
                new("Fechados", fechados.Length, fechados.Sum(o => o.ValorProposta ?? 0)),
                new("Perdidos", perdidos.Length, perdidos.Sum(o => o.ValorProposta ?? 0)),
            ],
            Agrupar(porCriacao, o => o.Fonte ?? "(sem fonte)"),
            Agrupar(porCriacao, o => o.ServicoProduto ?? "(sem serviço)"),
            RotulosEstagio.Todos.Select(estagio =>
            {
                var itens = porCriacao.Where(o => o.Estagio == estagio).ToArray();
                return new LinhaRelatorioFunil(RotulosEstagio.Texto(estagio), itens.Length, itens.Sum(o => o.ValorProposta ?? 0));
            }).ToArray(),
            porCriacao
                .GroupBy(o => (Uf: o.Uf ?? "(sem UF)", Cidade: o.Cidade ?? "(sem cidade)"))
                .Select(g => new RegiaoRelatorioFunil(g.Key.Uf, g.Key.Cidade, g.Count(), g.Sum(o => o.ValorProposta ?? 0)))
                .OrderBy(g => g.Uf).ThenBy(g => g.Cidade)
                .ToArray(),
            Agrupar(porCriacao, o => o.Responsavel),
            Agrupar(
                porCriacao.Where(o => o.DataPrevista is not null),
                o => o.DataPrevista!.Value.ToString("yyyy-MM-dd")),
            Agrupar(
                porEntrada.Where(o => o.Estagio == EstagioOportunidade.Descartado),
                o => o.MotivoDescarte ?? "(sem motivo)"),
            await MontarRsvpAsync(contaId, inicioUtc, fimUtc, ct));

        return Resultado<CatalogoRelatoriosFunil>.Sucesso(catalogo);
    }

    private async Task<RelatorioRsvpFunil> MontarRsvpAsync(
        string contaId,
        DateTimeOffset inicio,
        DateTimeOffset fim,
        CancellationToken ct)
    {
        var metricas = await dados.ObterMetricasRsvpFunilAsync(contaId, ct);
        var leads = await dados.ListarLeadsRsvpPeriodoAsync(contaId, inicio, fim, ct);
        var anteriores = await dados.ContarLeadsRsvpConvertidosAntesAsync(contaId, inicio, ct);
        var cliques = leads.Count;
        var contatados = leads.Count(l => l.ContatadoEm is not null);
        var descartados = leads.Count(l => l.Situacao == SituacaoLeadRsvp.Descartado);
        var geradas = leads.Count(l => l.Situacao == SituacaoLeadRsvp.Convertido);
        var conversao = cliques == 0 ? 0m : Math.Round((decimal)geradas / cliques * 100m, 2);
        return new RelatorioRsvpFunil(
            metricas?.Visualizacoes ?? 0,
            metricas?.CliquesBotao ?? cliques,
            contatados,
            descartados,
            geradas,
            conversao,
            anteriores);
    }

    private static IReadOnlyList<LinhaRelatorioFunil> Agrupar(
        IEnumerable<Oportunidade> origem,
        Func<Oportunidade, string> chave) =>
        origem
            .GroupBy(chave)
            .Select(g => new LinhaRelatorioFunil(g.Key, g.Count(), g.Sum(o => o.ValorProposta ?? 0)))
            .OrderBy(g => g.Chave)
            .ToArray();
}
