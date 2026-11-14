using Project.Application.Comum;
using Project.Domain.Modelos;

namespace Project.Application.Negocio;

public sealed record PerguntaBriefingLeitura(Guid Id, string Enunciado, TipoPerguntaBriefing Tipo, int Ordem);

public sealed record SecaoBriefingLeitura(
    Guid Id,
    string Titulo,
    string? Descricao,
    int Ordem,
    IReadOnlyList<PerguntaBriefingLeitura> Perguntas);

public sealed record BriefingLeitura(
    Guid Id,
    string Nome,
    string Origem,
    bool Padrao,
    DateTimeOffset CriadoEm,
    IReadOnlyList<SecaoBriefingLeitura> Secoes);

public sealed record PerguntaBriefingPedido(string? Enunciado, TipoPerguntaBriefing? Tipo);

public sealed record SecaoBriefingPedido(string? Titulo, string? Descricao, IReadOnlyList<PerguntaBriefingPedido>? Perguntas);

public sealed record SalvarBriefing(string? Nome, string? Origem, IReadOnlyList<SecaoBriefingPedido>? Secoes);

public sealed class BriefingServico(IDados dados)
{
    public async Task<IReadOnlyList<BriefingLeitura>> ListarAsync(string contaId, string? nome, CancellationToken ct)
    {
        var modelos = await dados.ListarBriefingsAsync(contaId, nome, ct);
        return modelos.Select(Ler).ToArray();
    }

    public async Task<Resultado<BriefingLeitura>> ObterAsync(string contaId, Guid id, CancellationToken ct)
    {
        var modelo = await dados.ObterBriefingAsync(contaId, id, ct);
        return modelo is null
            ? Resultado<BriefingLeitura>.Erro(Codigos.Indisponivel, "Indisponível.")
            : Resultado<BriefingLeitura>.Sucesso(Ler(modelo));
    }

    public async Task<Resultado<BriefingLeitura>> CriarAsync(string contaId, SalvarBriefing pedido, CancellationToken ct)
    {
        var montado = Montar(contaId, Guid.NewGuid(), pedido, DateTimeOffset.UtcNow, padrao: false);
        if (!montado.Ok)
        {
            return Resultado<BriefingLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        await dados.InserirBriefingAsync(montado.Valor!, ct);
        return Resultado<BriefingLeitura>.Sucesso(Ler(montado.Valor!));
    }

    public async Task<Resultado<BriefingLeitura>> AtualizarAsync(string contaId, Guid id, SalvarBriefing pedido, CancellationToken ct)
    {
        var modelo = await dados.ObterBriefingAsync(contaId, id, ct);
        if (modelo is null)
        {
            return Resultado<BriefingLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        if (modelo.Padrao)
        {
            return Resultado<BriefingLeitura>.Erro(Codigos.Validacao, "Clone antes de editar.");
        }

        var montado = Montar(contaId, modelo.Id, pedido, modelo.CriadoEm, modelo.Padrao);
        if (!montado.Ok)
        {
            return Resultado<BriefingLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        modelo.Nome = montado.Valor!.Nome;
        modelo.Origem = montado.Valor.Origem;
        modelo.Secoes.Clear();
        foreach (var secao in montado.Valor.Secoes)
        {
            modelo.Secoes.Add(secao);
        }

        await dados.SalvarBriefingAsync(ct);
        return Resultado<BriefingLeitura>.Sucesso(Ler(modelo));
    }

    public async Task<Resultado<BriefingLeitura>> ClonarAsync(string contaId, Guid id, CancellationToken ct)
    {
        var origem = await dados.ObterBriefingAsync(contaId, id, ct);
        if (origem is null)
        {
            return Resultado<BriefingLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var copia = new ModeloBriefing
        {
            Id = Guid.NewGuid(),
            ContaId = contaId,
            Nome = origem.Nome + " (cópia)",
            Origem = origem.Origem,
            Padrao = false,
            CriadoEm = DateTimeOffset.UtcNow,
        };
        foreach (var secao in origem.Secoes.OrderBy(item => item.Ordem))
        {
            var secaoId = Guid.NewGuid();
            var nova = new SecaoBriefing
            {
                Id = secaoId,
                ModeloId = copia.Id,
                Titulo = secao.Titulo,
                Descricao = secao.Descricao,
                Ordem = secao.Ordem,
            };
            foreach (var pergunta in secao.Perguntas.OrderBy(item => item.Ordem))
            {
                nova.Perguntas.Add(new PerguntaBriefing
                {
                    Id = Guid.NewGuid(),
                    SecaoId = secaoId,
                    Enunciado = pergunta.Enunciado,
                    Tipo = pergunta.Tipo,
                    Ordem = pergunta.Ordem,
                });
            }

            copia.Secoes.Add(nova);
        }

        await dados.InserirBriefingAsync(copia, ct);
        return Resultado<BriefingLeitura>.Sucesso(Ler(copia));
    }

    public async Task<Resultado<bool>> ExcluirAsync(string contaId, Guid id, CancellationToken ct)
    {
        var modelo = await dados.ObterBriefingAsync(contaId, id, ct);
        if (modelo is null)
        {
            return Resultado<bool>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        await dados.RemoverBriefingAsync(modelo, ct);
        return Resultado<bool>.Sucesso(true);
    }

    private static Resultado<ModeloBriefing> Montar(
        string contaId,
        Guid id,
        SalvarBriefing pedido,
        DateTimeOffset criadoEm,
        bool padrao)
    {
        var nome = Domain.Comum.Texto.Limpo(pedido.Nome);
        if (nome is null)
        {
            return Resultado<ModeloBriefing>.Erro(Codigos.Validacao, "Informe o nome do modelo.");
        }

        var modelo = new ModeloBriefing
        {
            Id = id,
            ContaId = contaId,
            Nome = nome,
            Origem = Domain.Comum.Texto.Limpo(pedido.Origem) ?? "GestaoEventos",
            Padrao = padrao,
            CriadoEm = criadoEm,
        };

        var ordemSecao = 0;
        foreach (var secao in pedido.Secoes ?? [])
        {
            var titulo = Domain.Comum.Texto.Limpo(secao.Titulo) ?? "";
            var secaoId = Guid.NewGuid();
            var entidade = new SecaoBriefing
            {
                Id = secaoId,
                ModeloId = id,
                Titulo = titulo,
                Descricao = Domain.Comum.Texto.Limpo(secao.Descricao),
                Ordem = ordemSecao++,
            };
            var ordemPergunta = 0;
            foreach (var pergunta in secao.Perguntas ?? [])
            {
                var enunciado = Domain.Comum.Texto.Limpo(pergunta.Enunciado);
                if (enunciado is null)
                {
                    return Resultado<ModeloBriefing>.Erro(Codigos.Validacao, "Informe o enunciado da pergunta.");
                }

                var tipo = pergunta.Tipo ?? TipoPerguntaBriefing.TextoCurto;
                if (!Enum.IsDefined(tipo))
                {
                    return Resultado<ModeloBriefing>.Erro(Codigos.Validacao, "Informe o tipo da pergunta.");
                }

                entidade.Perguntas.Add(new PerguntaBriefing
                {
                    Id = Guid.NewGuid(),
                    SecaoId = secaoId,
                    Enunciado = enunciado,
                    Tipo = tipo,
                    Ordem = ordemPergunta++,
                });
            }

            modelo.Secoes.Add(entidade);
        }

        return Resultado<ModeloBriefing>.Sucesso(modelo);
    }

    private static BriefingLeitura Ler(ModeloBriefing modelo) => new(
        modelo.Id,
        modelo.Nome,
        modelo.Origem,
        modelo.Padrao,
        modelo.CriadoEm,
        modelo.Secoes.OrderBy(secao => secao.Ordem).Select(secao => new SecaoBriefingLeitura(
            secao.Id,
            secao.Titulo,
            secao.Descricao,
            secao.Ordem,
            secao.Perguntas.OrderBy(pergunta => pergunta.Ordem).Select(pergunta =>
                new PerguntaBriefingLeitura(pergunta.Id, pergunta.Enunciado, pergunta.Tipo, pergunta.Ordem)).ToArray())).ToArray());
}
