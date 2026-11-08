using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Application.Clientes;
using Project.Domain;
using Project.Infrastructure.Identity;

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
    public InputModel Input { get; set; } = new();

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

        try
        {
            await _clientes.CriarAsync(user.Id, Input.Nome, Input.Telefone, Input.Email, cancellationToken);
            return RedirectToPage("/Clientes/Index");
        }
        catch (RegraDeNegocioException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return Page();
        }
    }

    public sealed class InputModel
    {
        public string? Nome { get; set; }

        public string? Telefone { get; set; }

        public string? Email { get; set; }
    }
}
