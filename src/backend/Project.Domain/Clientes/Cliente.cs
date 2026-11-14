namespace Project.Domain.Clientes;

public sealed class Cliente
{
    public Guid Id { get; set; }

    public string ContaId { get; set; } = "";

    public string Nome { get; set; } = "";

    public PapelCliente Papel { get; set; }

    public DateOnly? Nascimento { get; set; }

    public string Email { get; set; } = "";

    public string Ddi { get; set; } = "+55";

    public string Celular { get; set; } = "";

    public string? TelefoneFixo { get; set; }

    public string? Cpf { get; set; }

    public string? Rg { get; set; }

    public string? Cnpj { get; set; }

    public string? Profissao { get; set; }

    public string? InscricaoMunicipal { get; set; }

    public string? Religiao { get; set; }

    public string? Observacao { get; set; }

    public string? Facebook { get; set; }

    public string? Instagram { get; set; }

    public string? Pais { get; set; }

    public string? Cep { get; set; }

    public string? Endereco { get; set; }

    public string? Numero { get; set; }

    public string? Estado { get; set; }

    public string? Cidade { get; set; }

    public string? Bairro { get; set; }

    public string? Complemento { get; set; }

    public Guid? FotoId { get; set; }

    /// <summary>Id do usuário Identity com role Client, quando o Organizer concede acesso (RF-056, RF-084).</summary>
    public string? UsuarioId { get; set; }

    public bool AcessoPlataforma { get; set; }

    public bool PermiteConvidados { get; set; }

    public bool PermiteTarefas { get; set; }

    public bool PermiteAnexos { get; set; }

    public bool PermiteParcelas { get; set; }

    public bool PermiteRsvp { get; set; }

    public bool PermiteMapa { get; set; }

    public bool PermiteContrato { get; set; }

    public List<VinculoCliente> Vinculos { get; set; } = [];

    public bool TemPermissao(ModuloCliente modulo) =>
        AcessoPlataforma && modulo switch
        {
            ModuloCliente.Convidados => PermiteConvidados,
            ModuloCliente.Tarefas => PermiteTarefas,
            ModuloCliente.Anexos => PermiteAnexos,
            ModuloCliente.Parcelas => PermiteParcelas,
            ModuloCliente.Rsvp => PermiteRsvp,
            ModuloCliente.Mapa => PermiteMapa,
            ModuloCliente.Contrato => PermiteContrato,
            _ => false,
        };

    public void AplicarPermissoes(
        bool convidados,
        bool tarefas,
        bool anexos,
        bool parcelas,
        bool rsvp,
        bool mapa,
        bool contrato)
    {
        PermiteConvidados = convidados;
        PermiteTarefas = tarefas;
        PermiteAnexos = anexos;
        PermiteParcelas = parcelas;
        PermiteRsvp = rsvp;
        PermiteMapa = mapa;
        PermiteContrato = contrato;
    }
}

public sealed class VinculoCliente
{
    public Guid ClienteId { get; set; }

    public Guid EventoId { get; set; }
}
