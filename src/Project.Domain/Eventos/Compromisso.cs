namespace Project.Domain.Eventos;

public sealed class Compromisso
{
    private Compromisso()
    {
    }

    public Guid Id { get; private set; }

    public string AccountId { get; private set; } = "";

    public Guid EventoId { get; private set; }

    public string Nome { get; private set; } = "";

    public DateOnly Data { get; private set; }

    public TimeOnly? Hora { get; private set; }

    public static Compromisso DoEvento(string accountId, Guid eventoId, string nome, DateOnly data, TimeOnly? hora)
    {
        if (string.IsNullOrWhiteSpace(accountId) || eventoId == Guid.Empty || string.IsNullOrWhiteSpace(nome))
        {
            throw new RegraDeNegocioException("O compromisso precisa do evento.");
        }

        return new Compromisso
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            EventoId = eventoId,
            Nome = nome.Trim(),
            Data = data,
            Hora = hora
        };
    }
}
