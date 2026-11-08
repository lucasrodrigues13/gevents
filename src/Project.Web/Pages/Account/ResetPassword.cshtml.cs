using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project.Web.Pages.Account;

public sealed class ResetPasswordModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    public ResetPasswordModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public bool Completed { get; private set; }

    public void OnGet(string? email, string? code)
    {
        Input.Email = email ?? "";
        Input.Code = code ?? "";
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.FindByEmailAsync(Input.Email);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, IdentityMessages.InvalidToken);
            return Page();
        }

        var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, IdentityMessages.InvalidToken);
            return Page();
        }

        Completed = true;
        return Page();
    }

    public sealed class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Code { get; set; } = "";

        [Required(ErrorMessage = "Informe a senha.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";
    }
}
