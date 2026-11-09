namespace Project.Web.Ui;

public sealed class ModalConfirmacao
{
    public required string Id { get; init; }

    public required string Titulo { get; init; }

    public required string Mensagem { get; init; }

    public required string Pagina { get; init; }

    public required string Handler { get; init; }

    public required Guid RegistroId { get; init; }

    public string Botao { get; init; } = "Confirmar";

    public bool Perigo { get; init; }

    public bool Aberto { get; init; }

    public string? Registro { get; init; }
}
