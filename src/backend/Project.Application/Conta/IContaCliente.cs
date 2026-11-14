using Project.Application.Comum;

namespace Project.Application.Conta;

/// <summary>
/// Cria ou vincula conta Identity <c>Client</c> ao Organizer (RF-084 a RF-086).
/// A implementação fica na Infrastructure, atrás do UserManager.
/// </summary>
public interface IContaCliente
{
    Task<Resultado<string>> ConcederAsync(string organizerId, string email, CancellationToken ct);

    Task RevogarAsync(string usuarioId, CancellationToken ct);
}
