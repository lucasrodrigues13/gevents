using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Application.Convites;
using Project.Domain;

namespace Project.Web.Pages.Convite;

[AllowAnonymous]
public sealed class IndexModel : PageModel
{
    private readonly ConviteServico _convites;

    public IndexModel(ConviteServico convites)
    {
        _convites = convites;
    }

    public ConvitePublico? Convite { get; private set; }

    public string? Aviso { get; private set; }

    public async Task OnGetAsync(Guid link, CancellationToken cancellationToken)
    {
        Convite = await _convites.AbrirAsync(link, DateTime.UtcNow, cancellationToken);
        if (Convite is null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
        }
    }

    public async Task<IActionResult> OnPostAsync(Guid link, List<RespostaForm> respostas, CancellationToken cancellationToken)
    {
        try
        {
            Aviso = await _convites.ResponderAsync(
                link,
                respostas.Select(item => (item.ConvidadoId, item.Situacao)).ToArray(),
                DateTime.UtcNow,
                cancellationToken);
            if (Aviso is null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return Page();
            }
        }
        catch (RegraDeNegocioException exception)
        {
            Aviso = exception.Message;
        }

        Convite = await _convites.ObterPublicoAsync(link, cancellationToken);
        return Page();
    }

    public sealed class RespostaForm
    {
        public Guid ConvidadoId { get; set; }

        public string? Situacao { get; set; }
    }
}
