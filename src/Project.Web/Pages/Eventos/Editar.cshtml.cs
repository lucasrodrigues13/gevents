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
public sealed class EditarModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly EventoServico _servico;

    public EditarModel(UserManager<IdentityUser> userManager, EventoServico servico)
    {
        _userManager = userManager;
        _servico = servico;
    }

    [BindProperty]
    public EventoInput Input { get; set; } = new();

    public bool Disponivel { get; private set; }

    public bool Aberto { get; private set; }

    public bool TemImagem { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var detalhe = await CarregarAsync(id, cancellationToken);
        if (detalhe is null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return Page();
        }

        Disponivel = true;
        Input = EventoFormulario.ParaInput(detalhe);
        Aberto = detalhe.Situacao == SituacaoEvento.EmAndamento;
        TemImagem = detalhe.ArquivoId is not null;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var tipoOk = EventoFormulario.TentarCriarDados(Input, out var dados, out var erro);
        if (!tipoOk)
        {
            ModelState.AddModelError(string.Empty, erro!);
        }

        if (!ModelState.IsValid || !tipoOk)
        {
            await MarcarEstadoAsync(user.Id, id, cancellationToken);
            return Page();
        }

        var (conteudo, nome) = Input.AbrirImagem();
        try
        {
            var gravado = await _servico.EditarAsync(user.Id, id, dados, conteudo, nome, cancellationToken);
            return RedirectToPage("/Eventos/Home", new { id = gravado.Id, aviso = gravado.AvisoImagem });
        }
        catch (RegraDeNegocioException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await MarcarEstadoAsync(user.Id, id, cancellationToken);
            if (!Disponivel)
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

    private async Task MarcarEstadoAsync(string accountId, Guid id, CancellationToken cancellationToken)
    {
        var detalhe = await _servico.ObterAsync(accountId, id, cancellationToken);
        Disponivel = detalhe is not null;
        Aberto = detalhe is null || detalhe.Situacao == SituacaoEvento.EmAndamento;
        TemImagem = detalhe?.ArquivoId is not null;
    }

    private async Task<EventoDetalhe?> CarregarAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return null;
        }

        return await _servico.ObterAsync(user.Id, id, cancellationToken);
    }
}
