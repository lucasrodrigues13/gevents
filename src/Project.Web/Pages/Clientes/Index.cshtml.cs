using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Application.Clientes;
using Project.Domain;
using Project.Infrastructure.Identity;

namespace Project.Web.Pages.Clientes;

[Authorize(Roles = Roles.User)]
public sealed class IndexModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ClienteServico _clientes;

    public IndexModel(UserManager<IdentityUser> userManager, ClienteServico clientes)
    {
        _userManager = userManager;
        _clientes = clientes;
    }

    [BindProperty(SupportsGet = true)]
    public string? Nome { get; set; }

    public IReadOnlyList<ClienteItem> Clientes { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return;
        }

        Clientes = await _clientes.ListarAsync(user.Id, Nome, cancellationToken);
    }
}
