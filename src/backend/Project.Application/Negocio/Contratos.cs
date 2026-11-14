using Project.Domain.Clientes;
using Project.Domain.Convidados;
using Project.Domain.Eventos;
using Project.Domain.Fornecedores;
using Project.Domain.Funil;
using Project.Domain.Modelos;
using Project.Domain.Perfil;
using Project.Domain.Tarefas;

namespace Project.Application.Negocio;

public sealed record FiltroEventos(
    SituacaoEvento? Situacao,
    string? Nome,
    TipoEvento? Tipo,
    DateOnly? De,
    DateOnly? Ate,
    string? Tag);

public sealed record FiltroConvites(
    string? Nome,
    SituacaoConvidado? Situacao,
    string? Telefone,
    bool? Transfer,
    Guid? GrupoId,
    string? Observacao,
    bool SomenteConfirmados);

public sealed record EventoLinha(
    Guid Id,
    string Nome,
    SituacaoEvento Situacao,
    TipoEvento Tipo,
    DateOnly? Data,
    IReadOnlyList<string> Tags,
    int TarefasConcluidas,
    int Tarefas,
    int ConvidadosConfirmados,
    int Convidados);

public sealed record ContagemSituacao(SituacaoEvento Situacao, int Quantidade);

public sealed record EventoAndamento(Guid Id, string Nome, DateOnly? Data);

public sealed record ContagemMes(int Mes, int Quantidade);

public sealed record ContagemNome(string Nome, int Quantidade);

public sealed record ParticipacaoNome(string Nome, decimal Valor);

public sealed record ContagemOportunidadeEstagio(EstagioOportunidade Estagio, int Quantidade, decimal Valor);

public interface IDados
{
    Task<PerfilAssessoria?> ObterPerfilAsync(string contaId, CancellationToken ct);

    Task SalvarPerfilAsync(PerfilAssessoria perfil, CancellationToken ct);

    Task<IReadOnlyList<ModeloEvento>> ListarModelosAsync(string contaId, string? nome, CancellationToken ct);

    Task<ModeloEvento?> ObterModeloAsync(string contaId, Guid id, CancellationToken ct);

    Task InserirModeloAsync(ModeloEvento modelo, CancellationToken ct);

    Task AtualizarModeloAsync(CancellationToken ct);

    Task RemoverModeloAsync(ModeloEvento modelo, CancellationToken ct);

    Task<IReadOnlyList<EventoLinha>> ListarEventosAsync(string contaId, FiltroEventos filtro, CancellationToken ct);

    Task<Evento?> ObterEventoAsync(string contaId, Guid id, CancellationToken ct);

    Task<bool> EventoDaContaAsync(string contaId, Guid id, CancellationToken ct);

    Task InserirEventoAsync(
        Evento evento,
        IReadOnlyList<Tarefa> tarefas,
        CompromissoAgenda? agenda,
        Cliente? clienteNovo,
        CancellationToken ct);

    Task SalvarEventoAsync(CancellationToken ct);

    Task SubstituirTagsAsync(Evento evento, IReadOnlyList<string> nomes, CancellationToken ct);

    Task SincronizarAgendaAsync(Evento evento, CancellationToken ct);

    Task<bool> TemAssessoriaAsync(Guid eventoId, CancellationToken ct);

    Task AdicionarServicoAsync(ServicoEvento servico, CancellationToken ct);

    Task<ServicoEvento?> ObterServicoEventoAsync(string contaId, Guid eventoId, Guid servicoId, CancellationToken ct);

    Task RemoverConteudoModeloAsync(ModeloEvento modelo, CancellationToken ct);

    Task GravarModeloAtualizadoAsync(ModeloEvento modelo, CancellationToken ct);

    Task InserirFornecedorNoServicoAsync(FornecedorNoServico vinculo, CancellationToken ct);

    Task SubstituirSubitensAsync(Guid servicoId, IReadOnlyList<SubitemServico> subitens, decimal? valorEstimado, CancellationToken ct);

    Task SubstituirParcelasAsync(Guid servicoId, IReadOnlyList<ParcelaServico> parcelas, CancellationToken ct);

    Task AtualizarEtapaNegociacaoAsync(Guid vinculoId, EtapaNegociacao etapa, decimal? valorOrcamento, CancellationToken ct);

    Task MarcarServicoContratadoAsync(Guid servicoId, bool contratado, decimal? valorContratado, CancellationToken ct);

    Task<IReadOnlyList<Tarefa>> ListarTarefasAsync(string contaId, Guid? eventoId, CancellationToken ct);

    Task<Tarefa?> ObterTarefaAsync(string contaId, Guid id, CancellationToken ct);

    Task InserirTarefaAsync(Tarefa tarefa, CancellationToken ct);

    Task SalvarTarefaAsync(CancellationToken ct);

    Task<string?> NomeDoEventoAsync(string contaId, Guid eventoId, CancellationToken ct);

    Task<IReadOnlyList<Cliente>> ListarClientesAsync(string contaId, string? nome, CancellationToken ct);

    Task<Cliente?> ObterClienteAsync(string contaId, Guid id, CancellationToken ct);

    Task<Cliente?> ObterClientePorUsuarioAsync(string contaId, string usuarioId, CancellationToken ct);

    Task InserirClienteAsync(Cliente cliente, CancellationToken ct);

    Task SalvarClienteAsync(CancellationToken ct);

    Task<IReadOnlyList<(Guid Id, string Nome)>> EventosDoClienteAsync(string contaId, Guid clienteId, CancellationToken ct);

    Task<IReadOnlyList<Guid>> EventosDoUsuarioClienteAsync(string contaId, string usuarioId, CancellationToken ct);

    Task<bool> ClienteVinculadoAoEventoAsync(string contaId, string usuarioId, Guid eventoId, CancellationToken ct);

    Task<IReadOnlyList<Cliente>> ClientesDoEventoAsync(string contaId, Guid eventoId, CancellationToken ct);

    Task<bool> VincularAsync(string contaId, Guid clienteId, Guid eventoId, CancellationToken ct);

    Task<bool> DesvincularAsync(string contaId, Guid clienteId, Guid eventoId, CancellationToken ct);

    Task<IReadOnlyList<GrupoConvidados>> ListarGruposAsync(string contaId, CancellationToken ct);

    Task InserirGrupoAsync(GrupoConvidados grupo, CancellationToken ct);

    Task<GrupoConvidados?> ObterGrupoAsync(string contaId, Guid id, CancellationToken ct);

    Task<GrupoConvidados> GarantirGrupoAsync(string contaId, string nome, CancellationToken ct);

    Task<IReadOnlyList<Convite>> ListarConvitesAsync(string contaId, Guid eventoId, CancellationToken ct);

    Task<Convite?> ObterConviteAsync(string contaId, Guid id, CancellationToken ct);

    Task<Convite?> ObterConvitePorTokenAsync(string token, CancellationToken ct);

    Task<bool> EventoEmAndamentoAsync(Guid eventoId, CancellationToken ct);

    Task InserirConvitesAsync(IReadOnlyList<Convite> convites, CancellationToken ct);

    Task SalvarConvitesAsync(CancellationToken ct);

    Task RemoverConvidadoAsync(Convidado convidado, CancellationToken ct);

    Task<Convidado?> ObterConvidadoAsync(string contaId, Guid id, CancellationToken ct);

    Task<IReadOnlyList<ContagemSituacao>> ContarEventosAsync(string contaId, CancellationToken ct);

    Task<IReadOnlyList<EventoAndamento>> ListarEmAndamentoAsync(string contaId, CancellationToken ct);

    Task<SituacaoEvento?> SituacaoDoEventoAsync(string contaId, Guid eventoId, CancellationToken ct);

    Task<IReadOnlyList<ContagemMes>> ContarEventosPorMesAsync(string contaId, int ano, CancellationToken ct);

    Task<IReadOnlyList<ParticipacaoNome>> ParticipacaoOrcamentoConcluidosAsync(string contaId, CancellationToken ct);

    Task<IReadOnlyList<ContagemNome>> ContarFornecedoresContratadosAsync(string contaId, CancellationToken ct);

    Task<IReadOnlyList<ContagemNome>> ContarServicosUtilizadosAsync(string contaId, CancellationToken ct);

    Task<int> ContarTarefasNaSemanaAsync(string contaId, DateOnly inicio, DateOnly fim, CancellationToken ct);

    Task<IReadOnlyList<ModeloBriefing>> ListarBriefingsAsync(string contaId, string? nome, CancellationToken ct);

    Task<ModeloBriefing?> ObterBriefingAsync(string contaId, Guid id, CancellationToken ct);

    Task InserirBriefingAsync(ModeloBriefing modelo, CancellationToken ct);

    Task SalvarBriefingAsync(CancellationToken ct);

    Task RemoverBriefingAsync(ModeloBriefing modelo, CancellationToken ct);

    Task<IReadOnlyList<ModeloRoteiro>> ListarRoteirosAsync(string contaId, string? nome, CancellationToken ct);

    Task<ModeloRoteiro?> ObterRoteiroAsync(string contaId, Guid id, CancellationToken ct);

    Task InserirRoteiroAsync(ModeloRoteiro modelo, CancellationToken ct);

    Task SalvarRoteiroAsync(CancellationToken ct);

    Task RemoverRoteiroAsync(ModeloRoteiro modelo, CancellationToken ct);

    Task<IReadOnlyList<ModeloCronograma>> ListarCronogramasAsync(string contaId, string? nome, CancellationToken ct);

    Task<ModeloCronograma?> ObterCronogramaAsync(string contaId, Guid id, CancellationToken ct);

    Task InserirCronogramaAsync(ModeloCronograma modelo, CancellationToken ct);

    Task SalvarCronogramaAsync(CancellationToken ct);

    Task RemoverCronogramaAsync(ModeloCronograma modelo, CancellationToken ct);

    Task<IReadOnlyList<ModeloInspiracao>> ListarInspiracoesAsync(string contaId, string? nome, CancellationToken ct);

    Task<ModeloInspiracao?> ObterInspiracaoAsync(string contaId, Guid id, CancellationToken ct);

    Task InserirInspiracaoAsync(ModeloInspiracao modelo, CancellationToken ct);

    Task SalvarInspiracaoAsync(CancellationToken ct);

    Task RemoverInspiracaoAsync(ModeloInspiracao modelo, CancellationToken ct);

    Task<IReadOnlyList<ModeloMensagem>> ListarMensagensAsync(string contaId, string? nome, CancellationToken ct);

    Task<ModeloMensagem?> ObterMensagemAsync(string contaId, Guid id, CancellationToken ct);

    Task InserirMensagemAsync(ModeloMensagem modelo, CancellationToken ct);

    Task SalvarMensagemAsync(CancellationToken ct);

    Task RemoverMensagemAsync(ModeloMensagem modelo, CancellationToken ct);

    Task<IReadOnlyList<ServicoConta>> ListarServicosContaAsync(string contaId, string? nome, CancellationToken ct);

    Task<ServicoConta?> ObterServicoContaAsync(string contaId, Guid id, CancellationToken ct);

    Task<ServicoConta?> ObterServicoContaPorNomeAsync(string contaId, string nome, CancellationToken ct);

    Task InserirServicoContaAsync(ServicoConta servico, CancellationToken ct);

    Task RemoverServicoContaAsync(ServicoConta servico, CancellationToken ct);

    Task<IReadOnlyList<Fornecedor>> ListarFornecedoresAsync(string contaId, string? nome, CancellationToken ct);

    Task<Fornecedor?> ObterFornecedorAsync(string contaId, Guid id, CancellationToken ct);

    Task InserirFornecedorAsync(Fornecedor fornecedor, CancellationToken ct);

    Task SalvarFornecedorAsync(CancellationToken ct);

    Task RemoverFornecedorAsync(Fornecedor fornecedor, CancellationToken ct);

    Task<IReadOnlyDictionary<Guid, string>> NomesServicosContaAsync(string contaId, IEnumerable<Guid> ids, CancellationToken ct);

    Task<IReadOnlyDictionary<Guid, string>> NomesFornecedoresAsync(string contaId, IEnumerable<Guid> ids, CancellationToken ct);

    Task SalvarNegocioAsync(CancellationToken ct);

    Task<IReadOnlyList<CompromissoAgenda>> ListarCompromissosAsync(
        string contaId,
        DateOnly? de,
        DateOnly? ate,
        CancellationToken ct);

    Task<CompromissoAgenda?> ObterCompromissoAsync(string contaId, Guid id, CancellationToken ct);

    Task InserirCompromissoAsync(CompromissoAgenda compromisso, CancellationToken ct);

    Task RemoverCompromissoAsync(CompromissoAgenda compromisso, CancellationToken ct);

    Task<bool> TemCompromissoNaDataAsync(string contaId, DateOnly data, CancellationToken ct);

    Task<IReadOnlyList<Oportunidade>> ListarOportunidadesAsync(
        string contaId,
        FiltroOportunidades filtro,
        CancellationToken ct);

    Task<Oportunidade?> ObterOportunidadeAsync(string contaId, Guid id, CancellationToken ct);

    Task InserirOportunidadeAsync(Oportunidade oportunidade, CancellationToken ct);

    Task RemoverOportunidadeAsync(Oportunidade oportunidade, CancellationToken ct);

    Task<IReadOnlyList<ContagemOportunidadeEstagio>> ContarOportunidadesPorEstagioAsync(
        string contaId,
        CancellationToken ct);

    Task<IReadOnlyDictionary<Guid, string>> NomesOportunidadesAsync(
        string contaId,
        IEnumerable<Guid> ids,
        CancellationToken ct);

    Task<IReadOnlyList<Oportunidade>> ListarOportunidadesPorCriacaoAsync(
        string contaId,
        DateTimeOffset inicio,
        DateTimeOffset fim,
        CancellationToken ct);

    Task<IReadOnlyList<Oportunidade>> ListarOportunidadesPorEntradaEstagioAsync(
        string contaId,
        DateTimeOffset inicio,
        DateTimeOffset fim,
        CancellationToken ct);

    Task<IReadOnlyList<LeadRsvp>> ListarLeadsRsvpAtivosAsync(string contaId, string? busca, CancellationToken ct);

    Task<LeadRsvp?> ObterLeadRsvpAsync(string contaId, Guid id, CancellationToken ct);

    Task InserirLeadRsvpAsync(LeadRsvp lead, CancellationToken ct);

    Task<IReadOnlyList<LeadRsvp>> ListarLeadsRsvpPeriodoAsync(
        string contaId,
        DateTimeOffset inicio,
        DateTimeOffset fim,
        CancellationToken ct);

    Task<int> ContarLeadsRsvpConvertidosAntesAsync(string contaId, DateTimeOffset limite, CancellationToken ct);

    Task<MetricasRsvpFunil?> ObterMetricasRsvpFunilAsync(string contaId, CancellationToken ct);

    Task IncrementarVisualizacaoRsvpAsync(string contaId, CancellationToken ct);

    Task IncrementarCliqueRsvpAsync(string contaId, CancellationToken ct);

    Task<IReadOnlyList<ContratoFunil>> ListarContratosFunilAsync(
        string contaId,
        SituacaoContratoFunil? situacao,
        string? busca,
        DateOnly? criadoDe,
        DateOnly? criadoAte,
        CancellationToken ct);

    Task<ContratoFunil?> ObterContratoFunilAsync(string contaId, Guid id, CancellationToken ct);

    Task InserirContratoFunilAsync(ContratoFunil contrato, CancellationToken ct);

    Task RemoverContratoFunilAsync(ContratoFunil contrato, CancellationToken ct);

    Task<IReadOnlyList<OrcamentoFunil>> ListarOrcamentosFunilAsync(
        string contaId,
        SituacaoOrcamentoFunil? situacao,
        string? busca,
        DateOnly? criadoDe,
        DateOnly? criadoAte,
        CancellationToken ct);

    Task<OrcamentoFunil?> ObterOrcamentoFunilAsync(string contaId, Guid id, CancellationToken ct);

    Task InserirOrcamentoFunilAsync(OrcamentoFunil orcamento, CancellationToken ct);

    Task RemoverOrcamentoFunilAsync(OrcamentoFunil orcamento, CancellationToken ct);

    Task<IReadOnlyList<TarefaFunil>> ListarTarefasFunilAsync(
        string contaId,
        DateOnly? prazoDe,
        DateOnly? prazoAte,
        TipoTarefaFunil? tipo,
        string? responsavel,
        bool? completa,
        CancellationToken ct);

    Task<TarefaFunil?> ObterTarefaFunilAsync(string contaId, Guid id, CancellationToken ct);

    Task InserirTarefaFunilAsync(TarefaFunil tarefa, CancellationToken ct);

    Task RemoverTarefaFunilAsync(TarefaFunil tarefa, CancellationToken ct);

    Task<IReadOnlyList<ModeloServicoFunil>> ListarModelosServicoFunilAsync(string contaId, string? nome, CancellationToken ct);

    Task<ModeloServicoFunil?> ObterModeloServicoFunilAsync(string contaId, Guid id, CancellationToken ct);

    Task InserirModeloServicoFunilAsync(ModeloServicoFunil modelo, CancellationToken ct);

    Task RemoverModeloServicoFunilAsync(ModeloServicoFunil modelo, CancellationToken ct);

    Task<IReadOnlyList<ModeloOrcamentoFunil>> ListarModelosOrcamentoFunilAsync(string contaId, string? nome, CancellationToken ct);

    Task<ModeloOrcamentoFunil?> ObterModeloOrcamentoFunilAsync(string contaId, Guid id, CancellationToken ct);

    Task InserirModeloOrcamentoFunilAsync(ModeloOrcamentoFunil modelo, CancellationToken ct);

    Task RemoverModeloOrcamentoFunilAsync(ModeloOrcamentoFunil modelo, CancellationToken ct);

    Task<IReadOnlyList<ModeloContratoFunil>> ListarModelosContratoFunilAsync(string contaId, string? nome, CancellationToken ct);

    Task<ModeloContratoFunil?> ObterModeloContratoFunilAsync(string contaId, Guid id, CancellationToken ct);

    Task InserirModeloContratoFunilAsync(ModeloContratoFunil modelo, CancellationToken ct);

    Task RemoverModeloContratoFunilAsync(ModeloContratoFunil modelo, CancellationToken ct);

    Task<(string ContaId, string Nome)?> ResumoEventoPorIdAsync(Guid eventoId, CancellationToken ct);
}
