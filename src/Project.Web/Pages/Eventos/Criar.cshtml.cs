using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Application.Eventos;
using Project.Domain;
using Project.Infrastructure.Identity;
using Project.Web.Eventos;

namespace Project.Web.Pages.Eventos;

[Authorize(Roles = Roles.User)]
public sealed class CriarModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly EventoServico _servico;

    public CriarModel(UserManager<IdentityUser> userManager, EventoServico servico)
    {
        _userManager = userManager;
        _servico = servico;
    }

    [BindProperty]
    public EventoInput Input { get; set; } = new();

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

        var tipoOk = EventoFormulario.TentarCriarDados(Input, out var dados, out var erro);
        if (!tipoOk)
        {
            ModelState.AddModelError(string.Empty, erro!);
        }

        if (!ModelState.IsValid || !tipoOk)
        {
            return Page();
        }

        var (conteudo, nome) = Input.AbrirImagem();
        try
        {
            var gravado = await _servico.CriarAsync(user.Id, dados, conteudo, nome, cancellationToken);
            return RedirectToPage("/Eventos/Home", new { id = gravado.Id, aviso = gravado.AvisoImagem });
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
