using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Application.Dashboards;
using Project.Infrastructure.Identity;

namespace Project.Web.Pages.Dashboards;

[Authorize(Roles = Roles.User)]
public sealed class IndexModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly DashboardServico _dashboard;

    public IndexModel(UserManager<IdentityUser> userManager, DashboardServico dashboard)
    {
        _userManager = userManager;
        _dashboard = dashboard;
    }

    public DashboardEventos? Eventos { get; private set; }

    public IReadOnlyList<CartaoFunil> Funil { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        Eventos = await _dashboard.ObterEventosAsync(user.Id, cancellationToken);
        Funil = _dashboard.ObterFunil();
        return Page();
    }
}
