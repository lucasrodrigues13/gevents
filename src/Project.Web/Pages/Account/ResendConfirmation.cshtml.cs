using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Web.Security;

namespace Project.Web.Pages.Account;

public sealed class ResendConfirmationModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmailSender<IdentityUser> _emailSender;
    private readonly AccountRateLimiter _rateLimiter;

    public ResendConfirmationModel(
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
        if (!await _rateLimiter.AllowResendConfirmationAsync(HttpContext, cancellationToken))
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

        var user = await _userManager.FindByEmailAsync(Input.Email);
        if (user is null || user.EmailConfirmed)
        {
            Message = "Se a conta existir e ainda não estiver confirmada, enviaremos outro link.";
            return Page();
        }

        try
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = user.Id, code = token },
                protocol: Request.Scheme,
                host: Request.Host.Value)!;
            await _emailSender.SendConfirmationLinkAsync(user, user.Email!, link);
            Message = "Se a conta existir e ainda não estiver confirmada, enviaremos outro link.";
        }
        catch (InvalidOperationException)
        {
            Message = IdentityMessages.ConfirmationNotSent;
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
