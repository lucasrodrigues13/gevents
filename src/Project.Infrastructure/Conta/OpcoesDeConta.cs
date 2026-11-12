using Microsoft.AspNetCore.Identity;

namespace Project.Infrastructure.Conta;

public static class OpcoesDeConta
{
    public static void Aplicar(IdentityOptions options)
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireLowercase = true;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.AllowedForNewUsers = true;
        options.Tokens.EmailConfirmationTokenProvider = ConfirmacaoEmailTokenProvider.Nome;
        options.Tokens.PasswordResetTokenProvider = RedefinicaoSenhaTokenProvider.Nome;
    }
}
