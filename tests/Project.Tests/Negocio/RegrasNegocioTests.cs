using Project.Domain.Convidados;
using Project.Domain.Eventos;
using Project.Infrastructure.Convidados;

namespace Project.Tests.Negocio;

public class RegrasNegocioTests
{
    [Fact]
    public void Casamento_une_os_dois_nomes_com_e_comercial()
    {
        Assert.Equal("Ana & Bruno", NomeDoEvento.Montar(TipoEvento.Casamento, " Ana ", "Bruno"));
        Assert.Null(NomeDoEvento.Montar(TipoEvento.Casamento, "Ana", " "));
        Assert.Equal("Helena", NomeDoEvento.Montar(TipoEvento.Aniversario, " Helena ", null));
    }

    [Fact]
    public void Assessoria_separa_entrada_e_parcelas()
    {
        var parcelas = ParcelasDaAssessoria.Calcular(
            15000m, 4, new DateOnly(2026, 12, 10), true, BaseEntrada.Valor, 3000m, new DateOnly(2026, 11, 10));

        Assert.Equal(5, parcelas.Count);
        Assert.Equal(3000m, parcelas[0].Valor);
        Assert.True(parcelas[0].Entrada);
        Assert.All(parcelas.Skip(1), parcela => Assert.Equal(3000m, parcela.Valor));
        Assert.Equal(new DateOnly(2027, 3, 10), parcelas[4].Vencimento);
    }

    [Fact]
    public void Planilha_free_vira_gratuita_e_o_modelo_tem_11_colunas()
    {
        Assert.Equal(CustoConvidado.Gratuita, Rotulos.Custo("Free"));
        Assert.Equal(SituacaoConvidado.NaoComparecera, Rotulos.Situacao("Não comparecerá"));
        Assert.Equal("Cancelado", Rotulos.RecusaNoIcone(SituacaoConvidado.NaoComparecera));

        var planilha = new PlanilhaExcel();
        var bytes = planilha.Modelo();
        using var leitura = new MemoryStream(bytes);
        var linhas = planilha.Ler(leitura, "convidados.xlsx");

        Assert.True(linhas.Ok);
        Assert.Empty(linhas.Valor!);
        Assert.Equal(11, PlanilhaConvites.Colunas.Length);
        Assert.Equal("Nome do convite *", PlanilhaConvites.Colunas[0]);
        Assert.Equal("Mesa", PlanilhaConvites.Colunas[10]);
    }

    [Fact]
    public void Principal_precisa_de_telefone_do_mesmo_convite()
    {
        Assert.False(ConviteRegras.PodeSerPrincipal(false, "11999999999"));
        Assert.False(ConviteRegras.PodeSerPrincipal(true, " "));
        Assert.True(ConviteRegras.PodeSerPrincipal(true, "11999999999"));
    }

    [Fact]
    public void Qr_e_um_png()
    {
        var png = new GeradorQr().Png("http://localhost/rsvp/abc");

        Assert.True(png.Length > 8);
        Assert.Equal(0x89, png[0]);
        Assert.Equal(0x50, png[1]);
        Assert.Equal(0x4E, png[2]);
        Assert.Equal(0x47, png[3]);
    }
}
