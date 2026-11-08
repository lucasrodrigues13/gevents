using Project.Domain.Perfis;

namespace Project.Application.Perfis;

public interface IPerfilConsulta
{
    Task<Perfil?> ObterPorContaAsync(string accountId, CancellationToken cancellationToken);
}

public interface IPerfilRepositorio
{
    Task AdicionarAsync(Perfil perfil, CancellationToken cancellationToken);

    Task SalvarAsync(CancellationToken cancellationToken);
}

public sealed record PerfilDados(
    string? NomeEmpresa,
    string? Responsavel,
    string? Documento,
    string? Celular,
    DateOnly? Nascimento,
    string? Endereco);

public sealed class PerfilServico
{
    private readonly IPerfilConsulta _consulta;
    private readonly IPerfilRepositorio _repositorio;

    public PerfilServico(IPerfilConsulta consulta, IPerfilRepositorio repositorio)
    {
        _consulta = consulta;
        _repositorio = repositorio;
    }

    public async Task<Perfil?> ObterAsync(string accountId, CancellationToken cancellationToken)
    {
        return await _consulta.ObterPorContaAsync(accountId, cancellationToken);
    }

    public async Task SalvarAsync(string accountId, PerfilDados dados, DateOnly hoje, CancellationToken cancellationToken)
    {
        var perfil = await _consulta.ObterPorContaAsync(accountId, cancellationToken);
        if (perfil is null)
        {
            perfil = Perfil.Criar(accountId);
            await _repositorio.AdicionarAsync(perfil, cancellationToken);
        }

        perfil.Atualizar(
            dados.NomeEmpresa,
            dados.Responsavel,
            dados.Documento,
            dados.Celular,
            dados.Nascimento,
            dados.Endereco,
            hoje);

        await _repositorio.SalvarAsync(cancellationToken);
    }
}
