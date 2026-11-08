using Microsoft.AspNetCore.Identity;

namespace Project.Web.Pages.Account;

public static class IdentityMessages
{
    public const string InvalidCredentials = "E-mail ou senha incorretos.";

    public const string EmailNotConfirmed = "Confirme o e-mail antes de entrar.";

    public const string LockedOut = "Muitas tentativas. Tente novamente mais tarde.";

    public const string TooManyRequests = "Muitas tentativas. Tente novamente mais tarde.";

    public const string DuplicateEmail = "Já existe uma conta com este e-mail.";

    public const string ConfirmationNotSent = "A conta foi criada, mas a confirmação não foi enviada. O e-mail continua sem confirmação.";

    public const string ResetGeneric = "Se existir uma conta com esse e-mail, enviaremos as instruções de redefinição.";

    public const string InvalidToken = "O link é inválido ou expirou.";

    public const string GenericFailure = "Não foi possível concluir a operação.";

    public static string FromIdentity(IdentityError error)
    {
        return error.Code switch
        {
            "DuplicateEmail" or "DuplicateUserName" => DuplicateEmail,
            "InvalidEmail" => "Informe um e-mail válido.",
            "PasswordTooShort" => "A senha precisa ter pelo menos 8 caracteres.",
            "PasswordRequiresLower" => "A senha precisa ter pelo menos uma letra minúscula.",
            _ => GenericFailure
        };
    }
}
