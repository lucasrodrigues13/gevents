using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Application.Convites;
using Project.Domain;
using Project.Domain.Convites;
using Project.Domain.Eventos;
using Project.Infrastructure.Identity;

namespace Project.Web.Pages.Eventos.Convites;

[Authorize(Roles = Roles.User)]
public sealed class IndexModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ConviteServico _convites;

    public IndexModel(UserManager<IdentityUser> userManager, ConviteServico convites)
    {
        _userManager = userManager;
        _convites = convites;
    }

    [BindProperty(SupportsGet = true)]
    public string Visao { get; set; } = "convites";

    [BindProperty(SupportsGet = true)]
    public string? Nome { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Situacao { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Celular { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Transfer { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid? Grupo { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Observacao { get; set; }

    public PainelConvites? Painel { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await CarregarAsync(id, cancellationToken) ?? Page();
    }

    public async Task<IActionResult> OnPostConviteAsync(Guid id, string? nomeConvite, CancellationToken cancellationToken)
    {
        var user = await ExigirUsuarioAsync();
        if (user is null)
        {
            return Challenge();
        }

        try
        {
            await _convites.CriarConviteAsync(user.Id, id, nomeConvite, cancellationToken);
            return RedirectToPage(new { id });
        }
        catch (RegraDeNegocioException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return await CarregarAsync(id, cancellationToken) ?? Page();
        }
    }

    public async Task<IActionResult> OnPostConvidadoAsync(Guid id, ConvidadoForm input, CancellationToken cancellationToken)
    {
        var user = await ExigirUsuarioAsync();
        if (user is null)
        {
            return Challenge();
        }

        if (!ListasDeConvidado.TentarFaixa(input.Faixa, out var faixa)
            || !ListasDeConvidado.TentarCusto(input.Custo, out var custo)
            || !ListasDeConvidado.TentarGenero(input.Genero, out var genero))
        {
            ModelState.AddModelError(string.Empty, "O valor informado não é aceito.");
            return await CarregarAsync(id, cancellationToken) ?? Page();
        }

        try
        {
            await _convites.CriarConvidadoAsync(
                user.Id,
                id,
                input.ConviteId,
                new ConvidadoDados(input.Nome, faixa, custo, genero, input.Celular, input.Observacao, input.GrupoId, input.Transfer, input.Origem),
                cancellationToken);
            return RedirectToPage(new { id });
        }
        catch (RegraDeNegocioException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return await CarregarAsync(id, cancellationToken) ?? Page();
        }
    }

    private async Task<IActionResult?> CarregarAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await ExigirUsuarioAsync();
        if (user is null)
        {
            return Challenge();
        }

        if (!ListasDeConvidado.TentarSituacao(Situacao, out var situacao))
        {
            ModelState.AddModelError(string.Empty, "A situação não é aceita.");
            situacao = null;
        }

        bool? transfer = Transfer switch
        {
            "sim" => true,
            "nao" => false,
            _ => null
        };
        Painel = await _convites.ObterPainelAsync(
            user.Id,
            id,
            new FiltroConvidados(Nome, situacao, Celular, transfer, Grupo, Observacao),
            cancellationToken);
        if (Painel is null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
        }

        return null;
    }

    private Task<IdentityUser?> ExigirUsuarioAsync()
    {
        return _userManager.GetUserAsync(User);
    }

    public sealed class ConvidadoForm
    {
        public Guid ConviteId { get; set; }

        public string? Nome { get; set; }

        public string? Faixa { get; set; }

        public string? Custo { get; set; }

        public string? Genero { get; set; }

        public string? Celular { get; set; }

        public string? Observacao { get; set; }

        public Guid? GrupoId { get; set; }

        public bool Transfer { get; set; }

        public string? Origem { get; set; }
    }
}
