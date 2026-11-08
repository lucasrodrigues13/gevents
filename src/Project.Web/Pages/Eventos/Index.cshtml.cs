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
public sealed class IndexModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly EventoServico _servico;

    public IndexModel(UserManager<IdentityUser> userManager, EventoServico servico)
    {
        _userManager = userManager;
        _servico = servico;
    }

    [BindProperty(SupportsGet = true)]
    public string Aba { get; set; } = "emandamento";

    [BindProperty(SupportsGet = true)]
    public string? Nome { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Tipo { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateOnly? De { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateOnly? Ate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Tag { get; set; }

    public IReadOnlyList<EventoResumo> Eventos { get; private set; } = [];

    public DateOnly Hoje { get; private set; }

    public string? Erro { get; private set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        Hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        if (!TiposDeEvento.TentarLer(Tipo, out var tipo))
        {
            Erro = "O tipo do evento não é aceito.";
            return Page();
        }

        Eventos = await _servico.ListarAsync(
            user.Id,
            new FiltroEventos(Situacao(), Nome, tipo, De, Ate, Tag),
            cancellationToken);
        return Page();
    }

    private SituacaoEvento Situacao()
    {
        return Aba switch
        {
            "concluido" => SituacaoEvento.Concluido,
            "cancelado" => SituacaoEvento.Cancelado,
            _ => SituacaoEvento.EmAndamento
        };
    }
}
