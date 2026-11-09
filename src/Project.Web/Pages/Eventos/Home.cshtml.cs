using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Application.Clientes;
using Project.Application.Eventos;
using Project.Domain;
using Project.Domain.Eventos;
using Project.Infrastructure.Identity;
using Project.Web.Eventos;

namespace Project.Web.Pages.Eventos;

[Authorize(Roles = Roles.User)]
public sealed class HomeModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly EventoServico _servico;
    private readonly ClienteServico _clientes;

    public HomeModel(UserManager<IdentityUser> userManager, EventoServico servico, ClienteServico clientes)
    {
        _userManager = userManager;
        _servico = servico;
        _clientes = clientes;
    }

    public EventoDetalhe? Detalhe { get; private set; }

    public string? Confirmar { get; private set; }

    public string? Aviso { get; private set; }

    public DateOnly Hoje { get; private set; }

    public IReadOnlyList<ClienteResumo> ClientesDoEvento { get; private set; } = [];

    public IReadOnlyList<ClienteItem> ClientesDaConta { get; private set; } = [];

    public IReadOnlyList<string> Copias { get; private set; } = [];

    public Guid? LinkCadastro { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid id, string? confirmar, string? aviso, CancellationToken cancellationToken)
    {
        Hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        Aviso = EventoFormulario.AvisoPermitido(aviso);
        Detalhe = await CarregarAsync(id, cancellationToken);
        if (Detalhe is null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return Page();
        }

        await CarregarClientesAsync(id, cancellationToken);
        var conta = await _userManager.GetUserAsync(User);
        Copias = conta is null ? [] : await _servico.ListarCopiasAsync(conta.Id, id, cancellationToken);
        LinkCadastro = conta is null ? null : await _clientes.ObterLinkDoEventoAsync(conta.Id, id, cancellationToken);

        if (Detalhe.Situacao == SituacaoEvento.EmAndamento && confirmar is "concluir" or "cancelar")
        {
            Confirmar = confirmar;
        }

        return Page();
    }

    public Task<IActionResult> OnPostConcluirAsync(Guid id, string? confirmado, CancellationToken cancellationToken)
    {
        return EncerrarAsync(id, confirmado, (conta, token) => _servico.ConcluirAsync(conta, id, token), "Evento concluído.", cancellationToken);
    }

    public Task<IActionResult> OnPostCancelarAsync(Guid id, string? confirmado, CancellationToken cancellationToken)
    {
        return EncerrarAsync(id, confirmado, (conta, token) => _servico.CancelarAsync(conta, id, token), "Evento cancelado.", cancellationToken);
    }

    public async Task<IActionResult> OnPostVincularAsync(Guid id, Guid clienteId, CancellationToken cancellationToken)
    {
        return await AlterarVinculoAsync(id, userId => _clientes.VincularAsync(userId, clienteId, id, cancellationToken), cancellationToken);
    }

    public async Task<IActionResult> OnPostGerarLinkAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        try
        {
            await _clientes.ObterOuCriarLinkAsync(user.Id, id, cancellationToken);
            return RedirectToPage(new { id });
        }
        catch (RegraDeNegocioException)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return Page();
        }
    }

    public async Task<IActionResult> OnPostDesvincularAsync(Guid id, Guid clienteId, CancellationToken cancellationToken)
    {
        return await AlterarVinculoAsync(id, userId => _clientes.DesvincularAsync(userId, clienteId, id, cancellationToken), cancellationToken);
    }

    private async Task<IActionResult> EncerrarAsync(
        Guid id,
        string? confirmado,
        Func<string, CancellationToken, Task> acao,
        string aviso,
        CancellationToken cancellationToken)
    {
        if (confirmado != "sim")
        {
            return RedirectToPage(new { id });
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        try
        {
            await acao(user.Id, cancellationToken);
            return RedirectToPage(new { id, aviso });
        }
        catch (RegraDeNegocioException exception)
        {
            return RedirectToPage(new { id, aviso = exception.Message });
        }
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

    private async Task CarregarClientesAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return;
        }

        ClientesDoEvento = await _clientes.ListarDoEventoAsync(user.Id, id, cancellationToken);
        ClientesDaConta = await _clientes.ListarAsync(user.Id, null, cancellationToken);
    }

    private async Task<IActionResult> AlterarVinculoAsync(Guid id, Func<string, Task> acao, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        try
        {
            await acao(user.Id);
        }
        catch (RegraDeNegocioException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
        }

        Hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        Detalhe = await CarregarAsync(id, cancellationToken);
        if (Detalhe is null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return Page();
        }

        await CarregarClientesAsync(id, cancellationToken);
        return Page();
    }
}
