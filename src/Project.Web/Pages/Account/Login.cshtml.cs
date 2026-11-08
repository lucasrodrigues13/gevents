using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Infrastructure.Identity;
using Project.Web.Security;

namespace Project.Web.Pages.Account;

public sealed class LoginModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AccountRateLimiter _rateLimiter;

    public LoginModel(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        AccountRateLimiter rateLimiter)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _rateLimiter = rateLimiter;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? Message { get; private set; }

    public void OnGet()
    {
        Message = TempData["Message"] as string;
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!await _rateLimiter.AllowLoginAsync(HttpContext, cancellationToken))
        {
            return new ContentResult
            {
                StatusCode = StatusCodes.Status429TooManyRequests,
                Content = IdentityMessages.TooManyRequests
            };
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _signInManager.PasswordSignInAsync(
            Input.Email,
            Input.Password,
            isPersistent: false,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user is not null && await _userManager.IsInRoleAsync(user, Roles.Admin))
            {
                return RedirectToPage("/Admin/Index");
            }

            return RedirectToPage("/Dashboards/Index");
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, IdentityMessages.LockedOut);
            return Page();
        }

        if (result.IsNotAllowed)
        {
            ModelState.AddModelError(string.Empty, IdentityMessages.EmailNotConfirmed);
            return Page();
        }

        ModelState.AddModelError(string.Empty, IdentityMessages.InvalidCredentials);
        return Page();
    }

    public sealed class InputModel
    {
        [Required(ErrorMessage = "Informe o e-mail.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Informe a senha.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";
    }
}
