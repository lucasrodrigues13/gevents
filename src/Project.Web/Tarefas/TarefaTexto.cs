using Project.Domain.Tarefas;

namespace Project.Web.Tarefas;

public static class TarefaTexto
{
    public static string Situacao(SituacaoTarefa situacao)
    {
        return situacao switch
        {
            SituacaoTarefa.Pendente => "Pendente",
            SituacaoTarefa.EmAndamento => "Em andamento",
            SituacaoTarefa.Concluida => "Concluída",
            SituacaoTarefa.Atrasada => "Atrasada",
            _ => throw new InvalidOperationException("Situação desconhecida.")
        };
    }

    public static IReadOnlyList<SituacaoTarefa> Colunas { get; } =
    [
        SituacaoTarefa.Pendente,
        SituacaoTarefa.EmAndamento,
        SituacaoTarefa.Concluida,
        SituacaoTarefa.Atrasada
    ];
}
