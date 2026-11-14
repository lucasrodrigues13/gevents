namespace Project.Domain.Modelos;

public sealed class ModeloMensagem
{
    public Guid Id { get; set; }

    public string ContaId { get; set; } = "";

    public string Nome { get; set; } = "";

    public string Texto { get; set; } = "";
}

public static class ParametrosMensagem
{
    public static readonly string[] Todos =
    [
        "#nome_da_empresa",
        "#nome_evento",
        "#tipo_evento",
        "#data_evento",
        "#hora_evento",
        "#local_cerimonia",
        "#local_recepcao",
        "#endereco_cerimonia",
        "#endereco_recepcao",
        "#convite",
        "#link_convite",
        "#link_unico",
        "#nome_convidados",
        "#mesa_dos_convidados",
        "#site_url",
        "#prazo_rsvp",
        "#qr_code",
    ];
}
