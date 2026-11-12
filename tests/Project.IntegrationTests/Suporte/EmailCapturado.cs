using Microsoft.AspNetCore.Identity;

namespace Project.IntegrationTests.Suporte;

public sealed record MensagemDeEmail(string Email, string Tipo, string Corpo);

public sealed class EmailCapturado : IEmailSender<IdentityUser>
{
    private readonly List<MensagemDeEmail> _mensagens = [];

    public bool Falhar { get; set; }

    public IReadOnlyList<MensagemDeEmail> Mensagens
    {
        get
        {
            lock (_mensagens)
            {
                return _mensagens.ToArray();
            }
        }
    }

    public Task SendConfirmationLinkAsync(IdentityUser user, string email, string confirmationLink)
    {
        Registrar(email, "confirmacao", confirmationLink);
        return Task.CompletedTask;
    }

    public Task SendPasswordResetLinkAsync(IdentityUser user, string email, string resetLink)
    {
        Registrar(email, "redefinicao", resetLink);
        return Task.CompletedTask;
    }

    public Task SendPasswordResetCodeAsync(IdentityUser user, string email, string resetCode)
    {
        Registrar(email, "codigo", resetCode);
        return Task.CompletedTask;
    }

    private void Registrar(string email, string tipo, string corpo)
    {
        if (Falhar)
        {
            throw new InvalidOperationException("falha simulada de envio");
        }

        lock (_mensagens)
        {
            _mensagens.Add(new MensagemDeEmail(email, tipo, corpo));
        }
    }
}
