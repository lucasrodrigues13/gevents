using Microsoft.AspNetCore.Identity;

namespace Project.Infrastructure.Conta;

/// <summary>
/// Conta Identity. <see cref="OrganizerId"/> vincula um <c>Client</c> ao Organizer dono do tenant (RF-084).
/// Admin e Organizer ficam sem vínculo.
/// </summary>
public sealed class ApplicationUser : IdentityUser
{
    public string? OrganizerId { get; set; }

    public void VincularAoOrganizer(string organizerId)
    {
        if (string.IsNullOrWhiteSpace(organizerId))
        {
            throw new InvalidOperationException("Conta Client exige vínculo com um Organizer.");
        }

        if (string.Equals(Id, organizerId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Conta Client não pode vincular a si mesma.");
        }

        OrganizerId = organizerId.Trim();
    }

    public void GarantirVinculoClient()
    {
        if (string.IsNullOrWhiteSpace(OrganizerId))
        {
            throw new InvalidOperationException("Conta Client exige vínculo com um Organizer.");
        }
    }
}
