using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Project.Infrastructure.Identity;

public sealed class AdminBootstrapper
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly BootstrapOptions _options;
    private readonly ILogger<AdminBootstrapper> _logger;

    public AdminBootstrapper(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<BootstrapOptions> options,
        ILogger<AdminBootstrapper> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _options = options.Value;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_options.Email) || string.IsNullOrWhiteSpace(_options.Password))
        {
            throw new InvalidOperationException("Bootstrap habilitado sem credencial.");
        }

        cancellationToken.ThrowIfCancellationRequested();
        await EnsureRoleAsync(Roles.Admin);
        await EnsureRoleAsync(Roles.User);

        var user = await _userManager.FindByEmailAsync(_options.Email);
        if (user is null)
        {
            user = new IdentityUser
            {
                UserName = _options.Email,
                Email = _options.Email,
                EmailConfirmed = true
            };

            var created = await _userManager.CreateAsync(user, _options.Password);
            if (!created.Succeeded)
            {
                _logger.LogError("Bootstrap não criou o administrador.");
                throw new InvalidOperationException("Bootstrap não criou o administrador.");
            }

            var assigned = await _userManager.AddToRoleAsync(user, Roles.Admin);
            if (!assigned.Succeeded)
            {
                _logger.LogError("Bootstrap não atribuiu a role de administrador.");
                throw new InvalidOperationException("Bootstrap não atribuiu a role de administrador.");
            }

            return;
        }

        if (!await _userManager.IsInRoleAsync(user, Roles.Admin))
        {
            _logger.LogError("Bootstrap interrompido porque a conta informada já existe e não é administradora.");
            throw new InvalidOperationException("Bootstrap interrompido: a conta informada já existe e não é administradora.");
        }
    }

    private async Task EnsureRoleAsync(string role)
    {
        if (await _roleManager.RoleExistsAsync(role))
        {
            return;
        }

        var created = await _roleManager.CreateAsync(new IdentityRole(role));
        if (!created.Succeeded)
        {
            _logger.LogError("Bootstrap não criou uma role.");
            throw new InvalidOperationException("Bootstrap não criou uma role.");
        }
    }
}
