using Microsoft.AspNetCore.Identity;

namespace Project.IntegrationTests;

public sealed class FakeEmailSender : IEmailSender<IdentityUser>
{
    public List<SentEmail> Sent { get; } = [];

    public bool Fail { get; set; }

    public Task SendConfirmationLinkAsync(IdentityUser user, string email, string confirmationLink)
    {
        return StoreAsync(email, confirmationLink, SentEmailKind.Confirmation);
    }

    public Task SendPasswordResetLinkAsync(IdentityUser user, string email, string resetLink)
    {
        return StoreAsync(email, resetLink, SentEmailKind.PasswordReset);
    }

    public Task SendPasswordResetCodeAsync(IdentityUser user, string email, string resetCode)
    {
        return StoreAsync(email, resetCode, SentEmailKind.PasswordResetCode);
    }

    private Task StoreAsync(string email, string payload, SentEmailKind kind)
    {
        if (Fail)
        {
            throw new InvalidOperationException("Falha de SMTP.");
        }

        Sent.Add(new SentEmail(email, payload, kind));
        return Task.CompletedTask;
    }
}

public enum SentEmailKind
{
    Confirmation,
    PasswordReset,
    PasswordResetCode
}

public sealed record SentEmail(string Email, string Payload, SentEmailKind Kind);
