using Project.Application.Comum;
using Project.Domain.Perfil;

namespace Project.Application.Negocio;

public sealed record PerfilLeitura(
    string Email,
    string? NomeEmpresa,
    string? Responsavel,
    string? Documento,
    string? Celular,
    DateOnly? Nascimento,
    string? Endereco);

public sealed record SalvarPerfil(
    string? NomeEmpresa,
    string? Responsavel,
    string? Documento,
    string? Celular,
    DateOnly? Nascimento,
    string? Endereco);

public sealed class PerfilServico(IDados dados)
{
    public async Task<PerfilLeitura> ObterAsync(string contaId, string email, CancellationToken ct)
    {
        var perfil = await dados.ObterPerfilAsync(contaId, ct);
        return Ler(email, perfil);
    }

    public async Task<Resultado<PerfilLeitura>> SalvarAsync(
        string contaId,
        string email,
        SalvarPerfil pedido,
        DateOnly hoje,
        CancellationToken ct)
    {
        if (pedido.Nascimento is DateOnly nascimento && nascimento > hoje)
        {
            return Resultado<PerfilLeitura>.Erro(Codigos.Validacao, "A data de nascimento não pode ser futura.");
        }

        var perfil = await dados.ObterPerfilAsync(contaId, ct) ?? new PerfilAssessoria
        {
            Id = Guid.NewGuid(),
            ContaId = contaId,
        };
        perfil.NomeEmpresa = TextoLimpo(pedido.NomeEmpresa);
        perfil.Responsavel = TextoLimpo(pedido.Responsavel);
        perfil.Documento = TextoLimpo(pedido.Documento);
        perfil.Celular = TextoLimpo(pedido.Celular);
        perfil.Nascimento = pedido.Nascimento;
        perfil.Endereco = TextoLimpo(pedido.Endereco);
        await dados.SalvarPerfilAsync(perfil, ct);
        return Resultado<PerfilLeitura>.Sucesso(Ler(email, perfil));
    }

    private static PerfilLeitura Ler(string email, PerfilAssessoria? perfil) => new(
        email,
        perfil?.NomeEmpresa,
        perfil?.Responsavel,
        perfil?.Documento,
        perfil?.Celular,
        perfil?.Nascimento,
        perfil?.Endereco);

    private static string? TextoLimpo(string? valor) => Domain.Comum.Texto.Limpo(valor);
}
