using Project.Domain;
using Project.Domain.Convites;

namespace Project.Tests;

public class ConviteTests
{
    [Fact]
    public void Nome_vazio_nao_cria_grupo_convite_nem_convidado()
    {
        Assert.Equal("Informe o nome do grupo.", Assert.Throws<RegraDeNegocioException>(() => Grupo.Criar("conta", "  ")).Message);
        Assert.Equal("Informe o nome do convite.", Assert.Throws<RegraDeNegocioException>(() => Convite.Criar("conta", Guid.NewGuid(), "  ")).Message);
        Assert.Equal(
            "Informe o nome do convidado.",
            Assert.Throws<RegraDeNegocioException>(() => Convidado.Criar(Guid.NewGuid(), Dados("  "))).Message);
    }

    [Fact]
    public void Transfer_sem_origem_e_recusado()
    {
        var erro = Assert.Throws<RegraDeNegocioException>(() => Convidado.Criar(Guid.NewGuid(), Dados("Ana") with { Transfer = true }));

        Assert.Equal("Informe o local de origem do transfer.", erro.Message);
    }

    [Fact]
    public void Valor_fora_da_lista_e_presenca_pendente_sao_recusados()
    {
        Assert.False(ListasDeConvidado.TentarFaixa("bebe", out _));
        Assert.False(ListasDeConvidado.TentarCusto("vip", out _));
        Assert.False(ListasDeConvidado.TentarGenero("outro", out _));
        Assert.False(ListasDeConvidado.PresencaPublica("Pendente", out _));
        Assert.False(ListasDeConvidado.PresencaPublica("99", out _));

        var convidado = Convidado.Criar(Guid.NewGuid(), Dados("Ana"));
        Assert.Equal(
            "A presença não é aceita.",
            Assert.Throws<RegraDeNegocioException>(() => convidado.Responder(SituacaoConvidado.Pendente)).Message);
        Assert.Equal(SituacaoConvidado.Pendente, convidado.Situacao);
    }

    private static ConvidadoDados Dados(string nome)
    {
        return new ConvidadoDados(nome, null, null, null, null, null, null, false, null);
    }
}
