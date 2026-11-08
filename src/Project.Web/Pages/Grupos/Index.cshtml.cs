using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Application.Convites;
using Project.Domain;
using Project.Infrastructure.Identity;

namespace Project.Web.Pages.Grupos;

[Authorize(Roles = Roles.User)]
public sealed class IndexModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly GrupoServico _grupos;

    public IndexModel(UserManager<IdentityUser> userManager, GrupoServico grupos)
    {
        _userManager = userManager;
        _grupos = grupos;
    }

    [BindProperty]
    public string? Nome { get; set; }

    public IReadOnlyList<GrupoItem> Grupos { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await CarregarAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        try
        {
            await _grupos.CriarAsync(user.Id, Nome, cancellationToken);
            return RedirectToPage();
        }
        catch (RegraDeNegocioException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await CarregarAsync(cancellationToken);
            return Page();
        }
    }

    private async Task CarregarAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return;
        }

        Grupos = await _grupos.ListarAsync(user.Id, cancellationToken);
    }
}
