using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Application.Eventos;
using Project.Application.Tarefas;
using Project.Domain;
using Project.Domain.Eventos;
using Project.Infrastructure.Identity;

namespace Project.Web.Pages.Eventos.Tarefas;

[Authorize(Roles = Roles.User)]
public sealed class IndexModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly EventoServico _eventos;
    private readonly TarefaServico _tarefas;

    public IndexModel(UserManager<IdentityUser> userManager, EventoServico eventos, TarefaServico tarefas)
    {
        _userManager = userManager;
        _eventos = eventos;
        _tarefas = tarefas;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public EventoDetalhe? Evento { get; private set; }

    public IReadOnlyList<TarefaItem> Tarefas { get; private set; } = [];

    public string Email { get; private set; } = "";

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await CarregarAsync(id, cancellationToken) ?? Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            return await CarregarAsync(id, cancellationToken) ?? Page();
        }

        try
        {
            await _tarefas.CriarAsync(user.Id, id, Input.Titulo, Input.Descricao, Input.Prazo, cancellationToken);
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
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        Email = user.Email ?? "";
        Evento = await _eventos.ObterAsync(user.Id, id, cancellationToken);
        if (Evento is null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return null;
        }

        Tarefas = await _tarefas.ListarPorEventoAsync(user.Id, id, cancellationToken) ?? [];
        return null;
    }

    public sealed class InputModel
    {
        public string? Titulo { get; set; }

        public string? Descricao { get; set; }

        public DateOnly? Prazo { get; set; }
    }
}
