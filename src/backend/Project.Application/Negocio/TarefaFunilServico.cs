using Project.Application.Comum;
using Project.Domain.Funil;

namespace Project.Application.Negocio;

public sealed record TarefaFunilLeitura(
    Guid Id,
    Guid OportunidadeId,
    string OportunidadeNome,
    string Titulo,
    string? Observacao,
    string Tipo,
    string Responsavel,
    DateOnly Prazo,
    bool Completa,
    Guid? AnexoArquivoId);

public sealed record SalvarTarefaFunil(
    string? Titulo,
    Guid? OportunidadeId,
    string? Tipo,
    string? Responsavel,
    DateOnly? Prazo,
    string? Observacao,
    Guid? AnexoArquivoId);

public sealed record FiltroTarefaFunil(DateOnly? PrazoDe, DateOnly? PrazoAte, string? Tipo, string? Responsavel, bool? Completa);

public sealed class TarefaFunilServico(IDados dados)
{
    public async Task<IReadOnlyList<TarefaFunilLeitura>> ListarAsync(string contaId, FiltroTarefaFunil filtro, CancellationToken ct)
    {
        TipoTarefaFunil? tipo = null;
        if (!string.IsNullOrWhiteSpace(filtro.Tipo) &&
            Enum.TryParse<TipoTarefaFunil>(filtro.Tipo, true, out var lido) &&
            Enum.IsDefined(lido))
        {
            tipo = lido;
        }

        var itens = await dados.ListarTarefasFunilAsync(
            contaId,
            filtro.PrazoDe,
            filtro.PrazoAte,
            tipo,
            filtro.Responsavel,
            filtro.Completa,
            ct);
        var nomes = await dados.NomesOportunidadesAsync(contaId, itens.Select(i => i.OportunidadeId), ct);
        return itens.Select(item => Ler(item, nomes.GetValueOrDefault(item.OportunidadeId, ""))).ToArray();
    }

    public async Task<Resultado<TarefaFunilLeitura>> CriarAsync(string contaId, SalvarTarefaFunil pedido, CancellationToken ct)
    {
        var montado = await MontarAsync(contaId, Guid.NewGuid(), pedido, ct);
        if (!montado.Ok)
        {
            return Resultado<TarefaFunilLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        montado.Valor!.Completa = false;
        await dados.InserirTarefaFunilAsync(montado.Valor, ct);
        var nomes = await dados.NomesOportunidadesAsync(contaId, [montado.Valor.OportunidadeId], ct);
        return Resultado<TarefaFunilLeitura>.Sucesso(Ler(montado.Valor, nomes.GetValueOrDefault(montado.Valor.OportunidadeId, "")));
    }

    public async Task<Resultado<TarefaFunilLeitura>> AtualizarAsync(
        string contaId,
        Guid id,
        SalvarTarefaFunil pedido,
        CancellationToken ct)
    {
        var item = await dados.ObterTarefaFunilAsync(contaId, id, ct);
        if (item is null)
        {
            return Resultado<TarefaFunilLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var montado = await MontarAsync(contaId, item.Id, pedido, ct);
        if (!montado.Ok)
        {
            return Resultado<TarefaFunilLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        item.Titulo = montado.Valor!.Titulo;
        item.OportunidadeId = montado.Valor.OportunidadeId;
        item.Tipo = montado.Valor.Tipo;
        item.Responsavel = montado.Valor.Responsavel;
        item.Prazo = montado.Valor.Prazo;
        item.Observacao = montado.Valor.Observacao;
        item.AnexoArquivoId = montado.Valor.AnexoArquivoId;
        await dados.SalvarNegocioAsync(ct);
        var nomes = await dados.NomesOportunidadesAsync(contaId, [item.OportunidadeId], ct);
        return Resultado<TarefaFunilLeitura>.Sucesso(Ler(item, nomes.GetValueOrDefault(item.OportunidadeId, "")));
    }

    public async Task<Resultado<TarefaFunilLeitura>> ConcluirAsync(string contaId, Guid id, CancellationToken ct)
    {
        var item = await dados.ObterTarefaFunilAsync(contaId, id, ct);
        if (item is null)
        {
            return Resultado<TarefaFunilLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        item.Completa = true;
        await dados.SalvarNegocioAsync(ct);
        var nomes = await dados.NomesOportunidadesAsync(contaId, [item.OportunidadeId], ct);
        return Resultado<TarefaFunilLeitura>.Sucesso(Ler(item, nomes.GetValueOrDefault(item.OportunidadeId, "")));
    }

    public async Task<Resultado<TarefaFunilLeitura>> ReabrirAsync(string contaId, Guid id, CancellationToken ct)
    {
        var item = await dados.ObterTarefaFunilAsync(contaId, id, ct);
        if (item is null)
        {
            return Resultado<TarefaFunilLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        item.Completa = false;
        await dados.SalvarNegocioAsync(ct);
        var nomes = await dados.NomesOportunidadesAsync(contaId, [item.OportunidadeId], ct);
        return Resultado<TarefaFunilLeitura>.Sucesso(Ler(item, nomes.GetValueOrDefault(item.OportunidadeId, "")));
    }

    public async Task<Resultado<bool>> ExcluirAsync(string contaId, Guid id, CancellationToken ct)
    {
        var item = await dados.ObterTarefaFunilAsync(contaId, id, ct);
        if (item is null)
        {
            return Resultado<bool>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        await dados.RemoverTarefaFunilAsync(item, ct);
        return Resultado<bool>.Sucesso(true);
    }

    private async Task<Resultado<TarefaFunil>> MontarAsync(string contaId, Guid id, SalvarTarefaFunil pedido, CancellationToken ct)
    {
        var titulo = Domain.Comum.Texto.Limpo(pedido.Titulo);
        if (titulo is null)
        {
            return Resultado<TarefaFunil>.Erro(Codigos.Validacao, "Informe o título.");
        }

        if (pedido.OportunidadeId is not Guid oportunidadeId ||
            await dados.ObterOportunidadeAsync(contaId, oportunidadeId, ct) is null)
        {
            return Resultado<TarefaFunil>.Erro(Codigos.Validacao, "Informe a oportunidade.");
        }

        if (!Enum.TryParse<TipoTarefaFunil>(pedido.Tipo, true, out var tipo) || !Enum.IsDefined(tipo))
        {
            return Resultado<TarefaFunil>.Erro(Codigos.Validacao, "Informe o tipo da tarefa.");
        }

        var responsavel = Domain.Comum.Texto.Limpo(pedido.Responsavel);
        if (responsavel is null)
        {
            return Resultado<TarefaFunil>.Erro(Codigos.Validacao, "Informe o responsável.");
        }

        if (pedido.Prazo is not DateOnly prazo)
        {
            return Resultado<TarefaFunil>.Erro(Codigos.Validacao, "Informe o prazo.");
        }

        return Resultado<TarefaFunil>.Sucesso(new TarefaFunil
        {
            Id = id,
            ContaId = contaId,
            OportunidadeId = oportunidadeId,
            Titulo = titulo,
            Observacao = Domain.Comum.Texto.Limpo(pedido.Observacao),
            Tipo = tipo,
            Responsavel = responsavel,
            Prazo = prazo,
            AnexoArquivoId = pedido.AnexoArquivoId,
        });
    }

    private static TarefaFunilLeitura Ler(TarefaFunil item, string oportunidadeNome) =>
        new(
            item.Id,
            item.OportunidadeId,
            oportunidadeNome,
            item.Titulo,
            item.Observacao,
            item.Tipo.ToString(),
            item.Responsavel,
            item.Prazo,
            item.Completa,
            item.AnexoArquivoId);
}
