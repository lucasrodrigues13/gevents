using Project.Domain;
using Project.Domain.Tarefas;

namespace Project.Tests;

public class TarefaTests
{
    [Fact]
    public void Titulo_vazio_nao_cria()
    {
        var erro = Assert.Throws<RegraDeNegocioException>(() => Tarefa.Criar("conta", Guid.NewGuid(), "  ", null, null));

        Assert.Equal("Informe o título da tarefa.", erro.Message);
    }

    [Fact]
    public void Situacao_fora_das_quatro_e_recusada()
    {
        var tarefa = Tarefa.Criar("conta", Guid.NewGuid(), "Levar documento", null, null);

        Assert.False(SituacoesDeTarefa.TentarLer("feita", out _));
        Assert.False(SituacoesDeTarefa.TentarLer("99", out _));
        Assert.Equal(
            "A situação da tarefa não é aceita.",
            Assert.Throws<RegraDeNegocioException>(() => tarefa.MudarSituacao((SituacaoTarefa)99)).Message);
        Assert.Equal(SituacaoTarefa.Pendente, tarefa.Situacao);
    }
}
