using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Application.Eventos;
using Project.Application.Tarefas;
using Project.Domain;
using Project.Domain.Eventos;
using Project.Domain.Tarefas;
using Project.Infrastructure.Identity;

namespace Project.Web.Pages.Eventos.Tarefas;

[Authorize(Roles = Roles.User)]
public sealed class EditarModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly EventoServico _eventos;
    private readonly TarefaServico _tarefas;

    public EditarModel(UserManager<IdentityUser> userManager, EventoServico eventos, TarefaServico tarefas)
    {
        _userManager = userManager;
        _eventos = eventos;
        _tarefas = tarefas;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public bool Disponivel { get; private set; }

    public bool Aberto { get; private set; }

    public string Email { get; private set; } = "";

    public string? EventoNome { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var carregou = await CarregarAsync(id, preencher: true, cancellationToken);
        return carregou ?? Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        if (!SituacoesDeTarefa.TentarLer(Input.Situacao, out var situacao))
        {
            ModelState.AddModelError(string.Empty, "A situação da tarefa não é aceita.");
        }

        if (!ModelState.IsValid || !SituacoesDeTarefa.TentarLer(Input.Situacao, out situacao))
        {
            await CarregarAsync(id, preencher: false, cancellationToken);
            return Page();
        }

        try
        {
            await _tarefas.SalvarAsync(user.Id, id, Input.Titulo, Input.Descricao, Input.Prazo, situacao, cancellationToken);
            var tarefa = await _tarefas.ObterAsync(user.Id, id, cancellationToken);
            return RedirectToPage("/Eventos/Tarefas/Index", new { id = tarefa!.EventoId });
        }
        catch (RegraDeNegocioException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await CarregarAsync(id, preencher: false, cancellationToken);
            if (!Disponivel)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
            }

            return Page();
        }
    }

    private async Task<IActionResult?> CarregarAsync(Guid id, bool preencher, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        Email = user.Email ?? "";
        var tarefa = await _tarefas.ObterAsync(user.Id, id, cancellationToken);
        if (tarefa is null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return null;
        }

        var evento = await _eventos.ObterAsync(user.Id, tarefa.EventoId, cancellationToken);
        Disponivel = true;
        Aberto = evento?.Situacao == SituacaoEvento.EmAndamento;
        EventoNome = evento?.Nome;
        if (preencher)
        {
            Input = new InputModel
            {
                Titulo = tarefa.Titulo,
                Descricao = tarefa.Descricao,
                Prazo = tarefa.Prazo,
                Situacao = tarefa.Situacao.ToString()
            };
        }

        return null;
    }

    public sealed class InputModel
    {
        public string? Titulo { get; set; }

        public string? Descricao { get; set; }

        public DateOnly? Prazo { get; set; }

        public string? Situacao { get; set; }
    }
}
