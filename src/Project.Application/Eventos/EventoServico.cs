using Project.Application.Arquivos;
using Project.Application.Cep;
using Project.Domain;
using Project.Domain.Eventos;
using Project.Domain.Tarefas;

namespace Project.Application.Eventos;

public sealed class EventoServico
{
    private readonly IEventoConsulta _consulta;
    private readonly IEventoRepositorio _repositorio;
    private readonly IArmazenamentoArquivo _arquivos;
    private readonly IModeloConsulta _modelos;
    private readonly IConsultaCep _cep;

    public EventoServico(
        IEventoConsulta consulta,
        IEventoRepositorio repositorio,
        IArmazenamentoArquivo arquivos,
        IModeloConsulta modelos,
        IConsultaCep cep)
    {
        _consulta = consulta;
        _repositorio = repositorio;
        _arquivos = arquivos;
        _modelos = modelos;
        _cep = cep;
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
        var compromisso = await _consulta.ObterCompromissoAsync(accountId, id, cancellationToken);
        return ParaDetalhe(evento, indicadores, compromisso);
    }

    public async Task<EventoGravado> CriarAsync(
        string accountId,
        EventoDados dados,
        Stream? imagem,
        string? nomeImagem,
        CancellationToken cancellationToken)
    {
        var modelo = await ExigirModeloAsync(accountId, dados.ModeloId, cancellationToken);
        dados = await CompletarEnderecoAsync(dados, cancellationToken);
        var evento = Evento.CriarComModelo(accountId, modelo, dados.PrimeiroNome, dados.SegundoNome, dados);
        var (arquivoId, aviso) = await TentarImagemAsync(imagem, nomeImagem, cancellationToken);
        if (arquivoId is not null)
        {
            evento.DefinirArquivo(arquivoId);
        }

        var copias = CopiarModelo(accountId, evento, modelo);
        var compromisso = evento.SalvarNaAgenda && evento.Data is DateOnly data
            ? Compromisso.DoEvento(accountId, evento.Id, evento.Nome, data, evento.Hora)
            : null;
        try
        {
            await _repositorio.AdicionarAsync(evento, cancellationToken);
            await _repositorio.AdicionarCopiaAsync(copias.Servicos, copias.Tarefas, compromisso, cancellationToken);
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
        dados = await CompletarEnderecoAsync(dados with
        {
            ModeloId = evento.ModeloId,
            CasalNome = dados.CasalNome ?? evento.CasalNome,
            CasalPapel = dados.CasalPapel ?? evento.CasalPapel,
            CasalEmail = dados.CasalEmail ?? evento.CasalEmail,
            CasalCelular = dados.CasalCelular ?? evento.CasalCelular
        }, cancellationToken);
        evento.Editar(dados);
        if (evento.SalvarNaAgenda && evento.Data is DateOnly data && await _consulta.ObterCompromissoAsync(accountId, id, cancellationToken) is null)
        {
            await _repositorio.AdicionarCopiaAsync([], [], Compromisso.DoEvento(accountId, evento.Id, evento.Nome, data, evento.Hora), cancellationToken);
        }
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

    public Task<IReadOnlyList<string>> ListarCopiasAsync(string accountId, Guid id, CancellationToken cancellationToken)
    {
        return _consulta.ListarCopiasAsync(accountId, id, cancellationToken);
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

    public async Task GravarAssessoriaAsync(
        string accountId,
        Guid eventoId,
        decimal? valorTotal,
        int? quantidade,
        DateOnly? primeira,
        bool comEntrada,
        BaseEntrada? baseEntrada,
        decimal? entrada,
        DateOnly? dataEntrada,
        CancellationToken cancellationToken)
    {
        var evento = await ExigirAsync(accountId, eventoId, cancellationToken);
        if (evento.Situacao != SituacaoEvento.EmAndamento)
        {
            throw new RegraDeNegocioException("O evento não está em andamento.");
        }

        if (await _repositorio.TemAssessoriaAsync(accountId, eventoId, cancellationToken))
        {
            return;
        }

        var servico = ServicoEvento.ContratarAssessoria(accountId, eventoId, "Conta", valorTotal ?? 0);
        var parcelas = ParcelaEvento.Gerar(
            accountId,
            eventoId,
            servico.Id,
            valorTotal ?? 0,
            quantidade ?? 0,
            primeira ?? default,
            comEntrada ? entrada : null,
            comEntrada ? baseEntrada : null,
            comEntrada ? dataEntrada : null);
        await _repositorio.AdicionarAssessoriaAsync(servico, parcelas, cancellationToken);
        await _repositorio.SalvarAsync(cancellationToken);
    }

    private async Task<Modelo> ExigirModeloAsync(string accountId, Guid? modeloId, CancellationToken cancellationToken)
    {
        if (modeloId is null || modeloId == Guid.Empty)
        {
            throw new RegraDeNegocioException("Escolha um modelo.");
        }

        return await _modelos.ObterAsync(accountId, modeloId.Value, cancellationToken)
            ?? throw new RegraDeNegocioException("O modelo não está disponível.");
    }

    private async Task<EventoDados> CompletarEnderecoAsync(EventoDados dados, CancellationToken cancellationToken)
    {
        var cerimonia = await PreencherAsync(dados.Cerimonia, cancellationToken);
        var recepcao = await PreencherAsync(dados.Recepcao, cancellationToken);
        return dados with { Cerimonia = cerimonia, Recepcao = recepcao };
    }

    private async Task<EnderecoLocal?> PreencherAsync(EnderecoLocal? endereco, CancellationToken cancellationToken)
    {
        var atual = (endereco ?? EnderecoLocal.Vazio).Normalizar();
        var digitos = atual.CepDigitos();
        if (digitos is null || digitos.Length != 8)
        {
            return atual;
        }

        var achado = await _cep.ConsultarAsync(digitos, cancellationToken);
        if (achado is null)
        {
            return atual;
        }

        return atual with
        {
            Logradouro = achado.Logradouro,
            Estado = achado.Estado,
            Cidade = achado.Cidade,
            Bairro = achado.Bairro
        };
    }

    private static (IReadOnlyList<ServicoEvento> Servicos, IReadOnlyList<Tarefa> Tarefas) CopiarModelo(
        string accountId,
        Evento evento,
        Modelo modelo)
    {
        var servicos = new List<ServicoEvento>();
        var tarefas = new List<Tarefa>();
        foreach (var servicoModelo in modelo.Servicos)
        {
            var servico = ServicoEvento.Copiar(accountId, evento.Id, servicoModelo.Nome);
            servicos.Add(servico);
            foreach (var tarefaModelo in servicoModelo.Tarefas)
            {
                var tarefa = Tarefa.Criar(accountId, evento.Id, tarefaModelo.Titulo, null, null);
                tarefa.VincularServico(servico.Id);
                tarefas.Add(tarefa);
            }
        }

        return (servicos, tarefas);
    }

    private static EventoDetalhe ParaDetalhe(Evento evento, IndicadoresEvento indicadores, Compromisso? compromisso)
    {
        var cerimonia = new EnderecoLocal(
            evento.CerimoniaPais,
            evento.CerimoniaCep,
            evento.CerimoniaLogradouro,
            evento.CerimoniaNumero,
            evento.CerimoniaEstado,
            evento.CerimoniaCidade,
            evento.CerimoniaBairro);
        var recepcao = new EnderecoLocal(
            evento.RecepcaoPais,
            evento.RecepcaoCep,
            evento.RecepcaoLogradouro,
            evento.RecepcaoNumero,
            evento.RecepcaoEstado,
            evento.RecepcaoCidade,
            evento.RecepcaoBairro);
        var mapa = TextoMapa(evento.LocalCerimonia, cerimonia) ?? TextoMapa(evento.LocalRecepcao, recepcao);
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
            indicadores,
            evento.PrimeiroNome,
            evento.SegundoNome,
            evento.TempoDeFesta,
            evento.InicioCerimonia,
            evento.InicioRecepcao,
            evento.Padrinhos,
            evento.DamasEPajens,
            evento.NotificarAssessor,
            evento.NotificarAssistentes,
            evento.NotificarClientes,
            evento.SalvarNaAgenda,
            evento.SenhaDoSite,
            cerimonia,
            recepcao,
            compromisso is null ? null : $"{compromisso.Nome} {compromisso.Data:dd/MM/yyyy}",
            mapa is null ? null : "https://www.google.com/maps/search/?api=1&query=" + Uri.EscapeDataString(mapa));
    }

    private static string? TextoMapa(string? local, EnderecoLocal endereco)
    {
        var partes = new[] { local, endereco.Logradouro, endereco.Numero, endereco.Bairro, endereco.Cidade, endereco.Estado }
            .Where(parte => !string.IsNullOrWhiteSpace(parte))
            .Select(parte => parte!.Trim())
            .ToArray();
        return partes.Length == 0 ? null : string.Join(", ", partes);
    }
}
