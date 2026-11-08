using Project.Application.Arquivos;
using Project.Domain;
using Project.Domain.Eventos;

namespace Project.Application.Eventos;

public sealed class EventoServico
{
    private readonly IEventoConsulta _consulta;
    private readonly IEventoRepositorio _repositorio;
    private readonly IArmazenamentoArquivo _arquivos;

    public EventoServico(IEventoConsulta consulta, IEventoRepositorio repositorio, IArmazenamentoArquivo arquivos)
    {
        _consulta = consulta;
        _repositorio = repositorio;
        _arquivos = arquivos;
    }

    public Task<IReadOnlyList<EventoResumo>> ListarAsync(string accountId, FiltroEventos filtro, CancellationToken cancellationToken)
    {
        return _consulta.ListarAsync(accountId, filtro, cancellationToken);
    }

    public async Task<EventoDetalhe?> ObterAsync(string accountId, Guid id, CancellationToken cancellationToken)
    {
        var evento = await _consulta.ObterAsync(accountId, id, cancellationToken);
        if (evento is null)
        {
            return null;
        }

        var indicadores = await _consulta.ContarIndicadoresAsync(accountId, id, cancellationToken);
        return ParaDetalhe(evento, indicadores);
    }

    public async Task<EventoGravado> CriarAsync(
        string accountId,
        EventoDados dados,
        Stream? imagem,
        string? nomeImagem,
        CancellationToken cancellationToken)
    {
        var evento = Evento.Criar(accountId, dados);
        var (arquivoId, aviso) = await TentarImagemAsync(imagem, nomeImagem, cancellationToken);
        if (arquivoId is not null)
        {
            evento.DefinirArquivo(arquivoId);
        }

        try
        {
            await _repositorio.AdicionarAsync(evento, cancellationToken);
            await _repositorio.SalvarAsync(cancellationToken);
        }
        catch
        {
            if (arquivoId is not null)
            {
                await _arquivos.ExcluirAsync(arquivoId.Value, cancellationToken);
            }

            throw;
        }

        return new EventoGravado(evento.Id, aviso);
    }

    public async Task<EventoGravado> EditarAsync(
        string accountId,
        Guid id,
        EventoDados dados,
        Stream? imagem,
        string? nomeImagem,
        CancellationToken cancellationToken)
    {
        var evento = await ExigirAsync(accountId, id, cancellationToken);
        evento.Editar(dados);
        var anterior = evento.ArquivoId;
        var (novo, aviso) = await TentarImagemAsync(imagem, nomeImagem, cancellationToken);
        if (novo is not null)
        {
            evento.DefinirArquivo(novo);
        }

        try
        {
            await _repositorio.SalvarAsync(cancellationToken);
        }
        catch
        {
            if (novo is not null)
            {
                await _arquivos.ExcluirAsync(novo.Value, cancellationToken);
            }

            throw;
        }

        if (novo is not null && anterior is not null)
        {
            await _arquivos.ExcluirAsync(anterior.Value, cancellationToken);
        }

        return new EventoGravado(evento.Id, aviso);
    }

    public Task ConcluirAsync(string accountId, Guid id, CancellationToken cancellationToken)
    {
        return EncerrarAsync(accountId, id, evento => evento.Concluir(), cancellationToken);
    }

    public Task CancelarAsync(string accountId, Guid id, CancellationToken cancellationToken)
    {
        return EncerrarAsync(accountId, id, evento => evento.Cancelar(), cancellationToken);
    }

    public async Task<ArquivoBaixado?> AbrirImagemAsync(string accountId, Guid id, CancellationToken cancellationToken)
    {
        var evento = await _consulta.ObterAsync(accountId, id, cancellationToken);
        if (evento?.ArquivoId is null)
        {
            return null;
        }

        return await _arquivos.AbrirAsync(evento.ArquivoId.Value, cancellationToken);
    }

    private async Task EncerrarAsync(string accountId, Guid id, Action<Evento> acao, CancellationToken cancellationToken)
    {
        var evento = await ExigirAsync(accountId, id, cancellationToken);
        acao(evento);
        await _repositorio.SalvarAsync(cancellationToken);
    }

    private async Task<(Guid? Id, string? Aviso)> TentarImagemAsync(Stream? imagem, string? nome, CancellationToken cancellationToken)
    {
        if (imagem is null)
        {
            return (null, null);
        }

        try
        {
            var id = await _arquivos.SalvarImagemAsync(imagem, nome ?? "imagem", cancellationToken);
            return (id, null);
        }
        catch (RegraDeNegocioException exception)
        {
            return (null, exception.Message);
        }
    }

    private async Task<Evento> ExigirAsync(string accountId, Guid id, CancellationToken cancellationToken)
    {
        return await _consulta.ObterAsync(accountId, id, cancellationToken)
            ?? throw new RegraDeNegocioException("O evento não está disponível.");
    }

    private static EventoDetalhe ParaDetalhe(Evento evento, IndicadoresEvento indicadores)
    {
        return new EventoDetalhe(
            evento.Id,
            evento.Nome,
            evento.Situacao,
            evento.Data,
            evento.Hora,
            evento.Tipo,
            evento.Tags.ToArray(),
            evento.CasalNome,
            evento.CasalPapel,
            evento.CasalEmail,
            evento.CasalCelular,
            evento.OrcamentoEstimado,
            evento.ConvidadosEstimados,
            evento.LocalCerimonia,
            evento.LocalRecepcao,
            evento.ArquivoId,
            indicadores);
    }
}
