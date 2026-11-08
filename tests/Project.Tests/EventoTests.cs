using Project.Domain;
using Project.Domain.Eventos;

namespace Project.Tests;

public class EventoTests
{
    [Fact]
    public void Nome_vazio_nao_cria()
    {
        var erro = Assert.Throws<RegraDeNegocioException>(() => Evento.Criar("conta", Dados("   ")));

        Assert.Equal("Informe o nome do evento.", erro.Message);
    }

    [Fact]
    public void Transicao_fora_de_andamento_e_recusada()
    {
        var concluido = Evento.Criar("conta", Dados("Ana"));
        concluido.Concluir();
        var cancelado = Evento.Criar("conta", Dados("Bruno"));
        cancelado.Cancelar();

        Assert.Equal("O evento não está em andamento.", Assert.Throws<RegraDeNegocioException>(concluido.Concluir).Message);
        Assert.Throws<RegraDeNegocioException>(concluido.Cancelar);
        Assert.Throws<RegraDeNegocioException>(() => cancelado.Editar(Dados("Outro")));
    }

    [Fact]
    public void Tipo_fora_da_lista_e_recusado()
    {
        Assert.False(TiposDeEvento.TentarLer("batizado", out _));
        Assert.False(TiposDeEvento.TentarLer("99", out _));
        Assert.True(TiposDeEvento.TentarLer("casamento", out var tipo));
        Assert.Equal(TipoEvento.Casamento, tipo);
        Assert.True(TiposDeEvento.TentarLer(" ", out var vazio));
        Assert.Null(vazio);
    }

    [Fact]
    public void Email_invalido_e_orcamento_negativo_sao_recusados()
    {
        Assert.Equal(
            "Informe um e-mail válido.",
            Assert.Throws<RegraDeNegocioException>(() => Evento.Criar("conta", Dados("Ana") with { CasalEmail = "sem-arroba" })).Message);
        Assert.Throws<RegraDeNegocioException>(() => Evento.Criar("conta", Dados("Ana") with { OrcamentoEstimado = -1 }));
        Assert.Throws<RegraDeNegocioException>(() => Evento.Criar("conta", Dados("Ana") with { ConvidadosEstimados = -1 }));
    }

    private static EventoDados Dados(string nome)
    {
        return new EventoDados(nome, null, null, null, null, null, null, null, null, null, null, null, null);
    }
}
