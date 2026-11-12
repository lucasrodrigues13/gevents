using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Project.Application.Conta;

namespace Project.Infrastructure.Conta;

public sealed class EmissorJwt : IEmissorJwt
{
    private readonly JwtOpcoes _opcoes;

    public EmissorJwt(IOptions<JwtOpcoes> opcoes)
    {
        _opcoes = opcoes.Value;
    }

    public string Emitir(string usuarioId, string email, IReadOnlyList<string> papeis)
    {
        if (string.IsNullOrWhiteSpace(_opcoes.Secret) || _opcoes.Secret.Length < 32)
        {
            throw new InvalidOperationException("O segredo do JWT não está configurado.");
        }

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opcoes.Secret));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuarioId),
            new(ClaimTypes.NameIdentifier, usuarioId),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.Email, email),
        };
        claims.AddRange(papeis.Select(papel => new Claim(ClaimTypes.Role, papel)));

        var token = new JwtSecurityToken(
            issuer: _opcoes.Issuer,
            audience: _opcoes.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(_opcoes.ExpiryHours),
            signingCredentials: credenciais);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
