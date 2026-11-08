using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Web.Security;

namespace Project.Web.Pages.Account;

public sealed class ForgotPasswordModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmailSender<IdentityUser> _emailSender;
    private readonly AccountRateLimiter _rateLimiter;

    public ForgotPasswordModel(
        UserManager<IdentityUser> userManager,
        IEmailSender<IdentityUser> emailSender,
        AccountRateLimiter rateLimiter)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _rateLimiter = rateLimiter;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? Message { get; private set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!await _rateLimiter.AllowPasswordResetAsync(HttpContext, cancellationToken))
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

        Message = IdentityMessages.ResetGeneric;
        var user = await _userManager.FindByEmailAsync(Input.Email);
        if (user is null)
        {
            return Page();
        }

        try
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var link = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { email = user.Email, code = token },
                protocol: Request.Scheme,
                host: Request.Host.Value)!;
            await _emailSender.SendPasswordResetLinkAsync(user, user.Email!, link);
        }
        catch (InvalidOperationException)
        {
            Message = IdentityMessages.ResetGeneric;
        }

        return Page();
    }

    public sealed class InputModel
    {
        [Required(ErrorMessage = "Informe o e-mail.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        public string Email { get; set; } = "";
    }
}
