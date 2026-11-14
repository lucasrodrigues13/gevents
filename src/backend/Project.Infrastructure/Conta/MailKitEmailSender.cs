using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Project.Infrastructure.Conta;

public sealed class MailKitEmailSender : IEmailSender<ApplicationUser>
{
    private readonly EmailOpcoes _opcoes;

    public MailKitEmailSender(IOptions<EmailOpcoes> opcoes)
    {
        _opcoes = opcoes.Value;
    }

    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        return EnviarAsync(email, "Confirme seu e-mail", $"Use este link para confirmar sua conta: {confirmationLink}", CancellationToken.None);
    }

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        return EnviarAsync(email, "Redefina sua senha", $"Use este link para redefinir sua senha: {resetLink}", CancellationToken.None);
    }

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        return EnviarAsync(email, "Redefina sua senha", "Foi gerado um código para redefinir sua senha.", CancellationToken.None);
    }

    private async Task EnviarAsync(string email, string assunto, string texto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_opcoes.Host) || string.IsNullOrWhiteSpace(_opcoes.Password) || string.IsNullOrWhiteSpace(_opcoes.From))
        {
            throw new InvalidOperationException("O envio de e-mail não está configurado.");
        }

        var mensagem = new MimeMessage();
        mensagem.From.Add(MailboxAddress.Parse(_opcoes.From));
        mensagem.To.Add(MailboxAddress.Parse(email));
        mensagem.Subject = assunto;
        mensagem.Body = new TextPart("plain") { Text = texto };

        using var cliente = new SmtpClient();
        var soquete = _opcoes.StartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
        await cliente.ConnectAsync(_opcoes.Host, _opcoes.Port, soquete, cancellationToken);
        await cliente.AuthenticateAsync(_opcoes.Username, _opcoes.Password, cancellationToken);
        await cliente.SendAsync(mensagem, cancellationToken);
        await cliente.DisconnectAsync(true, cancellationToken);
    }
}
