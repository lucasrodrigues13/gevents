using Project.Application.Comum;
using Project.Domain.Funil;

namespace Project.Application.Negocio;

public sealed record ModeloServicoFunilLeitura(Guid Id, string Nome, string? Descricao, decimal? Valor);

public sealed record SalvarModeloServicoFunil(string? Nome, string? Descricao, decimal? Valor);

public sealed record ModeloOrcamentoFunilLeitura(
    Guid Id,
    string Nome,
    string Texto,
    string? CorPrimaria,
    string? CorSecundaria,
    IReadOnlyList<string> Parametros);

public sealed record SalvarModeloOrcamentoFunil(
    string? Nome,
    string? Texto,
    string? CorPrimaria,
    string? CorSecundaria);

public sealed record ModeloContratoFunilLeitura(Guid Id, string Nome, string Texto, IReadOnlyList<string> Parametros);

public sealed record SalvarModeloContratoFunil(string? Nome, string? Texto);

public static class TokensFunil
{
    public static readonly string[] Todos =
    [
        "#nome_oportunidade",
        "#data_prevista",
        "#horario",
        "#local_cerimonia",
        "#local_recepcao",
        "#quantidade_convidados",
        "#valor_proposta",
    ];
}

public sealed class ModeloServicoFunilServico(IDados dados)
{
    public async Task<IReadOnlyList<ModeloServicoFunilLeitura>> ListarAsync(string contaId, string? nome, CancellationToken ct)
    {
        var itens = await dados.ListarModelosServicoFunilAsync(contaId, nome, ct);
        return itens.Select(Ler).ToArray();
    }

    public async Task<Resultado<ModeloServicoFunilLeitura>> ObterAsync(string contaId, Guid id, CancellationToken ct)
    {
        var item = await dados.ObterModeloServicoFunilAsync(contaId, id, ct);
        return item is null
            ? Resultado<ModeloServicoFunilLeitura>.Erro(Codigos.Indisponivel, "Indisponível.")
            : Resultado<ModeloServicoFunilLeitura>.Sucesso(Ler(item));
    }

    public async Task<Resultado<ModeloServicoFunilLeitura>> CriarAsync(
        string contaId,
        SalvarModeloServicoFunil pedido,
        CancellationToken ct)
    {
        var montado = Montar(contaId, Guid.NewGuid(), pedido);
        if (!montado.Ok)
        {
            return Resultado<ModeloServicoFunilLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        await dados.InserirModeloServicoFunilAsync(montado.Valor!, ct);
        return Resultado<ModeloServicoFunilLeitura>.Sucesso(Ler(montado.Valor!));
    }

    public async Task<Resultado<ModeloServicoFunilLeitura>> AtualizarAsync(
        string contaId,
        Guid id,
        SalvarModeloServicoFunil pedido,
        CancellationToken ct)
    {
        var item = await dados.ObterModeloServicoFunilAsync(contaId, id, ct);
        if (item is null)
        {
            return Resultado<ModeloServicoFunilLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var montado = Montar(contaId, item.Id, pedido);
        if (!montado.Ok)
        {
            return Resultado<ModeloServicoFunilLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        item.Nome = montado.Valor!.Nome;
        item.Descricao = montado.Valor.Descricao;
        item.Valor = montado.Valor.Valor;
        await dados.SalvarNegocioAsync(ct);
        return Resultado<ModeloServicoFunilLeitura>.Sucesso(Ler(item));
    }

    public async Task<Resultado<bool>> ExcluirAsync(string contaId, Guid id, CancellationToken ct)
    {
        var item = await dados.ObterModeloServicoFunilAsync(contaId, id, ct);
        if (item is null)
        {
            return Resultado<bool>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        await dados.RemoverModeloServicoFunilAsync(item, ct);
        return Resultado<bool>.Sucesso(true);
    }

    private static Resultado<ModeloServicoFunil> Montar(string contaId, Guid id, SalvarModeloServicoFunil pedido)
    {
        var nome = Domain.Comum.Texto.Limpo(pedido.Nome);
        if (nome is null)
        {
            return Resultado<ModeloServicoFunil>.Erro(Codigos.Validacao, "Informe o nome do serviço/produto.");
        }

        var descricao = Domain.Comum.Texto.Limpo(pedido.Descricao);
        if (descricao is { Length: > 200 })
        {
            return Resultado<ModeloServicoFunil>.Erro(Codigos.Validacao, "A descrição deve ter até 200 caracteres.");
        }

        if (pedido.Valor is < 0)
        {
            return Resultado<ModeloServicoFunil>.Erro(Codigos.Validacao, "O valor não pode ser negativo.");
        }

        return Resultado<ModeloServicoFunil>.Sucesso(new ModeloServicoFunil
        {
            Id = id,
            ContaId = contaId,
            Nome = nome,
            Descricao = descricao,
            Valor = pedido.Valor,
        });
    }

    private static ModeloServicoFunilLeitura Ler(ModeloServicoFunil item) =>
        new(item.Id, item.Nome, item.Descricao, item.Valor);
}

public sealed class ModeloOrcamentoFunilServico(IDados dados)
{
    public async Task<IReadOnlyList<ModeloOrcamentoFunilLeitura>> ListarAsync(string contaId, string? nome, CancellationToken ct)
    {
        var itens = await dados.ListarModelosOrcamentoFunilAsync(contaId, nome, ct);
        return itens.Select(Ler).ToArray();
    }

    public async Task<Resultado<ModeloOrcamentoFunilLeitura>> ObterAsync(string contaId, Guid id, CancellationToken ct)
    {
        var item = await dados.ObterModeloOrcamentoFunilAsync(contaId, id, ct);
        return item is null
            ? Resultado<ModeloOrcamentoFunilLeitura>.Erro(Codigos.Indisponivel, "Indisponível.")
            : Resultado<ModeloOrcamentoFunilLeitura>.Sucesso(Ler(item));
    }

    public async Task<Resultado<ModeloOrcamentoFunilLeitura>> CriarAsync(
        string contaId,
        SalvarModeloOrcamentoFunil pedido,
        CancellationToken ct)
    {
        var montado = Montar(contaId, Guid.NewGuid(), pedido);
        if (!montado.Ok)
        {
            return Resultado<ModeloOrcamentoFunilLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        await dados.InserirModeloOrcamentoFunilAsync(montado.Valor!, ct);
        return Resultado<ModeloOrcamentoFunilLeitura>.Sucesso(Ler(montado.Valor!));
    }

    public async Task<Resultado<ModeloOrcamentoFunilLeitura>> AtualizarAsync(
        string contaId,
        Guid id,
        SalvarModeloOrcamentoFunil pedido,
        CancellationToken ct)
    {
        var item = await dados.ObterModeloOrcamentoFunilAsync(contaId, id, ct);
        if (item is null)
        {
            return Resultado<ModeloOrcamentoFunilLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var montado = Montar(contaId, item.Id, pedido);
        if (!montado.Ok)
        {
            return Resultado<ModeloOrcamentoFunilLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        item.Nome = montado.Valor!.Nome;
        item.Texto = montado.Valor.Texto;
        item.CorPrimaria = montado.Valor.CorPrimaria;
        item.CorSecundaria = montado.Valor.CorSecundaria;
        await dados.SalvarNegocioAsync(ct);
        return Resultado<ModeloOrcamentoFunilLeitura>.Sucesso(Ler(item));
    }

    public async Task<Resultado<bool>> ExcluirAsync(string contaId, Guid id, CancellationToken ct)
    {
        var item = await dados.ObterModeloOrcamentoFunilAsync(contaId, id, ct);
        if (item is null)
        {
            return Resultado<bool>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        await dados.RemoverModeloOrcamentoFunilAsync(item, ct);
        return Resultado<bool>.Sucesso(true);
    }

    private static Resultado<ModeloOrcamentoFunil> Montar(string contaId, Guid id, SalvarModeloOrcamentoFunil pedido)
    {
        var nome = Domain.Comum.Texto.Limpo(pedido.Nome);
        var texto = Domain.Comum.Texto.Limpo(pedido.Texto);
        if (nome is null)
        {
            return Resultado<ModeloOrcamentoFunil>.Erro(Codigos.Validacao, "Informe o nome do modelo.");
        }

        if (texto is null)
        {
            return Resultado<ModeloOrcamentoFunil>.Erro(Codigos.Validacao, "Informe o texto da proposta.");
        }

        var cor1 = Domain.Comum.Texto.Limpo(pedido.CorPrimaria);
        var cor2 = Domain.Comum.Texto.Limpo(pedido.CorSecundaria);
        if ((cor1 is not null && !CorValida(cor1)) || (cor2 is not null && !CorValida(cor2)))
        {
            return Resultado<ModeloOrcamentoFunil>.Erro(Codigos.Validacao, "Informe cores válidas.");
        }

        return Resultado<ModeloOrcamentoFunil>.Sucesso(new ModeloOrcamentoFunil
        {
            Id = id,
            ContaId = contaId,
            Nome = nome,
            Texto = texto,
            CorPrimaria = cor1,
            CorSecundaria = cor2,
        });
    }

    private static bool CorValida(string cor) =>
        cor.Length is 4 or 7 && cor.StartsWith('#') && cor[1..].All(c => Uri.IsHexDigit(c));

    private static ModeloOrcamentoFunilLeitura Ler(ModeloOrcamentoFunil item) =>
        new(item.Id, item.Nome, item.Texto, item.CorPrimaria, item.CorSecundaria, TokensFunil.Todos);
}

public sealed class ModeloContratoFunilServico(IDados dados)
{
    public async Task<IReadOnlyList<ModeloContratoFunilLeitura>> ListarAsync(string contaId, string? nome, CancellationToken ct)
    {
        var itens = await dados.ListarModelosContratoFunilAsync(contaId, nome, ct);
        return itens.Select(Ler).ToArray();
    }

    public async Task<Resultado<ModeloContratoFunilLeitura>> ObterAsync(string contaId, Guid id, CancellationToken ct)
    {
        var item = await dados.ObterModeloContratoFunilAsync(contaId, id, ct);
        return item is null
            ? Resultado<ModeloContratoFunilLeitura>.Erro(Codigos.Indisponivel, "Indisponível.")
            : Resultado<ModeloContratoFunilLeitura>.Sucesso(Ler(item));
    }

    public async Task<Resultado<ModeloContratoFunilLeitura>> CriarAsync(
        string contaId,
        SalvarModeloContratoFunil pedido,
        CancellationToken ct)
    {
        var montado = Montar(contaId, Guid.NewGuid(), pedido);
        if (!montado.Ok)
        {
            return Resultado<ModeloContratoFunilLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        await dados.InserirModeloContratoFunilAsync(montado.Valor!, ct);
        return Resultado<ModeloContratoFunilLeitura>.Sucesso(Ler(montado.Valor!));
    }

    public async Task<Resultado<ModeloContratoFunilLeitura>> AtualizarAsync(
        string contaId,
        Guid id,
        SalvarModeloContratoFunil pedido,
        CancellationToken ct)
    {
        var item = await dados.ObterModeloContratoFunilAsync(contaId, id, ct);
        if (item is null)
        {
            return Resultado<ModeloContratoFunilLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var montado = Montar(contaId, item.Id, pedido);
        if (!montado.Ok)
        {
            return Resultado<ModeloContratoFunilLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        item.Nome = montado.Valor!.Nome;
        item.Texto = montado.Valor.Texto;
        await dados.SalvarNegocioAsync(ct);
        return Resultado<ModeloContratoFunilLeitura>.Sucesso(Ler(item));
    }

    public async Task<Resultado<bool>> ExcluirAsync(string contaId, Guid id, CancellationToken ct)
    {
        var item = await dados.ObterModeloContratoFunilAsync(contaId, id, ct);
        if (item is null)
        {
            return Resultado<bool>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        await dados.RemoverModeloContratoFunilAsync(item, ct);
        return Resultado<bool>.Sucesso(true);
    }

    private static Resultado<ModeloContratoFunil> Montar(string contaId, Guid id, SalvarModeloContratoFunil pedido)
    {
        var nome = Domain.Comum.Texto.Limpo(pedido.Nome);
        var texto = Domain.Comum.Texto.Limpo(pedido.Texto);
        if (nome is null)
        {
            return Resultado<ModeloContratoFunil>.Erro(Codigos.Validacao, "Informe o nome do modelo.");
        }

        if (texto is null)
        {
            return Resultado<ModeloContratoFunil>.Erro(Codigos.Validacao, "Informe o texto do modelo.");
        }

        return Resultado<ModeloContratoFunil>.Sucesso(new ModeloContratoFunil
        {
            Id = id,
            ContaId = contaId,
            Nome = nome,
            Texto = texto,
        });
    }

    private static ModeloContratoFunilLeitura Ler(ModeloContratoFunil item) =>
        new(item.Id, item.Nome, item.Texto, TokensFunil.Todos);
}
