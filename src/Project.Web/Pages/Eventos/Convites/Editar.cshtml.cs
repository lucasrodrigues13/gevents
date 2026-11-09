using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Application.Convites;
using Project.Domain;
using Project.Infrastructure.Identity;

namespace Project.Web.Pages.Eventos.Convites;

[Authorize(Roles = Roles.User)]
public sealed class EditarModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ConviteServico _convites;

    public EditarModel(UserManager<IdentityUser> userManager, ConviteServico convites)
    {
        _userManager = userManager;
        _convites = convites;
    }

    public PainelConvites? Painel { get; private set; }

    public ConviteItem? Convite { get; private set; }

    public string? LinkWhatsApp { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid eventoId, Guid conviteId, CancellationToken cancellationToken)
    {
        return await CarregarAsync(eventoId, conviteId, cancellationToken) ?? Page();
    }

    public async Task<IActionResult> OnGetQrAsync(Guid eventoId, Guid conviteId, bool baixar, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var prefixo = $"{Request.Scheme}://{Request.Host}/Convite/";
        var png = await _convites.GerarQrAsync(user.Id, eventoId, conviteId, prefixo, cancellationToken);
        if (png is null)
        {
            return NotFound();
        }

        return baixar
            ? File(png, "image/png", "qrcode.png")
            : File(png, "image/png");
    }

    public async Task<IActionResult> OnPostTelefoneAsync(
        Guid eventoId,
        Guid conviteId,
        Guid convidadoId,
        string? ddi,
        string? celular,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        try
        {
            await _convites.DefinirTelefoneAsync(user.Id, eventoId, conviteId, convidadoId, ddi, celular, cancellationToken);
            return RedirectToPage(new { eventoId, conviteId });
        }
        catch (RegraDeNegocioException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return await CarregarAsync(eventoId, conviteId, cancellationToken) ?? Page();
        }
    }

    public async Task<IActionResult> OnPostWhatsAppAsync(
        Guid eventoId,
        Guid conviteId,
        string? ddi,
        string? celular,
        string? mensagem,
        CancellationToken cancellationToken)
    {
        try
        {
            LinkWhatsApp = ConviteServico.LinkWhatsApp(ddi, celular, mensagem);
        }
        catch (RegraDeNegocioException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
        }

        return await CarregarAsync(eventoId, conviteId, cancellationToken) ?? Page();
    }

    private async Task<IActionResult?> CarregarAsync(Guid eventoId, Guid conviteId, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        Painel = await _convites.ObterPainelAsync(
            user.Id,
            eventoId,
            new FiltroConvidados(null, null, null, null, null, null),
            cancellationToken);
        Convite = Painel?.Convites.FirstOrDefault(item => item.Id == conviteId);
        if (Painel is null || Convite is null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
        }

        return null;
    }
}
