using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Project.Application.Conta;
using Project.Infrastructure.Conta;

namespace Project.Tests.Conta;

public class ContaSemBancoTests
{
    [Fact]
    public void Confirmacao_vale_24_horas_e_redefinicao_vale_1_hora()
    {
        Assert.Equal(TimeSpan.FromHours(24), new ConfirmacaoEmailTokenProviderOptions().TokenLifespan);
        Assert.Equal(TimeSpan.FromHours(1), new RedefinicaoSenhaTokenProviderOptions().TokenLifespan);
        Assert.NotEqual(ConfirmacaoEmailTokenProvider.Nome, RedefinicaoSenhaTokenProvider.Nome);
    }

    [Fact]
    public void Senha_exige_8_caracteres_e_minuscula_e_o_bloqueio_e_de_15_minutos()
    {
        var opcoes = new IdentityOptions();
        OpcoesDeConta.Aplicar(opcoes);

        Assert.Equal(8, opcoes.Password.RequiredLength);
        Assert.True(opcoes.Password.RequireLowercase);
        Assert.False(opcoes.Password.RequireDigit);
        Assert.False(opcoes.Password.RequireUppercase);
        Assert.False(opcoes.Password.RequireNonAlphanumeric);
        Assert.Equal(5, opcoes.Lockout.MaxFailedAccessAttempts);
        Assert.Equal(TimeSpan.FromMinutes(15), opcoes.Lockout.DefaultLockoutTimeSpan);
        Assert.True(opcoes.SignIn.RequireConfirmedEmail);
        Assert.Equal(ConfirmacaoEmailTokenProvider.Nome, opcoes.Tokens.EmailConfirmationTokenProvider);
        Assert.Equal(RedefinicaoSenhaTokenProvider.Nome, opcoes.Tokens.PasswordResetTokenProvider);
    }

    [Fact]
    public void Jwt_de_acesso_dura_8_horas_e_carrega_o_papel_organizer()
    {
        var emissor = new EmissorJwt(Options.Create(new JwtOpcoes
        {
            Secret = "segredo-de-teste-com-32-caracteres",
            Issuer = "gevents-test",
            Audience = "gevents-test",
            ExpiryHours = 8,
        }));

        var token = emissor.Emitir("usuario-1", "pessoa@gevents.test", [Papeis.Organizer]);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal("gevents-test", jwt.Issuer);
        Assert.Contains(jwt.Claims, claim => claim.Type is "email" or ClaimTypes.Email && claim.Value == "pessoa@gevents.test");
        Assert.Contains(jwt.Claims, claim => claim.Type == ClaimTypes.Role && claim.Value == Papeis.Organizer);
        Assert.DoesNotContain(jwt.Claims, claim => claim.Type == ContaClaims.OrganizerId);
        Assert.InRange((jwt.ValidTo - DateTime.UtcNow).TotalHours, 7.5, 8.1);
    }

    [Fact]
    public void Jwt_de_client_inclui_organizer_id()
    {
        var emissor = new EmissorJwt(Options.Create(new JwtOpcoes
        {
            Secret = "segredo-de-teste-com-32-caracteres",
            Issuer = "gevents-test",
            Audience = "gevents-test",
            ExpiryHours = 8,
        }));

        var token = emissor.Emitir("client-1", "client@gevents.test", [Papeis.Client], "organizer-9");
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Contains(jwt.Claims, claim => claim.Type == ClaimTypes.Role && claim.Value == Papeis.Client);
        Assert.Contains(jwt.Claims, claim => claim.Type == ContaClaims.OrganizerId && claim.Value == "organizer-9");
    }

    [Fact]
    public void Jwt_sem_segredo_nao_e_emitido()
    {
        var emissor = new EmissorJwt(Options.Create(new JwtOpcoes { Secret = "curto" }));

        var erro = Assert.Throws<InvalidOperationException>(() => emissor.Emitir("usuario-1", "pessoa@gevents.test", [Papeis.Organizer]));

        Assert.DoesNotContain("curto", erro.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Client_exige_vinculo_com_organizer()
    {
        var client = new ApplicationUser { Id = "client-1" };

        Assert.Throws<InvalidOperationException>(() => client.GarantirVinculoClient());
        Assert.Throws<InvalidOperationException>(() => client.VincularAoOrganizer(""));
        Assert.Throws<InvalidOperationException>(() => client.VincularAoOrganizer("client-1"));

        client.VincularAoOrganizer("organizer-1");
        Assert.Equal("organizer-1", client.OrganizerId);
        client.GarantirVinculoClient();
    }
}
