using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Project.Application.Conta;
using Project.Infrastructure.Conta;

namespace Project.Api.Conta;

public static class ContaEndpoints
{
    public static void Mapear(WebApplication app)
    {
        var conta = app.MapGroup("/api/v1/conta");
        conta.MapPost("/cadastro", Cadastrar).RequireRateLimiting("cadastro");
        conta.MapPost("/entrar", Entrar).RequireRateLimiting("login");
        conta.MapPost("/confirmar-email", Confirmar);
        conta.MapPost("/reenviar-confirmacao", Reenviar).RequireRateLimiting("reenvio");
        conta.MapPost("/esqueci-senha", EsqueciSenha).RequireRateLimiting("recuperacao");
        conta.MapPost("/redefinir-senha", RedefinirSenha);
        conta.MapPost("/sair", Sair).RequireAuthorization();
        conta.MapGet("/area", Area).RequireAuthorization();

        app.MapGet("/api/v1/admin/area", Area)
            .RequireAuthorization(policy => policy.RequireRole(Papeis.Admin));
    }

    private static async Task<IResult> Cadastrar(
        CadastroPedido pedido,
        UserManager<IdentityUser> usuarios,
        IEmailSender<IdentityUser> email,
        IConfiguration configuracao,
        ILoggerFactory logs,
        CancellationToken ct)
    {
        var endereco = EmailNormalizado(pedido.Email);
        if (endereco is null)
        {
            return Erro(StatusCodes.Status400BadRequest, "validacao", "Informe um e-mail válido.");
        }

        if (string.IsNullOrWhiteSpace(pedido.Senha))
        {
            return Erro(StatusCodes.Status400BadRequest, "validacao", "Informe uma senha.");
        }

        var senha = pedido.Senha!;
        if (pedido.Confirmacao is not null && pedido.Confirmacao != senha)
        {
            return Erro(StatusCodes.Status400BadRequest, "validacao", "A confirmação da senha não confere.");
        }

        var usuario = new IdentityUser
        {
            UserName = endereco,
            Email = endereco,
        };
        var criado = await usuarios.CreateAsync(usuario, senha);
        if (!criado.Succeeded)
        {
            if (criado.Errors.Any(erro => erro.Code is "DuplicateUserName" or "DuplicateEmail"))
            {
                return Erro(StatusCodes.Status409Conflict, "duplicado", "Já existe conta para este e-mail.");
            }

            return Erro(StatusCodes.Status400BadRequest, "validacao", "A senha não atende às regras.");
        }

        var papel = await usuarios.AddToRoleAsync(usuario, Papeis.User);
        if (!papel.Succeeded)
        {
            return Erro(StatusCodes.Status500InternalServerError, "erro", "Não foi possível concluir a ação.");
        }

        var enviado = await EnviarConfirmacaoAsync(usuarios, email, usuario, configuracao, logs, ct);
        if (!enviado)
        {
            return Results.Json(
                new { mensagem = "A conta foi criada, mas o e-mail de confirmação não foi enviado.", confirmacaoEnviada = false },
                statusCode: StatusCodes.Status201Created);
        }

        return Results.Json(
            new { mensagem = "Conta criada. Enviamos um link para confirmar o e-mail.", confirmacaoEnviada = true },
            statusCode: StatusCodes.Status201Created);
    }

    private static async Task<IResult> Entrar(
        LoginPedido pedido,
        UserManager<IdentityUser> usuarios,
        SignInManager<IdentityUser> acesso,
        IEmissorJwt emissor)
    {
        var email = EmailNormalizado(pedido.Email);
        if (email is null || string.IsNullOrWhiteSpace(pedido.Senha))
        {
            return CredencialInvalida();
        }

        var usuario = await usuarios.FindByEmailAsync(email);
        if (usuario is null)
        {
            return CredencialInvalida();
        }

        var resultado = await acesso.CheckPasswordSignInAsync(usuario, pedido.Senha!, lockoutOnFailure: true);
        if (resultado.IsLockedOut)
        {
            return Erro(StatusCodes.Status423Locked, "bloqueio", "A ação não pode ser repetida agora.");
        }

        if (!resultado.Succeeded)
        {
            return CredencialInvalida();
        }

        var papeis = await usuarios.GetRolesAsync(usuario);
        var emailConfirmado = usuario.Email ?? email;
        var token = emissor.Emitir(usuario.Id, emailConfirmado, papeis.ToArray());
        return Results.Ok(new { token, email = emailConfirmado, papeis });
    }

    private static async Task<IResult> Confirmar(TokenPedido pedido, UserManager<IdentityUser> usuarios)
    {
        var usuario = string.IsNullOrWhiteSpace(pedido.UsuarioId) ? null : await usuarios.FindByIdAsync(pedido.UsuarioId);
        if (usuario is null || string.IsNullOrWhiteSpace(pedido.Token))
        {
            return TokenInvalido();
        }

        var resultado = await usuarios.ConfirmEmailAsync(usuario, pedido.Token);
        if (!resultado.Succeeded)
        {
            return TokenInvalido();
        }

        return Results.Ok(new { mensagem = "E-mail confirmado." });
    }

    private static async Task<IResult> Reenviar(
        EmailPedido pedido,
        UserManager<IdentityUser> usuarios,
        IEmailSender<IdentityUser> email,
        IConfiguration configuracao,
        ILoggerFactory logs,
        CancellationToken ct)
    {
        var emailNormalizado = EmailNormalizado(pedido.Email);
        if (emailNormalizado is not null)
        {
            var usuario = await usuarios.FindByEmailAsync(emailNormalizado);
            if (usuario is not null && !usuario.EmailConfirmed)
            {
                await EnviarConfirmacaoAsync(usuarios, email, usuario, configuracao, logs, ct);
            }
        }

        return Results.Ok(new { mensagem = "Se a conta existir e ainda não estiver confirmada, enviaremos outro link." });
    }

    private static async Task<IResult> EsqueciSenha(
        EmailPedido pedido,
        UserManager<IdentityUser> usuarios,
        IEmailSender<IdentityUser> email,
        IConfiguration configuracao,
        ILoggerFactory logs,
        CancellationToken ct)
    {
        var emailNormalizado = EmailNormalizado(pedido.Email);
        if (emailNormalizado is not null)
        {
            var usuario = await usuarios.FindByEmailAsync(emailNormalizado);
            if (usuario is not null)
            {
                try
                {
                    var token = await usuarios.GeneratePasswordResetTokenAsync(usuario);
                    var url = Link(configuracao, "redefinir-senha", usuario.Id, token);
                    await email.SendPasswordResetLinkAsync(usuario, usuario.Email ?? emailNormalizado, url);
                }
                catch (Exception)
                {
                    logs.CreateLogger("gevents.email").LogWarning("Falha ao enviar o e-mail de redefinição.");
                }
            }
        }

        return Results.Ok(new { mensagem = "Se a conta existir, enviaremos um link para redefinir a senha." });
    }

    private static async Task<IResult> RedefinirSenha(RedefinicaoPedido pedido, UserManager<IdentityUser> usuarios)
    {
        if (string.IsNullOrWhiteSpace(pedido.Senha))
        {
            return Erro(StatusCodes.Status400BadRequest, "validacao", "Informe uma senha.");
        }

        var senha = pedido.Senha!;
        if (pedido.Confirmacao is not null && pedido.Confirmacao != senha)
        {
            return Erro(StatusCodes.Status400BadRequest, "validacao", "A confirmação da senha não confere.");
        }

        var usuario = string.IsNullOrWhiteSpace(pedido.UsuarioId) ? null : await usuarios.FindByIdAsync(pedido.UsuarioId);
        if (usuario is null || string.IsNullOrWhiteSpace(pedido.Token))
        {
            return TokenInvalido();
        }

        var resultado = await usuarios.ResetPasswordAsync(usuario, pedido.Token!, senha);
        if (!resultado.Succeeded)
        {
            if (resultado.Errors.Any(erro => erro.Code.Contains("Password", StringComparison.Ordinal)))
            {
                return Erro(StatusCodes.Status400BadRequest, "validacao", "A senha não atende às regras.");
            }

            return TokenInvalido();
        }

        return Results.Ok(new { mensagem = "Senha redefinida." });
    }

    private static IResult Sair() => Results.NoContent();

    private static IResult Area(ClaimsPrincipal usuario)
    {
        var email = usuario.FindFirstValue(ClaimTypes.Email)
            ?? usuario.FindFirstValue("email");
        var papeis = usuario.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToArray();
        return Results.Ok(new { email, papeis });
    }

    private static async Task<bool> EnviarConfirmacaoAsync(
        UserManager<IdentityUser> usuarios,
        IEmailSender<IdentityUser> email,
        IdentityUser usuario,
        IConfiguration configuracao,
        ILoggerFactory logs,
        CancellationToken ct)
    {
        try
        {
            var token = await usuarios.GenerateEmailConfirmationTokenAsync(usuario);
            var endereco = usuario.Email ?? usuario.UserName ?? "";
            var url = Link(configuracao, "confirmar-email", usuario.Id, token);
            await email.SendConfirmationLinkAsync(usuario, endereco, url);
            return true;
        }
        catch (Exception)
        {
            logs.CreateLogger("gevents.email").LogWarning("Falha ao enviar o e-mail de confirmação.");
            return false;
        }
    }

    private static string Link(IConfiguration configuracao, string pagina, string usuarioId, string token)
    {
        var publico = (configuracao["PublicUrl"] ?? "").TrimEnd('/');
        return $"{publico}/{pagina}?usuarioId={Uri.EscapeDataString(usuarioId)}&token={Uri.EscapeDataString(token)}";
    }

    private static string? EmailNormalizado(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        var endereco = email.Trim();
        var arroba = endereco.IndexOf('@');
        if (arroba <= 0 || arroba >= endereco.Length - 1 || endereco.Contains(' '))
        {
            return null;
        }

        return endereco;
    }

    private static IResult CredencialInvalida()
        => Erro(StatusCodes.Status401Unauthorized, "credencial", "E-mail ou senha inválidos.");

    private static IResult TokenInvalido()
        => Erro(StatusCodes.Status400BadRequest, "token", "O link é inválido ou expirou.");

    private static IResult Erro(int status, string code, string message)
        => Results.Json(new ErroResposta(code, message), statusCode: status);
}

public sealed record CadastroPedido(string? Email, string? Senha, string? Confirmacao);
public sealed record LoginPedido(string? Email, string? Senha);
public sealed record EmailPedido(string? Email);
public sealed record TokenPedido(string? UsuarioId, string? Token);
public sealed record RedefinicaoPedido(string? UsuarioId, string? Token, string? Senha, string? Confirmacao);
