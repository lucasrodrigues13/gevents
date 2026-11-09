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
    private readonly IGrupoRepositorio _gruposRepositorio;
    private readonly IConviteConsulta _consulta;
    private readonly IConviteRepositorio _repositorio;
    private readonly ILeitorPlanilha _planilha;
    private readonly ICodigoQr _qr;

    public ConviteServico(
        IEventoConsulta eventos,
        IGrupoConsulta grupos,
        IGrupoRepositorio gruposRepositorio,
        IConviteConsulta consulta,
        IConviteRepositorio repositorio,
        ILeitorPlanilha planilha,
        ICodigoQr qr)
    {
        _eventos = eventos;
        _grupos = grupos;
        _gruposRepositorio = gruposRepositorio;
        _consulta = consulta;
        _repositorio = repositorio;
        _planilha = planilha;
        _qr = qr;
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

        var convites = await _consulta.ListarAsync(accountId, eventoId, cancellationToken);
        var todos = await _consulta.ListarConvidadosAsync(accountId, eventoId, cancellationToken);
        convites = ComTelefone(convites, todos);
        todos = Enriquecer(todos, convites);
        return new PainelConvites(
            evento.Nome,
            evento.Situacao == SituacaoEvento.EmAndamento,
            await _grupos.ListarAsync(accountId, cancellationToken),
            convites,
            todos,
            Filtrar(todos, filtro),
            evento.TransferAtivo,
            await _consulta.ListarLocaisAsync(accountId, eventoId, cancellationToken));
    }

    public byte[] Modelo()
    {
        return _planilha.Modelo();
    }

    public async Task CriarConviteAsync(
        string accountId,
        Guid eventoId,
        string? nome,
        ConvidadoDados dados,
        Guid? grupoId,
        string? observacao,
        CancellationToken cancellationToken)
    {
        await ExigirAbertoAsync(accountId, eventoId, cancellationToken);
        await ExigirGrupoAsync(accountId, grupoId, cancellationToken);
        await ExigirGrupoAsync(accountId, dados.GrupoId, cancellationToken);
        var criado = Convite.CriarComConvidado(accountId, eventoId, nome, dados);
        criado.Convite.DefinirGrupo(grupoId);
        criado.Convite.DefinirObservacao(observacao);
        await _repositorio.AdicionarAsync(criado.Convite, cancellationToken);
        await _repositorio.AdicionarAsync(criado.Convidado, cancellationToken);
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

    public async Task DefinirPrincipalAsync(
        string accountId,
        Guid eventoId,
        Guid conviteId,
        Guid convidadoId,
        CancellationToken cancellationToken)
    {
        await ExigirAbertoAsync(accountId, eventoId, cancellationToken);
        var convite = await ExigirConviteAsync(accountId, eventoId, conviteId, cancellationToken);
        var convidado = await _consulta.ObterConvidadoAsync(accountId, convidadoId, cancellationToken)
            ?? throw new RegraDeNegocioException("O convidado não está disponível.");
        convite.DefinirPrincipal(convidado);
        await _repositorio.SalvarAsync(cancellationToken);
    }

    public async Task DefinirTelefoneAsync(
        string accountId,
        Guid eventoId,
        Guid conviteId,
        Guid convidadoId,
        string? ddi,
        string? celular,
        CancellationToken cancellationToken)
    {
        await ExigirAbertoAsync(accountId, eventoId, cancellationToken);
        var convite = await ExigirConviteAsync(accountId, eventoId, conviteId, cancellationToken);
        var convidado = await _consulta.ObterConvidadoAsync(accountId, convidadoId, cancellationToken)
            ?? throw new RegraDeNegocioException("O convidado não está disponível.");
        if (convidado.ConviteId != convite.Id)
        {
            throw new RegraDeNegocioException("O convidado não está disponível.");
        }

        convidado.DefinirTelefone(ddi, celular);
        if (convite.ConvidadoPrincipalId == convidado.Id && string.IsNullOrWhiteSpace(convidado.Celular))
        {
            convite.LimparPrincipal();
        }

        await _repositorio.SalvarAsync(cancellationToken);
    }

    public async Task DefinirTransferDoEventoAsync(string accountId, Guid eventoId, bool ativo, CancellationToken cancellationToken)
    {
        var evento = await ExigirAbertoAsync(accountId, eventoId, cancellationToken);
        evento.DefinirTransfer(ativo);
        if (!ativo)
        {
            var convites = await _consulta.ListarDoEventoAsync(accountId, eventoId, cancellationToken);
            foreach (var convite in convites)
            {
                convite.DefinirTransfer(false, false, null);
            }
        }

        await _repositorio.SalvarAsync(cancellationToken);
    }

    public async Task AdicionarLocalAsync(string accountId, Guid eventoId, string? nome, CancellationToken cancellationToken)
    {
        var evento = await ExigirAbertoAsync(accountId, eventoId, cancellationToken);
        if (!evento.TransferAtivo)
        {
            throw new RegraDeNegocioException("O transfer do evento está desligado.");
        }

        var local = LocalTransfer.Criar(accountId, eventoId, nome);
        await _repositorio.AdicionarAsync(local, cancellationToken);
        await _repositorio.SalvarAsync(cancellationToken);
    }

    public async Task DefinirTransferDoConviteAsync(
        string accountId,
        Guid eventoId,
        Guid conviteId,
        bool usa,
        Guid? localId,
        CancellationToken cancellationToken)
    {
        var evento = await ExigirAbertoAsync(accountId, eventoId, cancellationToken);
        var convite = await ExigirConviteAsync(accountId, eventoId, conviteId, cancellationToken);
        if (usa && evento.TransferAtivo)
        {
            if (localId is null || !await _consulta.LocalPertenceAsync(accountId, eventoId, localId.Value, cancellationToken))
            {
                throw new RegraDeNegocioException("Informe o local de origem do transfer.");
            }
        }

        convite.DefinirTransfer(evento.TransferAtivo, usa, localId);
        await _repositorio.SalvarAsync(cancellationToken);
    }

    public async Task ImportarAsync(
        string accountId,
        Guid eventoId,
        Stream arquivo,
        string? nomeArquivo,
        CancellationToken cancellationToken)
    {
        await ExigirAbertoAsync(accountId, eventoId, cancellationToken);
        var linhas = ImportacaoConvites.Interpretar(_planilha.Ler(arquivo, nomeArquivo));
        if (linhas.Count == 0)
        {
            return;
        }

        var grupos = (await _grupos.ListarAsync(accountId, cancellationToken))
            .ToDictionary(grupo => grupo.Nome, grupo => grupo.Id, StringComparer.OrdinalIgnoreCase);
        Convite? atual = null;
        foreach (var linha in linhas)
        {
            if (linha.LinhaDoConvite || atual is null)
            {
                atual = Convite.Criar(accountId, eventoId, linha.NomeConvite);
                atual.DefinirObservacao(linha.ObservacaoConvite);
                if (linha.Grupo is not null)
                {
                    if (!grupos.TryGetValue(linha.Grupo, out var grupoId))
                    {
                        var grupo = Grupo.Criar(accountId, linha.Grupo);
                        await _gruposRepositorio.AdicionarAsync(grupo, cancellationToken);
                        grupoId = grupo.Id;
                        grupos[grupo.Nome] = grupoId;
                    }

                    atual.DefinirGrupo(grupoId);
                }

                await _repositorio.AdicionarAsync(atual, cancellationToken);
            }

            var convidado = Convidado.Criar(atual.Id, linha.Dados);
            await _repositorio.AdicionarAsync(convidado, cancellationToken);
            if (linha.Principal)
            {
                atual.DefinirPrincipal(convidado);
            }
        }

        await _repositorio.SalvarAsync(cancellationToken);
    }

    public async Task<byte[]?> GerarQrAsync(
        string accountId,
        Guid eventoId,
        Guid conviteId,
        string prefixo,
        CancellationToken cancellationToken)
    {
        var convite = await _consulta.ObterAsync(accountId, conviteId, cancellationToken);
        if (convite is null || convite.EventoId != eventoId)
        {
            return null;
        }

        return _qr.Png(prefixo + convite.Link);
    }

    public static string LinkWhatsApp(string? ddi, string? celular, string? mensagem)
    {
        var numero = new string($"{ddi}{celular}".Where(char.IsDigit).ToArray());
        if (numero.Length == 0)
        {
            throw new RegraDeNegocioException("Informe o celular.");
        }

        return "https://wa.me/" + numero + "?text=" + Uri.EscapeDataString(mensagem?.Trim() ?? "");
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

    private async Task<Evento> ExigirAbertoAsync(string accountId, Guid eventoId, CancellationToken cancellationToken)
    {
        var evento = await _eventos.ObterAsync(accountId, eventoId, cancellationToken)
            ?? throw new RegraDeNegocioException("O evento não está disponível.");
        if (evento.Situacao != SituacaoEvento.EmAndamento)
        {
            throw new RegraDeNegocioException("O evento não está em andamento.");
        }

        return evento;
    }

    private async Task<Convite> ExigirConviteAsync(
        string accountId,
        Guid eventoId,
        Guid conviteId,
        CancellationToken cancellationToken)
    {
        var convite = await _consulta.ObterAsync(accountId, conviteId, cancellationToken);
        if (convite is null || convite.EventoId != eventoId)
        {
            throw new RegraDeNegocioException("O convite não está disponível.");
        }

        return convite;
    }

    private async Task ExigirGrupoAsync(string accountId, Guid? grupoId, CancellationToken cancellationToken)
    {
        if (grupoId is not null && !await _grupos.PertenceAsync(accountId, grupoId.Value, cancellationToken))
        {
            throw new RegraDeNegocioException("O grupo não está disponível.");
        }
    }

    private static IReadOnlyList<ConviteItem> ComTelefone(IReadOnlyList<ConviteItem> convites, IReadOnlyList<ConvidadoItem> todos)
    {
        return convites.Select(convite =>
        {
            var principal = convite.ConvidadoPrincipalId is null
                ? null
                : todos.FirstOrDefault(item => item.Id == convite.ConvidadoPrincipalId);
            var telefone = principal is null || string.IsNullOrWhiteSpace(principal.Celular)
                ? null
                : string.IsNullOrWhiteSpace(principal.Ddi) ? principal.Celular : $"{principal.Ddi} {principal.Celular}";
            return convite with { Telefone = telefone };
        }).ToArray();
    }

    private static IReadOnlyList<ConvidadoItem> Enriquecer(IReadOnlyList<ConvidadoItem> todos, IReadOnlyList<ConviteItem> convites)
    {
        var porId = convites.ToDictionary(convite => convite.Id);
        return todos.Select(item => porId.TryGetValue(item.ConviteId, out var convite)
            ? item with
            {
                TelefoneConvite = convite.Telefone,
                GrupoConviteId = convite.GrupoId,
                ObservacaoConvite = convite.Observacao,
                TransferConvite = convite.UsaTransfer
            }
            : item).ToArray();
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
            var texto = filtro.Celular.Trim();
            consulta = consulta.Where(item =>
                Contem(item.Celular, texto)
                || Contem(item.Ddi, texto)
                || Contem(item.TelefoneConvite, texto)
                || Contem(Juntar(item.Ddi, item.Celular), texto));
        }

        if (filtro.Transfer is not null)
        {
            consulta = consulta.Where(item => filtro.Transfer.Value
                ? item.Transfer || item.TransferConvite
                : !item.Transfer && !item.TransferConvite);
        }

        if (filtro.GrupoId is not null)
        {
            consulta = consulta.Where(item => item.GrupoId == filtro.GrupoId || item.GrupoConviteId == filtro.GrupoId);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Observacao))
        {
            var texto = filtro.Observacao.Trim();
            consulta = consulta.Where(item => Contem(item.Observacao, texto) || Contem(item.ObservacaoConvite, texto));
        }

        return consulta.ToArray();
    }

    private static bool Contem(string? valor, string texto)
    {
        return valor is not null && valor.Contains(texto, StringComparison.OrdinalIgnoreCase);
    }

    private static string Juntar(string? ddi, string? celular)
    {
        return string.IsNullOrWhiteSpace(celular)
            ? ""
            : string.IsNullOrWhiteSpace(ddi) ? celular : $"{ddi} {celular}";
    }
}
