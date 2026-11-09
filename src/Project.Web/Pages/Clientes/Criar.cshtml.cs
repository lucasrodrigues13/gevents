using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Application.Clientes;
using Project.Domain;
using Project.Infrastructure.Identity;
using Project.Web.Clientes;

namespace Project.Web.Pages.Clientes;

[Authorize(Roles = Roles.User)]
public sealed class CriarModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ClienteServico _clientes;

    public CriarModel(UserManager<IdentityUser> userManager, ClienteServico clientes)
    {
        _userManager = userManager;
        _clientes = clientes;
    }

    [BindProperty]
    public ClienteInput Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(Guid? eventoId, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var (conteudo, nome) = Input.AbrirImagem();
        try
        {
            await _clientes.CriarAsync(user.Id, Input.ParaDados(), conteudo, nome, eventoId, cancellationToken);
            return eventoId is null
                ? RedirectToPage("/Clientes/Index")
                : RedirectToPage("/Eventos/Home", new { id = eventoId });
        }
        catch (RegraDeNegocioException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return Page();
        }
        finally
        {
            if (conteudo is not null)
            {
                await conteudo.DisposeAsync();
            }
        }
    }
}
