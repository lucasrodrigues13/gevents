using Project.Application.Comum;
using Project.Domain.Modelos;

namespace Project.Application.Negocio;

public sealed record ItemCronogramaLeitura(
    Guid Id,
    string? Rotulo,
    DateOnly? Data,
    TimeOnly? Hora,
    string? Responsavel,
    string? RecadoFornecedores,
    int Ordem);

public sealed record CronogramaLeitura(
    Guid Id,
    string Titulo,
    string? Descricao,
    IReadOnlyList<ItemCronogramaLeitura> Itens);

public sealed record ItemCronogramaPedido(
    string? Rotulo,
    DateOnly? Data,
    TimeOnly? Hora,
    string? Responsavel,
    string? RecadoFornecedores);

public sealed record SalvarCronograma(string? Titulo, string? Descricao, IReadOnlyList<ItemCronogramaPedido>? Itens);

public sealed class CronogramaServico(IDados dados)
{
    public async Task<IReadOnlyList<CronogramaLeitura>> ListarAsync(string contaId, string? nome, CancellationToken ct)
    {
        var modelos = await dados.ListarCronogramasAsync(contaId, nome, ct);
        return modelos.Select(Ler).ToArray();
    }

    public async Task<Resultado<CronogramaLeitura>> ObterAsync(string contaId, Guid id, CancellationToken ct)
    {
        var modelo = await dados.ObterCronogramaAsync(contaId, id, ct);
        return modelo is null
            ? Resultado<CronogramaLeitura>.Erro(Codigos.Indisponivel, "Indisponível.")
            : Resultado<CronogramaLeitura>.Sucesso(Ler(modelo));
    }

    public async Task<Resultado<CronogramaLeitura>> CriarAsync(string contaId, SalvarCronograma pedido, CancellationToken ct)
    {
        var montado = Montar(contaId, Guid.NewGuid(), pedido);
        if (!montado.Ok)
        {
            return Resultado<CronogramaLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        await dados.InserirCronogramaAsync(montado.Valor!, ct);
        return Resultado<CronogramaLeitura>.Sucesso(Ler(montado.Valor!));
    }

    public async Task<Resultado<CronogramaLeitura>> AtualizarAsync(string contaId, Guid id, SalvarCronograma pedido, CancellationToken ct)
    {
        var modelo = await dados.ObterCronogramaAsync(contaId, id, ct);
        if (modelo is null)
        {
            return Resultado<CronogramaLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var montado = Montar(contaId, modelo.Id, pedido);
        if (!montado.Ok)
        {
            return Resultado<CronogramaLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        modelo.Titulo = montado.Valor!.Titulo;
        modelo.Descricao = montado.Valor.Descricao;
        modelo.Itens.Clear();
        foreach (var item in montado.Valor.Itens)
        {
            modelo.Itens.Add(item);
        }

        await dados.SalvarCronogramaAsync(ct);
        return Resultado<CronogramaLeitura>.Sucesso(Ler(modelo));
    }

    public async Task<Resultado<CronogramaLeitura>> ClonarAsync(string contaId, Guid id, CancellationToken ct)
    {
        var origem = await dados.ObterCronogramaAsync(contaId, id, ct);
        if (origem is null)
        {
            return Resultado<CronogramaLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var copia = new ModeloCronograma
        {
            Id = Guid.NewGuid(),
            ContaId = contaId,
            Titulo = origem.Titulo + " (cópia)",
            Descricao = origem.Descricao,
        };
        foreach (var item in origem.Itens.OrderBy(valor => valor.Ordem))
        {
            copia.Itens.Add(new ItemCronograma
            {
                Id = Guid.NewGuid(),
                ModeloId = copia.Id,
                Rotulo = item.Rotulo,
                Data = item.Data,
                Hora = item.Hora,
                Responsavel = item.Responsavel,
                RecadoFornecedores = item.RecadoFornecedores,
                Ordem = item.Ordem,
            });
        }

        await dados.InserirCronogramaAsync(copia, ct);
        return Resultado<CronogramaLeitura>.Sucesso(Ler(copia));
    }

    public async Task<Resultado<bool>> ExcluirAsync(string contaId, Guid id, CancellationToken ct)
    {
        var modelo = await dados.ObterCronogramaAsync(contaId, id, ct);
        if (modelo is null)
        {
            return Resultado<bool>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        await dados.RemoverCronogramaAsync(modelo, ct);
        return Resultado<bool>.Sucesso(true);
    }

    private static Resultado<ModeloCronograma> Montar(string contaId, Guid id, SalvarCronograma pedido)
    {
        var titulo = Domain.Comum.Texto.Limpo(pedido.Titulo);
        if (titulo is null)
        {
            return Resultado<ModeloCronograma>.Erro(Codigos.Validacao, "Informe o título do modelo.");
        }

        var modelo = new ModeloCronograma
        {
            Id = id,
            ContaId = contaId,
            Titulo = titulo,
            Descricao = Domain.Comum.Texto.Limpo(pedido.Descricao),
        };
        var ordem = 0;
        foreach (var item in pedido.Itens ?? [])
        {
            modelo.Itens.Add(new ItemCronograma
            {
                Id = Guid.NewGuid(),
                ModeloId = id,
                Rotulo = Domain.Comum.Texto.Limpo(item.Rotulo),
                Data = item.Data,
                Hora = item.Hora,
                Responsavel = Domain.Comum.Texto.Limpo(item.Responsavel),
                RecadoFornecedores = Domain.Comum.Texto.Limpo(item.RecadoFornecedores),
                Ordem = ordem++,
            });
        }

        return Resultado<ModeloCronograma>.Sucesso(modelo);
    }

    private static CronogramaLeitura Ler(ModeloCronograma modelo) => new(
        modelo.Id,
        modelo.Titulo,
        modelo.Descricao,
        modelo.Itens.OrderBy(item => item.Ordem).Select(item => new ItemCronogramaLeitura(
            item.Id,
            item.Rotulo,
            item.Data,
            item.Hora,
            item.Responsavel,
            item.RecadoFornecedores,
            item.Ordem)).ToArray());
}
