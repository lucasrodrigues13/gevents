using Project.Application.Comum;
using Project.Domain.Eventos;
using Project.Domain.Tarefas;

namespace Project.Application.Negocio;

public sealed record SalvarTarefa(string? Titulo, string? Descricao, DateOnly? Prazo, SituacaoTarefa? Situacao);

public sealed record TarefaLeitura(
    Guid Id,
    Guid EventoId,
    string Evento,
    string Titulo,
    string? Descricao,
    DateOnly? Prazo,
    SituacaoTarefa Situacao,
    string ResponsavelContaId);

public sealed class TarefaServico(IDados dados)
{
    public async Task<Resultado<IReadOnlyList<TarefaLeitura>>> DoEventoAsync(string contaId, Guid eventoId, CancellationToken ct)
    {
        if (!await dados.EventoDaContaAsync(contaId, eventoId, ct))
        {
            return Resultado<IReadOnlyList<TarefaLeitura>>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var tarefas = await dados.ListarTarefasAsync(contaId, eventoId, ct);
        var nome = await dados.NomeDoEventoAsync(contaId, eventoId, ct) ?? "";
        return Resultado<IReadOnlyList<TarefaLeitura>>.Sucesso(tarefas.Select(tarefa => Ler(tarefa, nome)).ToArray());
    }

    public async Task<IReadOnlyList<TarefaLeitura>> DaContaAsync(string contaId, CancellationToken ct)
    {
        var tarefas = await dados.ListarTarefasAsync(contaId, null, ct);
        var lista = new List<TarefaLeitura>();
        foreach (var tarefa in tarefas)
        {
            var nome = await dados.NomeDoEventoAsync(contaId, tarefa.EventoId, ct) ?? "";
            lista.Add(Ler(tarefa, nome));
        }

        return lista;
    }

    public async Task<Resultado<TarefaLeitura>> CriarAsync(string contaId, Guid eventoId, SalvarTarefa pedido, CancellationToken ct)
    {
        var situacaoEvento = await dados.SituacaoDoEventoAsync(contaId, eventoId, ct);
        if (situacaoEvento is null)
        {
            return Resultado<TarefaLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        if (situacaoEvento != SituacaoEvento.EmAndamento)
        {
            return Resultado<TarefaLeitura>.Erro(Codigos.Validacao, "O evento não está em andamento.");
        }

        var titulo = Domain.Comum.Texto.Limpo(pedido.Titulo);
        if (titulo is null)
        {
            return Resultado<TarefaLeitura>.Erro(Codigos.Validacao, "Informe o título.");
        }

        var tarefa = new Tarefa
        {
            Id = Guid.NewGuid(),
            EventoId = eventoId,
            ContaId = contaId,
            Titulo = titulo,
            Descricao = Domain.Comum.Texto.Limpo(pedido.Descricao),
            Prazo = pedido.Prazo,
            Situacao = SituacaoTarefa.Pendente,
            ResponsavelContaId = contaId,
        };
        await dados.InserirTarefaAsync(tarefa, ct);
        var nome = await dados.NomeDoEventoAsync(contaId, eventoId, ct) ?? "";
        return Resultado<TarefaLeitura>.Sucesso(Ler(tarefa, nome));
    }

    public async Task<Resultado<TarefaLeitura>> AtualizarAsync(string contaId, Guid id, SalvarTarefa pedido, CancellationToken ct)
    {
        var tarefa = await dados.ObterTarefaAsync(contaId, id, ct);
        if (tarefa is null)
        {
            return Resultado<TarefaLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var situacaoEvento = await dados.SituacaoDoEventoAsync(contaId, tarefa.EventoId, ct);
        if (situacaoEvento != SituacaoEvento.EmAndamento)
        {
            return Resultado<TarefaLeitura>.Erro(Codigos.Validacao, "O evento não está em andamento.");
        }

        var titulo = Domain.Comum.Texto.Limpo(pedido.Titulo);
        if (titulo is null)
        {
            return Resultado<TarefaLeitura>.Erro(Codigos.Validacao, "Informe o título.");
        }

        if (pedido.Situacao is not SituacaoTarefa situacao || !Enum.IsDefined(situacao))
        {
            return Resultado<TarefaLeitura>.Erro(Codigos.Validacao, "Escolha a situação da tarefa.");
        }

        tarefa.Titulo = titulo;
        tarefa.Descricao = Domain.Comum.Texto.Limpo(pedido.Descricao);
        tarefa.Prazo = pedido.Prazo;
        tarefa.Situacao = situacao;
        await dados.SalvarTarefaAsync(ct);
        var nome = await dados.NomeDoEventoAsync(contaId, tarefa.EventoId, ct) ?? "";
        return Resultado<TarefaLeitura>.Sucesso(Ler(tarefa, nome));
    }

    private static TarefaLeitura Ler(Tarefa tarefa, string evento) => new(
        tarefa.Id,
        tarefa.EventoId,
        evento,
        tarefa.Titulo,
        tarefa.Descricao,
        tarefa.Prazo,
        tarefa.Situacao,
        tarefa.ResponsavelContaId);
}
