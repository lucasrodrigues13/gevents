using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Application.Tarefas;
using Project.Infrastructure.Identity;

namespace Project.Web.Pages.Tarefas;

[Authorize(Roles = Roles.User)]
public sealed class IndexModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly TarefaServico _tarefas;

    public IndexModel(UserManager<IdentityUser> userManager, TarefaServico tarefas)
    {
        _userManager = userManager;
        _tarefas = tarefas;
    }

    public IReadOnlyList<TarefaGeral> Tarefas { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return;
        }

        Tarefas = await _tarefas.ListarPorContaAsync(user.Id, cancellationToken);
    }
}
