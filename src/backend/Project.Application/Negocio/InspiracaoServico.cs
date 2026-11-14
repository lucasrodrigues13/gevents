using Project.Application.Comum;
using Project.Domain.Modelos;

namespace Project.Application.Negocio;

public sealed record ItemInspiracaoLeitura(Guid Id, Guid? ArquivoId, string? Url, int Ordem);

public sealed record InspiracaoLeitura(Guid Id, string Nome, IReadOnlyList<ItemInspiracaoLeitura> Itens);

public sealed record ItemInspiracaoPedido(Guid? ArquivoId, string? Url);

public sealed record SalvarInspiracao(string? Nome, IReadOnlyList<ItemInspiracaoPedido>? Itens);

public sealed class InspiracaoServico(IDados dados)
{
    public async Task<IReadOnlyList<InspiracaoLeitura>> ListarAsync(string contaId, string? nome, CancellationToken ct)
    {
        var modelos = await dados.ListarInspiracoesAsync(contaId, nome, ct);
        return modelos.Select(Ler).ToArray();
    }

    public async Task<Resultado<InspiracaoLeitura>> ObterAsync(string contaId, Guid id, CancellationToken ct)
    {
        var modelo = await dados.ObterInspiracaoAsync(contaId, id, ct);
        return modelo is null
            ? Resultado<InspiracaoLeitura>.Erro(Codigos.Indisponivel, "Indisponível.")
            : Resultado<InspiracaoLeitura>.Sucesso(Ler(modelo));
    }

    public async Task<Resultado<InspiracaoLeitura>> CriarAsync(string contaId, SalvarInspiracao pedido, CancellationToken ct)
    {
        var montado = Montar(contaId, Guid.NewGuid(), pedido);
        if (!montado.Ok)
        {
            return Resultado<InspiracaoLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        await dados.InserirInspiracaoAsync(montado.Valor!, ct);
        return Resultado<InspiracaoLeitura>.Sucesso(Ler(montado.Valor!));
    }

    public async Task<Resultado<InspiracaoLeitura>> AtualizarAsync(string contaId, Guid id, SalvarInspiracao pedido, CancellationToken ct)
    {
        var modelo = await dados.ObterInspiracaoAsync(contaId, id, ct);
        if (modelo is null)
        {
            return Resultado<InspiracaoLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var montado = Montar(contaId, modelo.Id, pedido);
        if (!montado.Ok)
        {
            return Resultado<InspiracaoLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        modelo.Nome = montado.Valor!.Nome;
        modelo.Itens.Clear();
        foreach (var item in montado.Valor.Itens)
        {
            modelo.Itens.Add(item);
        }

        await dados.SalvarInspiracaoAsync(ct);
        return Resultado<InspiracaoLeitura>.Sucesso(Ler(modelo));
    }

    public async Task<Resultado<InspiracaoLeitura>> ClonarAsync(string contaId, Guid id, CancellationToken ct)
    {
        var origem = await dados.ObterInspiracaoAsync(contaId, id, ct);
        if (origem is null)
        {
            return Resultado<InspiracaoLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var copia = new ModeloInspiracao
        {
            Id = Guid.NewGuid(),
            ContaId = contaId,
            Nome = origem.Nome + " (cópia)",
        };
        foreach (var item in origem.Itens.OrderBy(valor => valor.Ordem))
        {
            copia.Itens.Add(new ItemInspiracao
            {
                Id = Guid.NewGuid(),
                GaleriaId = copia.Id,
                ArquivoId = item.ArquivoId,
                Url = item.Url,
                Ordem = item.Ordem,
            });
        }

        await dados.InserirInspiracaoAsync(copia, ct);
        return Resultado<InspiracaoLeitura>.Sucesso(Ler(copia));
    }

    public async Task<Resultado<bool>> ExcluirAsync(string contaId, Guid id, CancellationToken ct)
    {
        var modelo = await dados.ObterInspiracaoAsync(contaId, id, ct);
        if (modelo is null)
        {
            return Resultado<bool>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        await dados.RemoverInspiracaoAsync(modelo, ct);
        return Resultado<bool>.Sucesso(true);
    }

    private static Resultado<ModeloInspiracao> Montar(string contaId, Guid id, SalvarInspiracao pedido)
    {
        var nome = Domain.Comum.Texto.Limpo(pedido.Nome);
        if (nome is null)
        {
            return Resultado<ModeloInspiracao>.Erro(Codigos.Validacao, "Informe o nome da galeria.");
        }

        var modelo = new ModeloInspiracao { Id = id, ContaId = contaId, Nome = nome };
        var ordem = 0;
        foreach (var item in pedido.Itens ?? [])
        {
            var url = Domain.Comum.Texto.Limpo(item.Url);
            if (item.ArquivoId is null && url is null)
            {
                continue;
            }

            modelo.Itens.Add(new ItemInspiracao
            {
                Id = Guid.NewGuid(),
                GaleriaId = id,
                ArquivoId = item.ArquivoId,
                Url = url,
                Ordem = ordem++,
            });
        }

        return Resultado<ModeloInspiracao>.Sucesso(modelo);
    }

    private static InspiracaoLeitura Ler(ModeloInspiracao modelo) => new(
        modelo.Id,
        modelo.Nome,
        modelo.Itens.OrderBy(item => item.Ordem)
            .Select(item => new ItemInspiracaoLeitura(item.Id, item.ArquivoId, item.Url, item.Ordem))
            .ToArray());
}
