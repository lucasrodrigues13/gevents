using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Application.Eventos;
using Project.Infrastructure.Identity;

namespace Project.Web.Pages.Eventos;

[Authorize(Roles = Roles.User)]
public sealed class ImagemModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly EventoServico _servico;

    public ImagemModel(UserManager<IdentityUser> userManager, EventoServico servico)
    {
        _userManager = userManager;
        _servico = servico;
    }

    public bool EventoDaConta { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var detalhe = await _servico.ObterAsync(user.Id, id, cancellationToken);
        if (detalhe is null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return Page();
        }

        EventoDaConta = true;
        var arquivo = await _servico.AbrirImagemAsync(user.Id, id, cancellationToken);
        if (arquivo is null)
        {
            return Page();
        }

        Response.Headers.XContentTypeOptions = "nosniff";
        return File(arquivo.Conteudo, arquivo.Mime, arquivo.NomeOriginal);
    }
}
