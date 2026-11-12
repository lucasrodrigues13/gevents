using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Project.Infrastructure.Conta;

public sealed class ConfirmacaoEmailTokenProvider : DataProtectorTokenProvider<IdentityUser>
{
    public const string Nome = "ConfirmacaoEmail";

    public ConfirmacaoEmailTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<ConfirmacaoEmailTokenProviderOptions> options,
        ILogger<DataProtectorTokenProvider<IdentityUser>> logger)
        : base(dataProtectionProvider, Microsoft.Extensions.Options.Options.Create<DataProtectionTokenProviderOptions>(options.Value), logger)
    {
    }
}

public sealed class ConfirmacaoEmailTokenProviderOptions : DataProtectionTokenProviderOptions
{
    public ConfirmacaoEmailTokenProviderOptions()
    {
        Name = ConfirmacaoEmailTokenProvider.Nome;
        TokenLifespan = TimeSpan.FromHours(24);
    }
}

public sealed class RedefinicaoSenhaTokenProvider : DataProtectorTokenProvider<IdentityUser>
{
    public const string Nome = "RedefinicaoSenha";

    public RedefinicaoSenhaTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<RedefinicaoSenhaTokenProviderOptions> options,
        ILogger<DataProtectorTokenProvider<IdentityUser>> logger)
        : base(dataProtectionProvider, Microsoft.Extensions.Options.Options.Create<DataProtectionTokenProviderOptions>(options.Value), logger)
    {
    }
}

public sealed class RedefinicaoSenhaTokenProviderOptions : DataProtectionTokenProviderOptions
{
    public RedefinicaoSenhaTokenProviderOptions()
    {
        Name = RedefinicaoSenhaTokenProvider.Nome;
        TokenLifespan = TimeSpan.FromHours(1);
    }
}
