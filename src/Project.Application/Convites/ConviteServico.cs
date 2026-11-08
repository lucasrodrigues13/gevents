using Project.Application.Eventos;
using Project.Domain;
using Project.Domain.Convites;
using Project.Domain.Eventos;

namespace Project.Application.Convites;

public sealed class GrupoServico
{
    private readonly IGrupoConsulta _consulta;
    private readonly IGrupoRepositorio _repositorio;

    public GrupoServico(IGrupoConsulta consulta, IGrupoRepositorio repositorio)
    {
        _consulta = consulta;
        _repositorio = repositorio;
    }

    public Task<IReadOnlyList<GrupoItem>> ListarAsync(string accountId, CancellationToken cancellationToken)
    {
        return _consulta.ListarAsync(accountId, cancellationToken);
    }

    public async Task CriarAsync(string accountId, string? nome, CancellationToken cancellationToken)
    {
        var grupo = Grupo.Criar(accountId, nome);
        await _repositorio.AdicionarAsync(grupo, cancellationToken);
        await _repositorio.SalvarAsync(cancellationToken);
    }
}

public sealed class ConviteServico
{
    private readonly IEventoConsulta _eventos;
    private readonly IGrupoConsulta _grupos;
    private readonly IConviteConsulta _consulta;
    private readonly IConviteRepositorio _repositorio;

    public ConviteServico(
        IEventoConsulta eventos,
        IGrupoConsulta grupos,
        IConviteConsulta consulta,
        IConviteRepositorio repositorio)
    {
        _eventos = eventos;
        _grupos = grupos;
        _consulta = consulta;
        _repositorio = repositorio;
    }

    public async Task<PainelConvites?> ObterPainelAsync(
        string accountId,
        Guid eventoId,
        FiltroConvidados filtro,
        CancellationToken cancellationToken)
    {
        var evento = await _eventos.ObterAsync(accountId, eventoId, cancellationToken);
        if (evento is null)
        {
            return null;
        }

        var todos = await _consulta.ListarConvidadosAsync(accountId, eventoId, cancellationToken);
        return new PainelConvites(
            evento.Nome,
            evento.Situacao == SituacaoEvento.EmAndamento,
            await _grupos.ListarAsync(accountId, cancellationToken),
            await _consulta.ListarAsync(accountId, eventoId, cancellationToken),
            todos,
            Filtrar(todos, filtro));
    }

    public async Task CriarConviteAsync(string accountId, Guid eventoId, string? nome, CancellationToken cancellationToken)
    {
        await ExigirAbertoAsync(accountId, eventoId, cancellationToken);
        var convite = Convite.Criar(accountId, eventoId, nome);
        await _repositorio.AdicionarAsync(convite, cancellationToken);
        await _repositorio.SalvarAsync(cancellationToken);
    }

    public async Task CriarConvidadoAsync(
        string accountId,
        Guid eventoId,
        Guid conviteId,
        ConvidadoDados dados,
        CancellationToken cancellationToken)
    {
        await ExigirAbertoAsync(accountId, eventoId, cancellationToken);
        var convite = await _consulta.ObterAsync(accountId, conviteId, cancellationToken);
        if (convite is null || convite.EventoId != eventoId)
        {
            throw new RegraDeNegocioException("O convite não está disponível.");
        }

        if (dados.GrupoId is not null && !await _grupos.PertenceAsync(accountId, dados.GrupoId.Value, cancellationToken))
        {
            throw new RegraDeNegocioException("O grupo não está disponível.");
        }

        var convidado = Convidado.Criar(convite.Id, dados);
        await _repositorio.AdicionarAsync(convidado, cancellationToken);
        await _repositorio.SalvarAsync(cancellationToken);
    }

    public async Task<ConvitePublico?> AbrirAsync(Guid link, DateTime momentoUtc, CancellationToken cancellationToken)
    {
        var convite = await _consulta.ObterPorLinkAsync(link, cancellationToken);
        if (convite is null)
        {
            return null;
        }

        if (!convite.RespostaRegistrada)
        {
            convite.RegistrarAcesso(momentoUtc);
            await _repositorio.SalvarAsync(cancellationToken);
        }

        return await MontarPublicoAsync(convite, cancellationToken);
    }

    public async Task<ConvitePublico?> ObterPublicoAsync(Guid link, CancellationToken cancellationToken)
    {
        var convite = await _consulta.ObterPorLinkAsync(link, cancellationToken);
        return convite is null ? null : await MontarPublicoAsync(convite, cancellationToken);
    }

    public async Task<string?> ResponderAsync(
        Guid link,
        IReadOnlyList<(Guid ConvidadoId, string? Situacao)> respostas,
        DateTime momentoUtc,
        CancellationToken cancellationToken)
    {
        var convite = await _consulta.ObterPorLinkAsync(link, cancellationToken);
        if (convite is null)
        {
            return null;
        }

        if (convite.RespostaRegistrada)
        {
            return "A resposta já foi registrada.";
        }

        var convidados = await _consulta.ListarConvidadosDoConviteAsync(convite.Id, cancellationToken);
        if (convidados.Count == 0)
        {
            throw new RegraDeNegocioException("Não há quem responder.");
        }

        if (respostas.Count != convidados.Count
            || respostas.Select(item => item.ConvidadoId).Distinct().Count() != convidados.Count
            || respostas.Any(item => convidados.All(convidado => convidado.Id != item.ConvidadoId)))
        {
            throw new RegraDeNegocioException("Não foi possível salvar a resposta.");
        }

        var presencas = new Dictionary<Guid, SituacaoConvidado>();
        foreach (var resposta in respostas)
        {
            if (!ListasDeConvidado.PresencaPublica(resposta.Situacao, out var situacao))
            {
                throw new RegraDeNegocioException("Não foi possível salvar a resposta.");
            }

            presencas[resposta.ConvidadoId] = situacao;
        }

        foreach (var convidado in convidados)
        {
            convidado.Responder(presencas[convidado.Id]);
        }

        convite.RegistrarResposta(momentoUtc);
        await _repositorio.SalvarAsync(cancellationToken);
        return "Resposta registrada.";
    }

    private async Task<ConvitePublico> MontarPublicoAsync(Convite convite, CancellationToken cancellationToken)
    {
        var convidados = await _consulta.ListarConvidadosDoConviteAsync(convite.Id, cancellationToken);
        return new ConvitePublico(
            convite.Nome,
            convite.RespostaRegistrada,
            convidados.Select(convidado => new PresencaPublica(convidado.Id, convidado.Nome, convidado.Situacao)).ToArray());
    }

    private async Task ExigirAbertoAsync(string accountId, Guid eventoId, CancellationToken cancellationToken)
    {
        var evento = await _eventos.ObterAsync(accountId, eventoId, cancellationToken)
            ?? throw new RegraDeNegocioException("O evento não está disponível.");
        if (evento.Situacao != SituacaoEvento.EmAndamento)
        {
            throw new RegraDeNegocioException("O evento não está em andamento.");
        }
    }

    private static IReadOnlyList<ConvidadoItem> Filtrar(IReadOnlyList<ConvidadoItem> todos, FiltroConvidados filtro)
    {
        IEnumerable<ConvidadoItem> consulta = todos;
        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            consulta = consulta.Where(item =>
                item.Nome.Contains(filtro.Nome.Trim(), StringComparison.OrdinalIgnoreCase)
                || item.ConviteNome.Contains(filtro.Nome.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (filtro.Situacao is not null)
        {
            consulta = consulta.Where(item => item.Situacao == filtro.Situacao);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Celular))
        {
            consulta = consulta.Where(item => item.Celular is not null && item.Celular.Contains(filtro.Celular.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (filtro.Transfer is not null)
        {
            consulta = consulta.Where(item => item.Transfer == filtro.Transfer);
        }

        if (filtro.GrupoId is not null)
        {
            consulta = consulta.Where(item => item.GrupoId == filtro.GrupoId);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Observacao))
        {
            consulta = consulta.Where(item => item.Observacao is not null && item.Observacao.Contains(filtro.Observacao.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        return consulta.ToArray();
    }
}
