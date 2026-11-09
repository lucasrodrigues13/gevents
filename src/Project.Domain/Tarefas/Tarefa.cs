namespace Project.Domain.Tarefas;

public sealed class Tarefa
{
    private Tarefa()
    {
    }

    public Guid Id { get; private set; }

    public Guid EventoId { get; private set; }

    public string AccountId { get; private set; } = "";

    public string Titulo { get; private set; } = "";

    public string? Descricao { get; private set; }

    public DateOnly? Prazo { get; private set; }

    public SituacaoTarefa Situacao { get; private set; }

    public Guid? ServicoEventoId { get; private set; }

    public void VincularServico(Guid servicoId)
    {
        ServicoEventoId = servicoId;
    }

    public static Tarefa Criar(string accountId, Guid eventoId, string? titulo, string? descricao, DateOnly? prazo)
    {
        if (string.IsNullOrWhiteSpace(accountId) || eventoId == Guid.Empty)
        {
            throw new RegraDeNegocioException("A tarefa precisa de um evento da conta.");
        }

        var tarefa = new Tarefa
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            EventoId = eventoId,
            Situacao = SituacaoTarefa.Pendente
        };
        tarefa.Aplicar(titulo, descricao, prazo);
        return tarefa;
    }

    public void Editar(string? titulo, string? descricao, DateOnly? prazo)
    {
        Aplicar(titulo, descricao, prazo);
    }

    public void MudarSituacao(SituacaoTarefa situacao)
    {
        if (!Enum.IsDefined(situacao))
        {
            throw new RegraDeNegocioException("A situação da tarefa não é aceita.");
        }

        Situacao = situacao;
    }

    private void Aplicar(string? titulo, string? descricao, DateOnly? prazo)
    {
        var texto = titulo?.Trim() ?? "";
        if (texto.Length == 0)
        {
            throw new RegraDeNegocioException("Informe o título da tarefa.");
        }

        Titulo = texto;
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        Prazo = prazo;
    }
}
