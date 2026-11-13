using Project.Domain.Clientes;
using Project.Domain.Convidados;
using Project.Domain.Eventos;
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

public interface IDados
{
    Task<PerfilAssessoria?> ObterPerfilAsync(string contaId, CancellationToken ct);

    Task SalvarPerfilAsync(PerfilAssessoria perfil, CancellationToken ct);

    Task<IReadOnlyList<ModeloEvento>> ListarModelosAsync(string contaId, CancellationToken ct);

    Task<ModeloEvento?> ObterModeloAsync(string contaId, Guid id, CancellationToken ct);

    Task InserirModeloAsync(ModeloEvento modelo, CancellationToken ct);

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

    Task<IReadOnlyList<Tarefa>> ListarTarefasAsync(string contaId, Guid? eventoId, CancellationToken ct);

    Task<Tarefa?> ObterTarefaAsync(string contaId, Guid id, CancellationToken ct);

    Task InserirTarefaAsync(Tarefa tarefa, CancellationToken ct);

    Task SalvarTarefaAsync(CancellationToken ct);

    Task<string?> NomeDoEventoAsync(string contaId, Guid eventoId, CancellationToken ct);

    Task<IReadOnlyList<Cliente>> ListarClientesAsync(string contaId, string? nome, CancellationToken ct);

    Task<Cliente?> ObterClienteAsync(string contaId, Guid id, CancellationToken ct);

    Task InserirClienteAsync(Cliente cliente, CancellationToken ct);

    Task SalvarClienteAsync(CancellationToken ct);

    Task<IReadOnlyList<(Guid Id, string Nome)>> EventosDoClienteAsync(string contaId, Guid clienteId, CancellationToken ct);

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
}
