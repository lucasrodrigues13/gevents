namespace Project.Domain.Tarefas;

public static class SituacoesDeTarefa
{
    public static bool TentarLer(string? texto, out SituacaoTarefa situacao)
    {
        if (Enum.TryParse<SituacaoTarefa>(texto, ignoreCase: true, out var lida) && Enum.IsDefined(lida))
        {
            situacao = lida;
            return true;
        }

        situacao = default;
        return false;
    }
}
