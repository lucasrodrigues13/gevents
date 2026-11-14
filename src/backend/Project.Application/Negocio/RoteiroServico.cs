using Project.Application.Comum;
using Project.Domain.Modelos;

namespace Project.Application.Negocio;

public sealed record ItemRoteiroLeitura(Guid Id, string Texto, int Ordem);

public sealed record SecaoRoteiroLeitura(
    Guid Id,
    string? Titulo,
    TimeOnly? Horario,
    string? Musica,
    int Ordem,
    IReadOnlyList<ItemRoteiroLeitura> Itens);

public sealed record RoteiroLeitura(Guid Id, string Nome, IReadOnlyList<SecaoRoteiroLeitura> Secoes);

public sealed record ItemRoteiroPedido(string? Texto);

public sealed record SecaoRoteiroPedido(string? Titulo, TimeOnly? Horario, string? Musica, IReadOnlyList<ItemRoteiroPedido>? Itens);

public sealed record SalvarRoteiro(string? Nome, IReadOnlyList<SecaoRoteiroPedido>? Secoes);

public sealed class RoteiroServico(IDados dados)
{
    public async Task<IReadOnlyList<RoteiroLeitura>> ListarAsync(string contaId, string? nome, CancellationToken ct)
    {
        var modelos = await dados.ListarRoteirosAsync(contaId, nome, ct);
        return modelos.Select(Ler).ToArray();
    }

    public async Task<Resultado<RoteiroLeitura>> ObterAsync(string contaId, Guid id, CancellationToken ct)
    {
        var modelo = await dados.ObterRoteiroAsync(contaId, id, ct);
        return modelo is null
            ? Resultado<RoteiroLeitura>.Erro(Codigos.Indisponivel, "Indisponível.")
            : Resultado<RoteiroLeitura>.Sucesso(Ler(modelo));
    }

    public async Task<Resultado<RoteiroLeitura>> CriarAsync(string contaId, SalvarRoteiro pedido, CancellationToken ct)
    {
        var montado = Montar(contaId, Guid.NewGuid(), pedido);
        if (!montado.Ok)
        {
            return Resultado<RoteiroLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        await dados.InserirRoteiroAsync(montado.Valor!, ct);
        return Resultado<RoteiroLeitura>.Sucesso(Ler(montado.Valor!));
    }

    public async Task<Resultado<RoteiroLeitura>> AtualizarAsync(string contaId, Guid id, SalvarRoteiro pedido, CancellationToken ct)
    {
        var modelo = await dados.ObterRoteiroAsync(contaId, id, ct);
        if (modelo is null)
        {
            return Resultado<RoteiroLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var montado = Montar(contaId, modelo.Id, pedido);
        if (!montado.Ok)
        {
            return Resultado<RoteiroLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        modelo.Nome = montado.Valor!.Nome;
        modelo.Secoes.Clear();
        foreach (var secao in montado.Valor.Secoes)
        {
            modelo.Secoes.Add(secao);
        }

        await dados.SalvarRoteiroAsync(ct);
        return Resultado<RoteiroLeitura>.Sucesso(Ler(modelo));
    }

    public async Task<Resultado<RoteiroLeitura>> ClonarAsync(string contaId, Guid id, CancellationToken ct)
    {
        var origem = await dados.ObterRoteiroAsync(contaId, id, ct);
        if (origem is null)
        {
            return Resultado<RoteiroLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var copia = new ModeloRoteiro
        {
            Id = Guid.NewGuid(),
            ContaId = contaId,
            Nome = origem.Nome + " (cópia)",
        };
        foreach (var secao in origem.Secoes.OrderBy(item => item.Ordem))
        {
            var secaoId = Guid.NewGuid();
            var nova = new SecaoRoteiro
            {
                Id = secaoId,
                ModeloId = copia.Id,
                Titulo = secao.Titulo,
                Horario = secao.Horario,
                Musica = secao.Musica,
                Ordem = secao.Ordem,
            };
            foreach (var item in secao.Itens.OrderBy(valor => valor.Ordem))
            {
                nova.Itens.Add(new ItemRoteiro
                {
                    Id = Guid.NewGuid(),
                    SecaoId = secaoId,
                    Texto = item.Texto,
                    Ordem = item.Ordem,
                });
            }

            copia.Secoes.Add(nova);
        }

        await dados.InserirRoteiroAsync(copia, ct);
        return Resultado<RoteiroLeitura>.Sucesso(Ler(copia));
    }

    public async Task<Resultado<bool>> ExcluirAsync(string contaId, Guid id, CancellationToken ct)
    {
        var modelo = await dados.ObterRoteiroAsync(contaId, id, ct);
        if (modelo is null)
        {
            return Resultado<bool>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        await dados.RemoverRoteiroAsync(modelo, ct);
        return Resultado<bool>.Sucesso(true);
    }

    private static Resultado<ModeloRoteiro> Montar(string contaId, Guid id, SalvarRoteiro pedido)
    {
        var nome = Domain.Comum.Texto.Limpo(pedido.Nome);
        if (nome is null)
        {
            return Resultado<ModeloRoteiro>.Erro(Codigos.Validacao, "Informe o nome do modelo.");
        }

        var modelo = new ModeloRoteiro { Id = id, ContaId = contaId, Nome = nome };
        var ordemSecao = 0;
        foreach (var secao in pedido.Secoes ?? [])
        {
            var secaoId = Guid.NewGuid();
            var entidade = new SecaoRoteiro
            {
                Id = secaoId,
                ModeloId = id,
                Titulo = Domain.Comum.Texto.Limpo(secao.Titulo),
                Horario = secao.Horario,
                Musica = Domain.Comum.Texto.Limpo(secao.Musica),
                Ordem = ordemSecao++,
            };
            var ordemItem = 0;
            foreach (var item in secao.Itens ?? [])
            {
                var texto = Domain.Comum.Texto.Limpo(item.Texto);
                if (texto is null)
                {
                    continue;
                }

                entidade.Itens.Add(new ItemRoteiro
                {
                    Id = Guid.NewGuid(),
                    SecaoId = secaoId,
                    Texto = texto,
                    Ordem = ordemItem++,
                });
            }

            modelo.Secoes.Add(entidade);
        }

        return Resultado<ModeloRoteiro>.Sucesso(modelo);
    }

    private static RoteiroLeitura Ler(ModeloRoteiro modelo) => new(
        modelo.Id,
        modelo.Nome,
        modelo.Secoes.OrderBy(secao => secao.Ordem).Select(secao => new SecaoRoteiroLeitura(
            secao.Id,
            secao.Titulo,
            secao.Horario,
            secao.Musica,
            secao.Ordem,
            secao.Itens.OrderBy(item => item.Ordem).Select(item => new ItemRoteiroLeitura(item.Id, item.Texto, item.Ordem)).ToArray())).ToArray());
}
