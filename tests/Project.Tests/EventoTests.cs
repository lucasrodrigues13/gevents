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

    [Fact]
    public void Sem_modelo_nao_cria()
    {
        var erro = Assert.Throws<RegraDeNegocioException>(() => Evento.CriarComModelo("conta", null, "Ana", null, Dados("Ana")));

        Assert.Equal("Escolha um modelo.", erro.Message);
    }

    [Fact]
    public void Casamento_sem_um_dos_nomes_nao_cria_e_os_dois_juntam_com_e()
    {
        var modelo = Modelo.Criar("conta", "Checklist", TipoEvento.Casamento);

        Assert.Equal("Informe os dois nomes.", Assert.Throws<RegraDeNegocioException>(() => Evento.CriarComModelo("conta", modelo, "Ana", " ", Dados("Ana"))).Message);
        var evento = Evento.CriarComModelo("conta", modelo, "Ana", "Bia", Dados("Ana"));
        Assert.Equal("Ana & Bia", evento.Nome);
    }

    [Fact]
    public void Valor_nao_positivo_e_quantidade_invalida_nao_geram_parcela()
    {
        Assert.Equal("Informe um valor maior que zero.", Assert.Throws<RegraDeNegocioException>(() => ParcelaEvento.Gerar("conta", Guid.NewGuid(), Guid.NewGuid(), 0, 1, new DateOnly(2026, 1, 1), null, null, null)).Message);
        Assert.Equal("Informe a quantidade de parcelas.", Assert.Throws<RegraDeNegocioException>(() => ParcelaEvento.Gerar("conta", Guid.NewGuid(), Guid.NewGuid(), 10, 0, new DateOnly(2026, 1, 1), null, null, null)).Message);
    }

    private static EventoDados Dados(string nome)
    {
        return new EventoDados(nome, null, null, null, null, null, null, null, null, null, null, null, null);
    }
}
