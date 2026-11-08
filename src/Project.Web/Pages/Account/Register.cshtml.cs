using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Infrastructure.Identity;
using Project.Web.Security;

namespace Project.Web.Pages.Account;

public sealed class RegisterModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IEmailSender<IdentityUser> _emailSender;
    private readonly AccountRateLimiter _rateLimiter;

    public RegisterModel(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IEmailSender<IdentityUser> emailSender,
        AccountRateLimiter rateLimiter)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _emailSender = emailSender;
        _rateLimiter = rateLimiter;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? Warning { get; private set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!await _rateLimiter.AllowSignupAsync(HttpContext, cancellationToken))
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

        var user = new IdentityUser
        {
            UserName = Input.Email,
            Email = Input.Email,
            EmailConfirmed = false
        };

        var created = await _userManager.CreateAsync(user, Input.Password);
        if (!created.Succeeded)
        {
            foreach (var error in created.Errors)
            {
                ModelState.AddModelError(string.Empty, IdentityMessages.FromIdentity(error));
            }

            return Page();
        }

        if (!await _roleManager.RoleExistsAsync(Roles.User))
        {
            var roleCreated = await _roleManager.CreateAsync(new IdentityRole(Roles.User));
            if (!roleCreated.Succeeded)
            {
                ModelState.AddModelError(string.Empty, IdentityMessages.GenericFailure);
                return Page();
            }
        }

        var role = await _userManager.AddToRoleAsync(user, Roles.User);
        if (!role.Succeeded)
        {
            ModelState.AddModelError(string.Empty, IdentityMessages.GenericFailure);
            return Page();
        }

        try
        {
            await SendConfirmationAsync(user);
        }
        catch (InvalidOperationException)
        {
            Warning = IdentityMessages.ConfirmationNotSent;
            return Page();
        }

        TempData["Message"] = "Enviamos o link de confirmação. Ele vale 24 horas.";
        return RedirectToPage("/Account/Login");
    }

    private async Task SendConfirmationAsync(IdentityUser user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var link = Url.Page(
            "/Account/ConfirmEmail",
            pageHandler: null,
            values: new { userId = user.Id, code = token },
            protocol: Request.Scheme,
            host: Request.Host.Value)!;

        await _emailSender.SendConfirmationLinkAsync(user, user.Email!, link);
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
