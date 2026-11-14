using Project.Application.Comum;
using Project.Domain.Eventos;

namespace Project.Application.Negocio;

public sealed record CompromissoLeitura(
    Guid Id,
    string Descricao,
    DateOnly Data,
    TimeOnly? Hora,
    Guid? EventoId,
    bool DeEvento);

public sealed record SalvarCompromisso(string? Descricao, DateOnly? Data, TimeOnly? Hora);

public sealed class AgendaServico(IDados dados)
{
    public async Task<IReadOnlyList<CompromissoLeitura>> ListarAsync(
        string contaId,
        DateOnly? de,
        DateOnly? ate,
        CancellationToken ct)
    {
        var itens = await dados.ListarCompromissosAsync(contaId, de, ate, ct);
        return itens.Select(Ler).ToArray();
    }

    public async Task<Resultado<CompromissoLeitura>> CriarAsync(string contaId, SalvarCompromisso pedido, CancellationToken ct)
    {
        var montado = Montar(contaId, Guid.NewGuid(), pedido, null);
        if (!montado.Ok)
        {
            return Resultado<CompromissoLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        await dados.InserirCompromissoAsync(montado.Valor!, ct);
        return Resultado<CompromissoLeitura>.Sucesso(Ler(montado.Valor!));
    }

    public async Task<Resultado<CompromissoLeitura>> AtualizarAsync(
        string contaId,
        Guid id,
        SalvarCompromisso pedido,
        CancellationToken ct)
    {
        var item = await dados.ObterCompromissoAsync(contaId, id, ct);
        if (item is null)
        {
            return Resultado<CompromissoLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        if (item.EventoId is not null)
        {
            return Resultado<CompromissoLeitura>.Erro(Codigos.Validacao, "Compromisso de evento não é editado nesta tela.");
        }

        var montado = Montar(contaId, item.Id, pedido, null);
        if (!montado.Ok)
        {
            return Resultado<CompromissoLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        item.Descricao = montado.Valor!.Descricao;
        item.Data = montado.Valor.Data;
        item.Hora = montado.Valor.Hora;
        await dados.SalvarNegocioAsync(ct);
        return Resultado<CompromissoLeitura>.Sucesso(Ler(item));
    }

    public async Task<Resultado<bool>> ExcluirAsync(string contaId, Guid id, CancellationToken ct)
    {
        var item = await dados.ObterCompromissoAsync(contaId, id, ct);
        if (item is null)
        {
            return Resultado<bool>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        if (item.EventoId is not null)
        {
            return Resultado<bool>.Erro(Codigos.Validacao, "Compromisso de evento não é excluído nesta tela.");
        }

        await dados.RemoverCompromissoAsync(item, ct);
        return Resultado<bool>.Sucesso(true);
    }

    public Task<bool> TemCompromissoNaDataAsync(string contaId, DateOnly data, CancellationToken ct) =>
        dados.TemCompromissoNaDataAsync(contaId, data, ct);

    private static Resultado<CompromissoAgenda> Montar(
        string contaId,
        Guid id,
        SalvarCompromisso pedido,
        Guid? eventoId)
    {
        var descricao = Domain.Comum.Texto.Limpo(pedido.Descricao);
        if (descricao is null)
        {
            return Resultado<CompromissoAgenda>.Erro(Codigos.Validacao, "Informe a descrição.");
        }

        if (pedido.Data is not DateOnly data)
        {
            return Resultado<CompromissoAgenda>.Erro(Codigos.Validacao, "Informe a data.");
        }

        return Resultado<CompromissoAgenda>.Sucesso(new CompromissoAgenda
        {
            Id = id,
            ContaId = contaId,
            EventoId = eventoId,
            Descricao = descricao,
            Data = data,
            Hora = pedido.Hora,
        });
    }

    private static CompromissoLeitura Ler(CompromissoAgenda item) =>
        new(item.Id, item.Descricao, item.Data!.Value, item.Hora, item.EventoId, item.EventoId is not null);
}
