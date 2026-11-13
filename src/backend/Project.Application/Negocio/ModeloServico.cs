using Project.Application.Comum;
using Project.Domain.Eventos;

namespace Project.Application.Negocio;

public sealed record ModeloLeitura(Guid Id, string Nome, TipoEvento Tipo, IReadOnlyList<string> Servicos, IReadOnlyList<string> Tarefas);

public sealed record NovoModelo(string? Nome, TipoEvento? Tipo, IReadOnlyList<string>? Servicos, IReadOnlyList<string>? Tarefas);

public sealed class ModeloServico(IDados dados)
{
    public async Task<IReadOnlyList<ModeloLeitura>> ListarAsync(string contaId, CancellationToken ct)
    {
        var modelos = await dados.ListarModelosAsync(contaId, ct);
        return modelos.Select(Ler).ToArray();
    }

    public async Task<Resultado<ModeloLeitura>> CriarAsync(string contaId, NovoModelo pedido, CancellationToken ct)
    {
        var nome = Domain.Comum.Texto.Limpo(pedido.Nome);
        if (nome is null)
        {
            return Resultado<ModeloLeitura>.Erro(Codigos.Validacao, "Informe o nome do modelo.");
        }

        if (pedido.Tipo is not TipoEvento tipo || !Enum.IsDefined(tipo))
        {
            return Resultado<ModeloLeitura>.Erro(Codigos.Validacao, "Informe o tipo do modelo.");
        }

        var modelo = new ModeloEvento
        {
            Id = Guid.NewGuid(),
            ContaId = contaId,
            Nome = nome,
            Tipo = tipo,
        };
        foreach (var servico in Nomes(pedido.Servicos))
        {
            modelo.Servicos.Add(new ServicoModelo { Id = Guid.NewGuid(), ModeloId = modelo.Id, Nome = servico });
        }

        foreach (var tarefa in Nomes(pedido.Tarefas))
        {
            modelo.Tarefas.Add(new TarefaModelo { Id = Guid.NewGuid(), ModeloId = modelo.Id, Titulo = tarefa });
        }

        await dados.InserirModeloAsync(modelo, ct);
        return Resultado<ModeloLeitura>.Sucesso(Ler(modelo));
    }

    private static ModeloLeitura Ler(ModeloEvento modelo) => new(
        modelo.Id,
        modelo.Nome,
        modelo.Tipo,
        modelo.Servicos.Select(servico => servico.Nome).ToArray(),
        modelo.Tarefas.Select(tarefa => tarefa.Titulo).ToArray());

    private static IEnumerable<string> Nomes(IReadOnlyList<string>? valores) =>
        (valores ?? []).Select(Domain.Comum.Texto.Limpo).OfType<string>();
}
