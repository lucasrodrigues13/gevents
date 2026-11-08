using System.Net.Mail;

namespace Project.Domain.Clientes;

public sealed class Cliente
{
    private Cliente()
    {
    }

    public Guid Id { get; private set; }

    public string AccountId { get; private set; } = "";

    public string Nome { get; private set; } = "";

    public string? Telefone { get; private set; }

    public string? Email { get; private set; }

    public static Cliente Criar(string accountId, string? nome, string? telefone, string? email)
    {
        if (string.IsNullOrWhiteSpace(accountId))
        {
            throw new RegraDeNegocioException("O cliente precisa de uma conta.");
        }

        var cliente = new Cliente
        {
            Id = Guid.NewGuid(),
            AccountId = accountId
        };
        cliente.Aplicar(nome, telefone, email);
        return cliente;
    }

    public void Editar(string? nome, string? telefone, string? email)
    {
        Aplicar(nome, telefone, email);
    }

    private void Aplicar(string? nome, string? telefone, string? email)
    {
        var texto = nome?.Trim() ?? "";
        if (texto.Length == 0)
        {
            throw new RegraDeNegocioException("Informe o nome do cliente.");
        }

        var endereco = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        if (endereco is not null && !EmailValido(endereco))
        {
            throw new RegraDeNegocioException("Informe um e-mail válido.");
        }

        Nome = texto;
        Telefone = string.IsNullOrWhiteSpace(telefone) ? null : telefone.Trim();
        Email = endereco;
    }

    private static bool EmailValido(string email)
    {
        return MailAddress.TryCreate(email, out var endereco)
            && endereco.Address.Equals(email, StringComparison.OrdinalIgnoreCase);
    }
}

public sealed class VinculoCliente
{
    private VinculoCliente()
    {
    }

    public Guid Id { get; private set; }

    public string AccountId { get; private set; } = "";

    public Guid ClienteId { get; private set; }

    public Guid EventoId { get; private set; }

    public static VinculoCliente Criar(string accountId, Guid clienteId, Guid eventoId)
    {
        if (string.IsNullOrWhiteSpace(accountId) || clienteId == Guid.Empty || eventoId == Guid.Empty)
        {
            throw new RegraDeNegocioException("O vínculo precisa da conta, do cliente e do evento.");
        }

        return new VinculoCliente
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            ClienteId = clienteId,
            EventoId = eventoId
        };
    }
}
