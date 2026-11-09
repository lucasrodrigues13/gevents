using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Application.Clientes;
using Project.Application.Eventos;
using Project.Domain.Clientes;
using Project.Web.Clientes;
using Project.Domain;
using Project.Infrastructure.Identity;
using Project.Web.Eventos;

namespace Project.Web.Pages.Eventos;

[Authorize(Roles = Roles.User)]
public sealed class CriarModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly EventoServico _servico;
    private readonly ModeloServico _modelos;
    private readonly ClienteServico _clientes;

    public CriarModel(UserManager<IdentityUser> userManager, EventoServico servico, ModeloServico modelos, ClienteServico clientes)
    {
        _userManager = userManager;
        _servico = servico;
        _modelos = modelos;
        _clientes = clientes;
    }

    [BindProperty]
    public EventoInput Input { get; set; } = new();

    [BindProperty]
    public bool CadastrarCliente { get; set; }

    [BindProperty]
    public ClienteInput Cliente { get; set; } = new();

    public IReadOnlyList<ModeloResumo> Modelos { get; private set; } = [];

    public async Task OnGetAsync(Guid? modelo, CancellationToken cancellationToken)
    {
        await CarregarModelosAsync(cancellationToken);
        if (modelo is not Guid id)
        {
            return;
        }

        var escolhido = Modelos.FirstOrDefault(item => item.Id == id);
        if (escolhido is null)
        {
            return;
        }

        Input.ModeloId = escolhido.Id;
        Input.Tipo = escolhido.Tipo.ToString();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
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

        if (CadastrarCliente)
        {
            try
            {
                Project.Domain.Clientes.Cliente.Criar(user.Id, Cliente.ParaDados());
            }
            catch (RegraDeNegocioException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                tipoOk = false;
            }
        }

        if (!ModelState.IsValid || !tipoOk)
        {
            await CarregarModelosAsync(cancellationToken);
            return Page();
        }

        var (conteudo, nome) = Input.AbrirImagem();
        try
        {
            var gravado = await _servico.CriarAsync(user.Id, dados, conteudo, nome, cancellationToken);
            if (CadastrarCliente)
            {
                var (foto, nomeFoto) = Cliente.AbrirImagem();
                try
                {
                    await _clientes.CriarAsync(user.Id, Cliente.ParaDados(), foto, nomeFoto, gravado.Id, cancellationToken);
                }
                finally
                {
                    if (foto is not null)
                    {
                        await foto.DisposeAsync();
                    }
                }
            }

            return RedirectToPage("/Eventos/Valor", new { id = gravado.Id, aviso = gravado.AvisoImagem });
        }
        catch (RegraDeNegocioException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await CarregarModelosAsync(cancellationToken);
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

    private async Task CarregarModelosAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        Modelos = user is null ? [] : await _modelos.ListarAsync(user.Id, cancellationToken);
    }
}
