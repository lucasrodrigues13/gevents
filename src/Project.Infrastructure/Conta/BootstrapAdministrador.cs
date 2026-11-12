using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Project.Application.Conta;

namespace Project.Infrastructure.Conta;

public sealed class GarantiaDePapeis
{
    private readonly RoleManager<IdentityRole> _roles;

    public GarantiaDePapeis(RoleManager<IdentityRole> roles)
    {
        _roles = roles;
    }

    public async Task GarantirAsync()
    {
        foreach (var papel in new[] { Papeis.Admin, Papeis.User })
        {
            if (!await _roles.RoleExistsAsync(papel))
            {
                var criado = await _roles.CreateAsync(new IdentityRole(papel));
                if (!criado.Succeeded)
                {
                    throw new InvalidOperationException("Não foi possível preparar os papéis da conta.");
                }
            }
        }
    }
}

public sealed class BootstrapAdministrador
{
    private readonly UserManager<IdentityUser> _usuarios;
    private readonly BootstrapOpcoes _opcoes;

    public BootstrapAdministrador(UserManager<IdentityUser> usuarios, IOptions<BootstrapOpcoes> opcoes)
    {
        _usuarios = usuarios;
        _opcoes = opcoes.Value;
    }

    public async Task ExecutarAsync()
    {
        if (!_opcoes.Enabled)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_opcoes.AdminEmail) || string.IsNullOrWhiteSpace(_opcoes.AdminPassword))
        {
            throw new InvalidOperationException("Bootstrap interrompido: credencial ausente.");
        }

        var existente = await _usuarios.FindByEmailAsync(_opcoes.AdminEmail);
        if (existente is not null)
        {
            if (await _usuarios.IsInRoleAsync(existente, Papeis.Admin))
            {
                return;
            }

            throw new InvalidOperationException("Bootstrap interrompido: o e-mail já pertence a um usuário comum.");
        }

        var usuario = new IdentityUser
        {
            UserName = _opcoes.AdminEmail.Trim(),
            Email = _opcoes.AdminEmail.Trim(),
            EmailConfirmed = true,
        };
        var criado = await _usuarios.CreateAsync(usuario, _opcoes.AdminPassword);
        if (!criado.Succeeded)
        {
            throw new InvalidOperationException("Bootstrap interrompido: credencial inválida.");
        }

        var papel = await _usuarios.AddToRoleAsync(usuario, Papeis.Admin);
        if (!papel.Succeeded)
        {
            throw new InvalidOperationException("Bootstrap interrompido: não foi possível atribuir o papel.");
        }
    }
}
