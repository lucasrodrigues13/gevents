using Project.Application.Comum;
using Project.Domain.Funil;

namespace Project.Application.Negocio;

public sealed record ContratoFunilLeitura(
    Guid Id,
    Guid OportunidadeId,
    string OportunidadeNome,
    string Conteudo,
    string Situacao,
    DateTimeOffset CriadoEm,
    DateTimeOffset? EnviadoEm,
    DateTimeOffset? AssinadoEm,
    Guid? ModeloId);

public sealed record SalvarContratoFunil(Guid? OportunidadeId, string? Conteudo, Guid? ModeloId);

public sealed class ContratoFunilServico(IDados dados)
{
    public async Task<IReadOnlyList<ContratoFunilLeitura>> ListarAsync(
        string contaId,
        string? situacao,
        string? busca,
        DateOnly? criadoDe,
        DateOnly? criadoAte,
        CancellationToken ct)
    {
        SituacaoContratoFunil? filtro = null;
        if (!string.IsNullOrWhiteSpace(situacao) &&
            Enum.TryParse<SituacaoContratoFunil>(situacao, true, out var lido) &&
            Enum.IsDefined(lido))
        {
            filtro = lido;
        }

        var itens = await dados.ListarContratosFunilAsync(contaId, filtro, busca, criadoDe, criadoAte, ct);
        var nomes = await dados.NomesOportunidadesAsync(contaId, itens.Select(i => i.OportunidadeId), ct);
        return itens.Select(item => Ler(item, nomes.GetValueOrDefault(item.OportunidadeId, ""))).ToArray();
    }

    public async Task<Resultado<ContratoFunilLeitura>> ObterAsync(string contaId, Guid id, CancellationToken ct)
    {
        var item = await dados.ObterContratoFunilAsync(contaId, id, ct);
        if (item is null)
        {
            return Resultado<ContratoFunilLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var nomes = await dados.NomesOportunidadesAsync(contaId, [item.OportunidadeId], ct);
        return Resultado<ContratoFunilLeitura>.Sucesso(Ler(item, nomes.GetValueOrDefault(item.OportunidadeId, "")));
    }

    public async Task<Resultado<ContratoFunilLeitura>> CriarAsync(string contaId, SalvarContratoFunil pedido, CancellationToken ct)
    {
        if (pedido.OportunidadeId is not Guid oportunidadeId ||
            await dados.ObterOportunidadeAsync(contaId, oportunidadeId, ct) is not { } oportunidade)
        {
            return Resultado<ContratoFunilLeitura>.Erro(Codigos.Validacao, "Informe a oportunidade.");
        }

        var conteudo = Domain.Comum.Texto.Limpo(pedido.Conteudo);
        Guid? modeloId = pedido.ModeloId;
        if (conteudo is null && modeloId is Guid mid)
        {
            var modelo = await dados.ObterModeloContratoFunilAsync(contaId, mid, ct);
            if (modelo is null)
            {
                return Resultado<ContratoFunilLeitura>.Erro(Codigos.Validacao, "Modelo inválido.");
            }

            conteudo = AplicarTokens(modelo.Texto, oportunidade.Nome);
        }
        else if (conteudo is not null && modeloId is Guid mid2)
        {
            var modelo = await dados.ObterModeloContratoFunilAsync(contaId, mid2, ct);
            if (modelo is null)
            {
                return Resultado<ContratoFunilLeitura>.Erro(Codigos.Validacao, "Modelo inválido.");
            }
        }

        if (conteudo is null)
        {
            return Resultado<ContratoFunilLeitura>.Erro(Codigos.Validacao, "Informe o conteúdo do contrato.");
        }

        var item = new ContratoFunil
        {
            Id = Guid.NewGuid(),
            ContaId = contaId,
            OportunidadeId = oportunidadeId,
            Conteudo = conteudo,
            Situacao = SituacaoContratoFunil.EmAndamento,
            CriadoEm = DateTimeOffset.UtcNow,
            ModeloId = modeloId,
        };
        await dados.InserirContratoFunilAsync(item, ct);
        return Resultado<ContratoFunilLeitura>.Sucesso(Ler(item, oportunidade.Nome));
    }

    public async Task<Resultado<ContratoFunilLeitura>> AtualizarAsync(
        string contaId,
        Guid id,
        SalvarContratoFunil pedido,
        CancellationToken ct)
    {
        var item = await dados.ObterContratoFunilAsync(contaId, id, ct);
        if (item is null)
        {
            return Resultado<ContratoFunilLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        if (item.Situacao != SituacaoContratoFunil.EmAndamento)
        {
            return Resultado<ContratoFunilLeitura>.Erro(Codigos.Validacao, "Só contratos em andamento podem ser editados.");
        }

        var conteudo = Domain.Comum.Texto.Limpo(pedido.Conteudo);
        if (conteudo is null)
        {
            return Resultado<ContratoFunilLeitura>.Erro(Codigos.Validacao, "Informe o conteúdo do contrato.");
        }

        item.Conteudo = conteudo;
        await dados.SalvarNegocioAsync(ct);
        var nomes = await dados.NomesOportunidadesAsync(contaId, [item.OportunidadeId], ct);
        return Resultado<ContratoFunilLeitura>.Sucesso(Ler(item, nomes.GetValueOrDefault(item.OportunidadeId, "")));
    }

    public Task<Resultado<ContratoFunilLeitura>> EnviarAsync(string contaId, Guid id, CancellationToken ct) =>
        MudarAsync(contaId, id, item =>
        {
            if (item.Situacao != SituacaoContratoFunil.EmAndamento)
            {
                return "Só contratos em andamento podem ser enviados.";
            }

            item.EnviadoEm = DateTimeOffset.UtcNow;
            return null;
        }, ct);

    public Task<Resultado<ContratoFunilLeitura>> AssinarAsync(string contaId, Guid id, CancellationToken ct) =>
        MudarAsync(contaId, id, item =>
        {
            if (item.Situacao != SituacaoContratoFunil.EmAndamento)
            {
                return "Só contratos em andamento podem ser assinados.";
            }

            item.Situacao = SituacaoContratoFunil.Assinado;
            item.AssinadoEm = DateTimeOffset.UtcNow;
            return null;
        }, ct);

    public Task<Resultado<ContratoFunilLeitura>> CancelarAsync(string contaId, Guid id, CancellationToken ct) =>
        MudarAsync(contaId, id, item =>
        {
            if (item.Situacao == SituacaoContratoFunil.Cancelado)
            {
                return "Contrato já cancelado.";
            }

            item.Situacao = SituacaoContratoFunil.Cancelado;
            return null;
        }, ct);

    public async Task<Resultado<bool>> ExcluirAsync(string contaId, Guid id, CancellationToken ct)
    {
        var item = await dados.ObterContratoFunilAsync(contaId, id, ct);
        if (item is null)
        {
            return Resultado<bool>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        if (item.Situacao != SituacaoContratoFunil.EmAndamento)
        {
            return Resultado<bool>.Erro(Codigos.Validacao, "Só contratos em andamento podem ser excluídos.");
        }

        await dados.RemoverContratoFunilAsync(item, ct);
        return Resultado<bool>.Sucesso(true);
    }

    private async Task<Resultado<ContratoFunilLeitura>> MudarAsync(
        string contaId,
        Guid id,
        Func<ContratoFunil, string?> aplicar,
        CancellationToken ct)
    {
        var item = await dados.ObterContratoFunilAsync(contaId, id, ct);
        if (item is null)
        {
            return Resultado<ContratoFunilLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var erro = aplicar(item);
        if (erro is not null)
        {
            return Resultado<ContratoFunilLeitura>.Erro(Codigos.Validacao, erro);
        }

        await dados.SalvarNegocioAsync(ct);
        var nomes = await dados.NomesOportunidadesAsync(contaId, [item.OportunidadeId], ct);
        return Resultado<ContratoFunilLeitura>.Sucesso(Ler(item, nomes.GetValueOrDefault(item.OportunidadeId, "")));
    }

    private static string AplicarTokens(string texto, string nomeOportunidade) =>
        texto.Replace("#nome_oportunidade", nomeOportunidade, StringComparison.OrdinalIgnoreCase);

    private static ContratoFunilLeitura Ler(ContratoFunil item, string oportunidadeNome) =>
        new(
            item.Id,
            item.OportunidadeId,
            oportunidadeNome,
            item.Conteudo,
            item.Situacao.ToString(),
            item.CriadoEm,
            item.EnviadoEm,
            item.AssinadoEm,
            item.ModeloId);
}
