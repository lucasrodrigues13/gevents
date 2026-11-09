using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Application.Eventos;
using Project.Domain;
using Project.Domain.Eventos;
using Project.Infrastructure.Identity;
using Project.Web.Eventos;

namespace Project.Web.Pages.Eventos;

[Authorize(Roles = Roles.User)]
public sealed class ValorModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly EventoServico _servico;

    public ValorModel(UserManager<IdentityUser> userManager, EventoServico servico)
    {
        _userManager = userManager;
        _servico = servico;
    }

    [BindProperty]
    public decimal? ValorTotal { get; set; }

    [BindProperty]
    public int? Quantidade { get; set; }

    [BindProperty]
    public DateOnly? Primeira { get; set; }

    [BindProperty]
    public bool ComEntrada { get; set; }

    [BindProperty]
    public string? BaseEntrada { get; set; }

    [BindProperty]
    public decimal? Entrada { get; set; }

    [BindProperty]
    public DateOnly? DataEntrada { get; set; }

    public string? Nome { get; private set; }

    [BindProperty(SupportsGet = true)]
    public string? Aviso { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var detalhe = await CarregarAsync(id, cancellationToken);
        if (detalhe is null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return Page();
        }

        Nome = detalhe.Nome;
        Aviso = EventoFormulario.AvisoPermitido(Aviso);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        if (ValorTotal is null || ValorTotal <= 0 || Quantidade is null || Quantidade <= 0 || Primeira is null)
        {
            ModelState.AddModelError(string.Empty, "Informe o valor, a quantidade de parcelas e a data da primeira.");
            var detalhe = await CarregarAsync(id, cancellationToken);
            Nome = detalhe?.Nome;
            if (detalhe is null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
            }

            return Page();
        }

        try
        {
            var baseEntrada = BaseEntrada == "Porcentagem" ? Project.Domain.Eventos.BaseEntrada.Porcentagem : Project.Domain.Eventos.BaseEntrada.Valor;
            await _servico.GravarAssessoriaAsync(user.Id, id, ValorTotal, Quantidade, Primeira, ComEntrada, baseEntrada, Entrada, DataEntrada, cancellationToken);
            return RedirectToPage("/Eventos/Home", new { id });
        }
        catch (RegraDeNegocioException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            var detalhe = await CarregarAsync(id, cancellationToken);
            Nome = detalhe?.Nome;
            return Page();
        }
    }

    private async Task<EventoDetalhe?> CarregarAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        return user is null ? null : await _servico.ObterAsync(user.Id, id, cancellationToken);
    }
}
