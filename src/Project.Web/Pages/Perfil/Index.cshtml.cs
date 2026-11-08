using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Application.Perfis;
using Project.Domain;
using Project.Infrastructure.Identity;

namespace Project.Web.Pages.Perfil;

[Authorize(Roles = Roles.User)]
public sealed class IndexModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly PerfilServico _servico;

    public IndexModel(UserManager<IdentityUser> userManager, PerfilServico servico)
    {
        _userManager = userManager;
        _servico = servico;
    }

    public string Email { get; private set; } = "";

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? Message { get; private set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        Email = user.Email ?? "";
        var perfil = await _servico.ObterAsync(user.Id, cancellationToken);
        if (perfil is not null)
        {
            Input = new InputModel
            {
                NomeEmpresa = perfil.NomeEmpresa,
                Responsavel = perfil.Responsavel,
                Documento = perfil.Documento,
                Celular = perfil.Celular,
                Nascimento = perfil.Nascimento,
                Endereco = perfil.Endereco
            };
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        Email = user.Email ?? "";
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _servico.SalvarAsync(
                user.Id,
                new PerfilDados(Input.NomeEmpresa, Input.Responsavel, Input.Documento, Input.Celular, Input.Nascimento, Input.Endereco),
                DateOnly.FromDateTime(DateTime.UtcNow),
                cancellationToken);
        }
        catch (RegraDeNegocioException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return Page();
        }

        Message = "Perfil salvo.";
        return Page();
    }

    public sealed class InputModel
    {
        public string? NomeEmpresa { get; set; }

        public string? Responsavel { get; set; }

        public string? Documento { get; set; }

        public string? Celular { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? Nascimento { get; set; }

        public string? Endereco { get; set; }
    }
}
