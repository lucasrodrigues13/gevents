using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Project.Application.Comum;
using Project.Application.Conta;

namespace Project.Infrastructure.Conta;

public sealed class ContaCliente(
    UserManager<ApplicationUser> usuarios,
    IEmailSender<ApplicationUser> email,
    IConfiguration configuracao) : IContaCliente
{
    public async Task<Resultado<string>> ConcederAsync(string organizerId, string emailContato, CancellationToken ct)
    {
        var endereco = emailContato.Trim();
        var existente = await usuarios.FindByEmailAsync(endereco);
        if (existente is not null)
        {
            var papeis = await usuarios.GetRolesAsync(existente);
            if (papeis.Contains(Papeis.Admin) || papeis.Contains(Papeis.Organizer))
            {
                return Resultado<string>.Erro(Codigos.Validacao, "Este e-mail já pertence a outra conta.");
            }

            if (papeis.Contains(Papeis.Client))
            {
                if (!string.Equals(existente.OrganizerId, organizerId, StringComparison.Ordinal))
                {
                    return Resultado<string>.Erro(Codigos.Validacao, "Este e-mail já pertence a outra conta.");
                }

                await TentarEnviarPrimeiroAcessoAsync(existente, endereco);
                return Resultado<string>.Sucesso(existente.Id);
            }

            return Resultado<string>.Erro(Codigos.Validacao, "Este e-mail já pertence a outra conta.");
        }

        var usuario = new ApplicationUser
        {
            UserName = endereco,
            Email = endereco,
            EmailConfirmed = true,
        };
        usuario.VincularAoOrganizer(organizerId);

        var senha = SenhaTemporaria();
        var criado = await usuarios.CreateAsync(usuario, senha);
        if (!criado.Succeeded)
        {
            if (criado.Errors.Any(erro => erro.Code is "DuplicateUserName" or "DuplicateEmail"))
            {
                return Resultado<string>.Erro(Codigos.Validacao, "Este e-mail já pertence a outra conta.");
            }

            return Resultado<string>.Erro(Codigos.Validacao, "Não foi possível criar o acesso do cliente.");
        }

        var papel = await usuarios.AddToRoleAsync(usuario, Papeis.Client);
        if (!papel.Succeeded)
        {
            await usuarios.DeleteAsync(usuario);
            return Resultado<string>.Erro(Codigos.Validacao, "Não foi possível criar o acesso do cliente.");
        }

        await TentarEnviarPrimeiroAcessoAsync(usuario, endereco);
        return Resultado<string>.Sucesso(usuario.Id);
    }

    public async Task RevogarAsync(string usuarioId, CancellationToken ct)
    {
        var usuario = await usuarios.FindByIdAsync(usuarioId);
        if (usuario is null)
        {
            return;
        }

        if (await usuarios.IsInRoleAsync(usuario, Papeis.Client))
        {
            await usuarios.RemoveFromRoleAsync(usuario, Papeis.Client);
        }

        usuario.OrganizerId = null;
        await usuarios.UpdateAsync(usuario);
        await usuarios.SetLockoutEnabledAsync(usuario, true);
        await usuarios.SetLockoutEndDateAsync(usuario, DateTimeOffset.UtcNow.AddYears(100));
    }

    private async Task TentarEnviarPrimeiroAcessoAsync(ApplicationUser usuario, string endereco)
    {
        try
        {
            var token = await usuarios.GeneratePasswordResetTokenAsync(usuario);
            var publico = (configuracao["PublicUrl"] ?? "").TrimEnd('/');
            var url = $"{publico}/redefinir-senha?usuarioId={Uri.EscapeDataString(usuario.Id)}&token={Uri.EscapeDataString(token)}";
            await email.SendPasswordResetLinkAsync(usuario, endereco, url);
        }
        catch
        {
            // Conta Client já criada; o disparo de primeiro acesso não bloqueia o vínculo (SPEC-009).
        }
    }

    private static string SenhaTemporaria()
    {
        var bytes = RandomNumberGenerator.GetBytes(24);
        return Convert.ToBase64String(bytes) + "aA";
    }
}
