using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Application.Eventos;
using Project.Domain;
using Project.Domain.Eventos;
using Project.Infrastructure.Identity;

namespace Project.Web.Pages.Modelos;

[Authorize(Roles = Roles.User)]
public sealed class CriarModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ModeloServico _servico;

    public CriarModel(UserManager<IdentityUser> userManager, ModeloServico servico)
    {
        _userManager = userManager;
        _servico = servico;
    }

    [BindProperty]
    public string? Nome { get; set; }

    [BindProperty]
    public string? Tipo { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        if (!TiposDeEvento.TentarLer(Tipo, out var tipo) || tipo is null)
        {
            ModelState.AddModelError(string.Empty, "O tipo do modelo não é aceito.");
            return Page();
        }

        try
        {
            var id = await _servico.CriarAsync(user.Id, Nome, tipo, cancellationToken);
            return RedirectToPage("/Modelos/Editar", new { id });
        }
        catch (RegraDeNegocioException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return Page();
        }
    }
}
