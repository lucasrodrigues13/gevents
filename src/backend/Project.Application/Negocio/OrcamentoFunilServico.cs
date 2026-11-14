using Project.Application.Comum;
using Project.Domain.Funil;

namespace Project.Application.Negocio;

public sealed record OrcamentoFunilLeitura(
    Guid Id,
    Guid OportunidadeId,
    string OportunidadeNome,
    string Conteudo,
    decimal? Valor,
    string Situacao,
    DateTimeOffset CriadoEm,
    DateTimeOffset? EnviadoEm,
    DateTimeOffset? RespondidoEm,
    Guid? ModeloId);

public sealed record SalvarOrcamentoFunil(
    Guid? OportunidadeId,
    string? Conteudo,
    decimal? Valor,
    Guid? ModeloId,
    bool? MoverParaOrcamentoEnviado);

public sealed class OrcamentoFunilServico(IDados dados)
{
    public async Task<IReadOnlyList<OrcamentoFunilLeitura>> ListarAsync(
        string contaId,
        string? situacao,
        string? busca,
        DateOnly? criadoDe,
        DateOnly? criadoAte,
        CancellationToken ct)
    {
        SituacaoOrcamentoFunil? filtro = null;
        if (!string.IsNullOrWhiteSpace(situacao) &&
            Enum.TryParse<SituacaoOrcamentoFunil>(situacao, true, out var lido) &&
            Enum.IsDefined(lido))
        {
            filtro = lido;
        }

        var itens = await dados.ListarOrcamentosFunilAsync(contaId, filtro, busca, criadoDe, criadoAte, ct);
        var nomes = await dados.NomesOportunidadesAsync(contaId, itens.Select(i => i.OportunidadeId), ct);
        return itens.Select(item => Ler(item, nomes.GetValueOrDefault(item.OportunidadeId, ""))).ToArray();
    }

    public async Task<Resultado<OrcamentoFunilLeitura>> ObterAsync(string contaId, Guid id, CancellationToken ct)
    {
        var item = await dados.ObterOrcamentoFunilAsync(contaId, id, ct);
        if (item is null)
        {
            return Resultado<OrcamentoFunilLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var nomes = await dados.NomesOportunidadesAsync(contaId, [item.OportunidadeId], ct);
        return Resultado<OrcamentoFunilLeitura>.Sucesso(Ler(item, nomes.GetValueOrDefault(item.OportunidadeId, "")));
    }

    public async Task<Resultado<OrcamentoFunilLeitura>> CriarAsync(string contaId, SalvarOrcamentoFunil pedido, CancellationToken ct)
    {
        if (pedido.OportunidadeId is not Guid oportunidadeId ||
            await dados.ObterOportunidadeAsync(contaId, oportunidadeId, ct) is not { } oportunidade)
        {
            return Resultado<OrcamentoFunilLeitura>.Erro(Codigos.Validacao, "Informe a oportunidade.");
        }

        if (pedido.Valor is < 0)
        {
            return Resultado<OrcamentoFunilLeitura>.Erro(Codigos.Validacao, "O valor não pode ser negativo.");
        }

        var conteudo = Domain.Comum.Texto.Limpo(pedido.Conteudo);
        Guid? modeloId = pedido.ModeloId;
        if (conteudo is null && modeloId is Guid mid)
        {
            var modelo = await dados.ObterModeloOrcamentoFunilAsync(contaId, mid, ct);
            if (modelo is null)
            {
                return Resultado<OrcamentoFunilLeitura>.Erro(Codigos.Validacao, "Modelo inválido.");
            }

            conteudo = AplicarTokens(modelo.Texto, oportunidade.Nome);
        }
        else if (modeloId is Guid mid2)
        {
            var modelo = await dados.ObterModeloOrcamentoFunilAsync(contaId, mid2, ct);
            if (modelo is null)
            {
                return Resultado<OrcamentoFunilLeitura>.Erro(Codigos.Validacao, "Modelo inválido.");
            }
        }

        if (conteudo is null)
        {
            return Resultado<OrcamentoFunilLeitura>.Erro(Codigos.Validacao, "Informe o conteúdo do orçamento.");
        }

        var item = new OrcamentoFunil
        {
            Id = Guid.NewGuid(),
            ContaId = contaId,
            OportunidadeId = oportunidadeId,
            Conteudo = conteudo,
            Valor = pedido.Valor,
            Situacao = SituacaoOrcamentoFunil.EmAndamento,
            CriadoEm = DateTimeOffset.UtcNow,
            ModeloId = modeloId,
        };
        await dados.InserirOrcamentoFunilAsync(item, ct);
        return Resultado<OrcamentoFunilLeitura>.Sucesso(Ler(item, oportunidade.Nome));
    }

    public async Task<Resultado<OrcamentoFunilLeitura>> AtualizarAsync(
        string contaId,
        Guid id,
        SalvarOrcamentoFunil pedido,
        CancellationToken ct)
    {
        var item = await dados.ObterOrcamentoFunilAsync(contaId, id, ct);
        if (item is null)
        {
            return Resultado<OrcamentoFunilLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        if (item.Situacao != SituacaoOrcamentoFunil.EmAndamento)
        {
            return Resultado<OrcamentoFunilLeitura>.Erro(Codigos.Validacao, "Só orçamentos em andamento podem ser editados.");
        }

        var conteudo = Domain.Comum.Texto.Limpo(pedido.Conteudo);
        if (conteudo is null)
        {
            return Resultado<OrcamentoFunilLeitura>.Erro(Codigos.Validacao, "Informe o conteúdo do orçamento.");
        }

        if (pedido.Valor is < 0)
        {
            return Resultado<OrcamentoFunilLeitura>.Erro(Codigos.Validacao, "O valor não pode ser negativo.");
        }

        item.Conteudo = conteudo;
        item.Valor = pedido.Valor;
        await dados.SalvarNegocioAsync(ct);
        var nomes = await dados.NomesOportunidadesAsync(contaId, [item.OportunidadeId], ct);
        return Resultado<OrcamentoFunilLeitura>.Sucesso(Ler(item, nomes.GetValueOrDefault(item.OportunidadeId, "")));
    }

    public async Task<Resultado<OrcamentoFunilLeitura>> EnviarAsync(
        string contaId,
        Guid id,
        bool? moverParaOrcamentoEnviado,
        CancellationToken ct)
    {
        var item = await dados.ObterOrcamentoFunilAsync(contaId, id, ct);
        if (item is null)
        {
            return Resultado<OrcamentoFunilLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        if (item.Situacao != SituacaoOrcamentoFunil.EmAndamento)
        {
            return Resultado<OrcamentoFunilLeitura>.Erro(Codigos.Validacao, "Só orçamentos em andamento podem ser enviados.");
        }

        item.EnviadoEm = DateTimeOffset.UtcNow;
        if (moverParaOrcamentoEnviado == true)
        {
            var oportunidade = await dados.ObterOportunidadeAsync(contaId, item.OportunidadeId, ct);
            if (oportunidade is not null &&
                oportunidade.Estagio is EstagioOportunidade.ContatoInicial)
            {
                oportunidade.Estagio = EstagioOportunidade.OrcamentoEnviado;
                oportunidade.EntradaNoEstagioEm = DateTimeOffset.UtcNow;
            }
        }

        await dados.SalvarNegocioAsync(ct);
        var nomes = await dados.NomesOportunidadesAsync(contaId, [item.OportunidadeId], ct);
        return Resultado<OrcamentoFunilLeitura>.Sucesso(Ler(item, nomes.GetValueOrDefault(item.OportunidadeId, "")));
    }

    public Task<Resultado<OrcamentoFunilLeitura>> ResponderAsync(string contaId, Guid id, CancellationToken ct) =>
        MudarAsync(contaId, id, item =>
        {
            if (item.Situacao != SituacaoOrcamentoFunil.EmAndamento)
            {
                return "Só orçamentos em andamento podem receber resposta.";
            }

            item.Situacao = SituacaoOrcamentoFunil.Respondido;
            item.RespondidoEm = DateTimeOffset.UtcNow;
            return null;
        }, ct);

    public Task<Resultado<OrcamentoFunilLeitura>> CancelarAsync(string contaId, Guid id, CancellationToken ct) =>
        MudarAsync(contaId, id, item =>
        {
            if (item.Situacao != SituacaoOrcamentoFunil.EmAndamento)
            {
                return "Só orçamentos em andamento podem ser cancelados.";
            }

            item.Situacao = SituacaoOrcamentoFunil.Cancelado;
            return null;
        }, ct);

    public async Task<Resultado<bool>> ExcluirAsync(string contaId, Guid id, CancellationToken ct)
    {
        var item = await dados.ObterOrcamentoFunilAsync(contaId, id, ct);
        if (item is null)
        {
            return Resultado<bool>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        if (item.Situacao != SituacaoOrcamentoFunil.EmAndamento)
        {
            return Resultado<bool>.Erro(Codigos.Validacao, "Só orçamentos em andamento podem ser excluídos.");
        }

        await dados.RemoverOrcamentoFunilAsync(item, ct);
        return Resultado<bool>.Sucesso(true);
    }

    private async Task<Resultado<OrcamentoFunilLeitura>> MudarAsync(
        string contaId,
        Guid id,
        Func<OrcamentoFunil, string?> aplicar,
        CancellationToken ct)
    {
        var item = await dados.ObterOrcamentoFunilAsync(contaId, id, ct);
        if (item is null)
        {
            return Resultado<OrcamentoFunilLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var erro = aplicar(item);
        if (erro is not null)
        {
            return Resultado<OrcamentoFunilLeitura>.Erro(Codigos.Validacao, erro);
        }

        await dados.SalvarNegocioAsync(ct);
        var nomes = await dados.NomesOportunidadesAsync(contaId, [item.OportunidadeId], ct);
        return Resultado<OrcamentoFunilLeitura>.Sucesso(Ler(item, nomes.GetValueOrDefault(item.OportunidadeId, "")));
    }

    private static string AplicarTokens(string texto, string nomeOportunidade) =>
        texto.Replace("#nome_oportunidade", nomeOportunidade, StringComparison.OrdinalIgnoreCase);

    private static OrcamentoFunilLeitura Ler(OrcamentoFunil item, string oportunidadeNome) =>
        new(
            item.Id,
            item.OportunidadeId,
            oportunidadeNome,
            item.Conteudo,
            item.Valor,
            item.Situacao.ToString(),
            item.CriadoEm,
            item.EnviadoEm,
            item.RespondidoEm,
            item.ModeloId);
}
