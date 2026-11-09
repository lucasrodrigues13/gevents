using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Application.Eventos;
using Project.Infrastructure.Identity;

namespace Project.Web.Pages.Modelos;

[Authorize(Roles = Roles.User)]
public sealed class IndexModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ModeloServico _servico;

    public IndexModel(UserManager<IdentityUser> userManager, ModeloServico servico)
    {
        _userManager = userManager;
        _servico = servico;
    }

    public IReadOnlyList<ModeloResumo> Modelos { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return;
        }

        Modelos = await _servico.ListarAsync(user.Id, cancellationToken);
    }
}
