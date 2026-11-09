using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Application.Clientes;
using Project.Domain;
using Project.Web.Clientes;

namespace Project.Web.Pages.Cadastro;

[AllowAnonymous]
public sealed class IndexModel : PageModel
{
    private readonly ClienteServico _clientes;

    public IndexModel(ClienteServico clientes)
    {
        _clientes = clientes;
    }

    [BindProperty]
    public ClienteInput Input { get; set; } = new();

    public bool Disponivel { get; private set; } = true;

    public bool Gravado { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!await _clientes.LinkExisteAsync(id, cancellationToken))
        {
            Disponivel = false;
            Response.StatusCode = StatusCodes.Status404NotFound;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        var (conteudo, nome) = Input.AbrirImagem();
        try
        {
            await _clientes.CadastrarPeloLinkAsync(id, Input.ParaDados(), conteudo, nome, cancellationToken);
            Gravado = true;
            return Page();
        }
        catch (RegraDeNegocioException exception)
        {
            if (exception.Message == "O link não está disponível.")
            {
                Disponivel = false;
                Response.StatusCode = StatusCodes.Status404NotFound;
            }

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
