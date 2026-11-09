using Project.Domain;
using Project.Domain.Eventos;

namespace Project.Application.Eventos;

public sealed record ModeloResumo(Guid Id, string Nome, TipoEvento Tipo);

public sealed record ModeloDetalhe(Guid Id, string Nome, TipoEvento Tipo, IReadOnlyList<ServicoModelo> Servicos);

public interface IModeloConsulta
{
    Task<IReadOnlyList<ModeloResumo>> ListarAsync(string accountId, CancellationToken cancellationToken);

    Task<Modelo?> ObterAsync(string accountId, Guid id, CancellationToken cancellationToken);
}

public interface IModeloRepositorio
{
    Task AdicionarAsync(Modelo modelo, CancellationToken cancellationToken);

    Task RegistrarServicoAsync(ServicoModelo servico, CancellationToken cancellationToken);

    Task RegistrarTarefaAsync(TarefaModelo tarefa, CancellationToken cancellationToken);

    Task SalvarAsync(CancellationToken cancellationToken);
}

public sealed class ModeloServico
{
    private readonly IModeloConsulta _consulta;
    private readonly IModeloRepositorio _repositorio;

    public ModeloServico(IModeloConsulta consulta, IModeloRepositorio repositorio)
    {
        _consulta = consulta;
        _repositorio = repositorio;
    }

    public Task<IReadOnlyList<ModeloResumo>> ListarAsync(string accountId, CancellationToken cancellationToken)
    {
        return _consulta.ListarAsync(accountId, cancellationToken);
    }

    public async Task<ModeloDetalhe?> ObterAsync(string accountId, Guid id, CancellationToken cancellationToken)
    {
        var modelo = await _consulta.ObterAsync(accountId, id, cancellationToken);
        return modelo is null ? null : new ModeloDetalhe(modelo.Id, modelo.Nome, modelo.Tipo, modelo.Servicos);
    }

    public async Task<Guid> CriarAsync(string accountId, string? nome, TipoEvento? tipo, CancellationToken cancellationToken)
    {
        if (tipo is null)
        {
            throw new RegraDeNegocioException("O tipo do modelo não é aceito.");
        }

        var modelo = Modelo.Criar(accountId, nome, tipo.Value);
        await _repositorio.AdicionarAsync(modelo, cancellationToken);
        await _repositorio.SalvarAsync(cancellationToken);
        return modelo.Id;
    }

    public async Task AdicionarServicoAsync(string accountId, Guid modeloId, string? nome, CancellationToken cancellationToken)
    {
        var modelo = await ExigirAsync(accountId, modeloId, cancellationToken);
        var servico = modelo.AdicionarServico(nome);
        await _repositorio.RegistrarServicoAsync(servico, cancellationToken);
        await _repositorio.SalvarAsync(cancellationToken);
    }

    public async Task AdicionarTarefaAsync(string accountId, Guid modeloId, Guid servicoId, string? titulo, CancellationToken cancellationToken)
    {
        var modelo = await ExigirAsync(accountId, modeloId, cancellationToken);
        var servico = modelo.Servicos.FirstOrDefault(item => item.Id == servicoId)
            ?? throw new RegraDeNegocioException("O serviço não está disponível.");
        var tarefa = servico.AdicionarTarefa(titulo);
        await _repositorio.RegistrarTarefaAsync(tarefa, cancellationToken);
        await _repositorio.SalvarAsync(cancellationToken);
    }

    private async Task<Modelo> ExigirAsync(string accountId, Guid id, CancellationToken cancellationToken)
    {
        return await _consulta.ObterAsync(accountId, id, cancellationToken)
            ?? throw new RegraDeNegocioException("O modelo não está disponível.");
    }
}
