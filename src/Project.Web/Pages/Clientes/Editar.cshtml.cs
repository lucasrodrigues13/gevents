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
public sealed class EditarModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ClienteServico _clientes;

    public EditarModel(UserManager<IdentityUser> userManager, ClienteServico clientes)
    {
        _userManager = userManager;
        _clientes = clientes;
    }

    [BindProperty]
    public ClienteInput Input { get; set; } = new();

    public bool Disponivel { get; private set; }

    public IReadOnlyList<EventoVinculado> Eventos { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!await CarregarAsync(id, preencher: true, cancellationToken))
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var (conteudo, nome) = Input.AbrirImagem();
        try
        {
            await _clientes.EditarAsync(user.Id, id, Input.ParaDados(), conteudo, nome, cancellationToken);
            return RedirectToPage("/Clientes/Index");
        }
        catch (RegraDeNegocioException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            if (!await CarregarAsync(id, preencher: false, cancellationToken))
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
            }

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

    private async Task<bool> CarregarAsync(Guid id, bool preencher, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return false;
        }

        var cliente = await _clientes.ObterAsync(user.Id, id, cancellationToken);
        if (cliente is null)
        {
            return false;
        }

        var lista = await _clientes.ListarAsync(user.Id, null, cancellationToken);
        Disponivel = true;
        Eventos = lista.FirstOrDefault(item => item.Id == id)?.Eventos ?? [];
        if (preencher)
        {
            Input = ClienteInput.De(cliente);
        }

        return true;
    }
}
