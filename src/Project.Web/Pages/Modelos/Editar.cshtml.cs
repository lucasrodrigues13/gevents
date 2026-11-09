using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Application.Eventos;
using Project.Domain;
using Project.Infrastructure.Identity;

namespace Project.Web.Pages.Modelos;

[Authorize(Roles = Roles.User)]
public sealed class EditarModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ModeloServico _servico;

    public EditarModel(UserManager<IdentityUser> userManager, ModeloServico servico)
    {
        _userManager = userManager;
        _servico = servico;
    }

    [BindProperty]
    public string? NomeServico { get; set; }

    [BindProperty]
    public Guid? ServicoId { get; set; }

    [BindProperty]
    public string? TituloTarefa { get; set; }

    public ModeloDetalhe? Detalhe { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        Detalhe = await CarregarAsync(id, cancellationToken);
        if (Detalhe is null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostServicoAsync(Guid id, CancellationToken cancellationToken)
    {
        return await GravarAsync(id, (accountId, token) => _servico.AdicionarServicoAsync(accountId, id, NomeServico, token), cancellationToken);
    }

    public async Task<IActionResult> OnPostTarefaAsync(Guid id, CancellationToken cancellationToken)
    {
        return await GravarAsync(
            id,
            (accountId, token) => _servico.AdicionarTarefaAsync(accountId, id, ServicoId ?? Guid.Empty, TituloTarefa, token),
            cancellationToken);
    }

    private async Task<IActionResult> GravarAsync(Guid id, Func<string, CancellationToken, Task> acao, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        try
        {
            await acao(user.Id, cancellationToken);
            return RedirectToPage(new { id });
        }
        catch (RegraDeNegocioException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            Detalhe = await CarregarAsync(id, cancellationToken);
            if (Detalhe is null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
            }

            return Page();
        }
    }

    private async Task<ModeloDetalhe?> CarregarAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        return user is null ? null : await _servico.ObterAsync(user.Id, id, cancellationToken);
    }
}
