namespace Project.Domain.Tarefas;

public sealed class Tarefa
{
    public Guid Id { get; set; }

    public Guid EventoId { get; set; }

    public string ContaId { get; set; } = "";

    public string Titulo { get; set; } = "";

    public string? Descricao { get; set; }

    public DateOnly? Prazo { get; set; }

    public SituacaoTarefa Situacao { get; set; } = SituacaoTarefa.Pendente;

    public string ResponsavelContaId { get; set; } = "";
}
