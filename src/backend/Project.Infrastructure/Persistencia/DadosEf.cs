using Microsoft.EntityFrameworkCore;
using Project.Application.Negocio;
using Project.Domain.Clientes;
using Project.Domain.Convidados;
using Project.Domain.Eventos;
using Project.Domain.Perfil;
using Project.Domain.Tarefas;

namespace Project.Infrastructure.Persistencia;

public sealed class DadosEf(AppDbContext banco) : IDados
{
    public Task<PerfilAssessoria?> ObterPerfilAsync(string contaId, CancellationToken ct) =>
        banco.Perfis.FirstOrDefaultAsync(perfil => perfil.ContaId == contaId, ct);

    public async Task SalvarPerfilAsync(PerfilAssessoria perfil, CancellationToken ct)
    {
        if (banco.Entry(perfil).State == EntityState.Detached)
        {
            banco.Perfis.Add(perfil);
        }

        await banco.SaveChangesAsync(ct);
    }

    public Task<IReadOnlyList<ModeloEvento>> ListarModelosAsync(string contaId, CancellationToken ct) =>
        Lista(banco.Modelos.AsNoTracking().Include(modelo => modelo.Servicos).Include(modelo => modelo.Tarefas)
            .Where(modelo => modelo.ContaId == contaId)
            .OrderBy(modelo => modelo.Nome), ct);

    public Task<ModeloEvento?> ObterModeloAsync(string contaId, Guid id, CancellationToken ct) =>
        banco.Modelos.Include(modelo => modelo.Servicos).Include(modelo => modelo.Tarefas)
            .FirstOrDefaultAsync(modelo => modelo.Id == id && modelo.ContaId == contaId, ct);

    public async Task InserirModeloAsync(ModeloEvento modelo, CancellationToken ct)
    {
        banco.Modelos.Add(modelo);
        await banco.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<EventoLinha>> ListarEventosAsync(string contaId, FiltroEventos filtro, CancellationToken ct)
    {
        var consulta = banco.Eventos.AsNoTracking().Where(evento => evento.ContaId == contaId);
        if (filtro.Situacao is SituacaoEvento situacao)
        {
            consulta = consulta.Where(evento => evento.Situacao == situacao);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            var nome = filtro.Nome.Trim().ToLower();
            consulta = consulta.Where(evento => evento.Nome.ToLower().Contains(nome));
        }

        if (filtro.Tipo is TipoEvento tipo)
        {
            consulta = consulta.Where(evento => evento.Tipo == tipo);
        }

        if (filtro.De is not null || filtro.Ate is not null)
        {
            consulta = consulta.Where(evento => evento.Data != null);
            if (filtro.De is DateOnly de)
            {
                consulta = consulta.Where(evento => evento.Data >= de);
            }

            if (filtro.Ate is DateOnly ate)
            {
                consulta = consulta.Where(evento => evento.Data <= ate);
            }
        }

        if (!string.IsNullOrWhiteSpace(filtro.Tag))
        {
            var tag = filtro.Tag.Trim().ToLower();
            consulta = consulta.Where(evento => evento.Tags.Any(item => item.Nome.ToLower() == tag));
        }

        var linhas = await consulta
            .OrderBy(evento => evento.Nome)
            .Select(evento => new EventoLinha(
                evento.Id,
                evento.Nome,
                evento.Situacao,
                evento.Tipo,
                evento.Data,
                evento.Tags.Select(tag => tag.Nome).ToList(),
                banco.Tarefas.Count(tarefa => tarefa.EventoId == evento.Id && tarefa.Situacao == SituacaoTarefa.Concluida),
                banco.Tarefas.Count(tarefa => tarefa.EventoId == evento.Id),
                banco.Convidados.Count(convidado =>
                    banco.Convites.Any(convite => convite.Id == convidado.ConviteId && convite.EventoId == evento.Id)
                    && convidado.Situacao == SituacaoConvidado.Confirmado),
                banco.Convidados.Count(convidado =>
                    banco.Convites.Any(convite => convite.Id == convidado.ConviteId && convite.EventoId == evento.Id))))
            .ToListAsync(ct);
        return linhas;
    }

    public Task<Evento?> ObterEventoAsync(string contaId, Guid id, CancellationToken ct) =>
        banco.Eventos
            .Include(evento => evento.Tags)
            .Include(evento => evento.Servicos).ThenInclude(servico => servico.Parcelas)
            .Include(evento => evento.LocaisTransfer)
            .FirstOrDefaultAsync(evento => evento.Id == id && evento.ContaId == contaId, ct);

    public Task<bool> EventoDaContaAsync(string contaId, Guid id, CancellationToken ct) =>
        banco.Eventos.AnyAsync(evento => evento.Id == id && evento.ContaId == contaId, ct);

    public async Task InserirEventoAsync(
        Evento evento,
        IReadOnlyList<Tarefa> tarefas,
        CompromissoAgenda? agenda,
        Cliente? clienteNovo,
        CancellationToken ct)
    {
        banco.Eventos.Add(evento);
        banco.Tarefas.AddRange(tarefas);
        if (agenda is not null)
        {
            banco.Compromissos.Add(agenda);
        }

        if (clienteNovo is not null)
        {
            banco.Clientes.Add(clienteNovo);
        }

        await banco.SaveChangesAsync(ct);
    }

    public Task SalvarEventoAsync(CancellationToken ct) => banco.SaveChangesAsync(ct);

    public async Task SubstituirTagsAsync(Evento evento, IReadOnlyList<string> nomes, CancellationToken ct)
    {
        evento.Tags.Clear();
        foreach (var nome in nomes)
        {
            var alvo = nome.ToLower();
            var tag = await banco.Tags.FirstOrDefaultAsync(item => item.ContaId == evento.ContaId && item.Nome.ToLower() == alvo, ct);
            if (tag is null)
            {
                tag = new TagConta { Id = Guid.NewGuid(), ContaId = evento.ContaId, Nome = nome };
                banco.Tags.Add(tag);
            }

            evento.Tags.Add(tag);
        }

        await banco.SaveChangesAsync(ct);
    }

    public async Task SincronizarAgendaAsync(Evento evento, CancellationToken ct)
    {
        var compromisso = await banco.Compromissos.FirstOrDefaultAsync(item => item.EventoId == evento.Id, ct);
        if (!evento.SalvarNaAgenda)
        {
            if (compromisso is not null)
            {
                banco.Compromissos.Remove(compromisso);
            }
        }
        else if (compromisso is null)
        {
            banco.Compromissos.Add(new CompromissoAgenda
            {
                Id = Guid.NewGuid(),
                ContaId = evento.ContaId,
                EventoId = evento.Id,
                Data = evento.Data,
                Hora = evento.Hora,
            });
        }
        else
        {
            compromisso.Data = evento.Data;
            compromisso.Hora = evento.Hora;
        }

        await banco.SaveChangesAsync(ct);
    }

    public Task<bool> TemAssessoriaAsync(Guid eventoId, CancellationToken ct) =>
        banco.Set<ServicoEvento>().AnyAsync(
            servico => servico.EventoId == eventoId && servico.Nome == ParcelasDaAssessoria.NomeServico && servico.Contratado,
            ct);

    public async Task AdicionarServicoAsync(ServicoEvento servico, CancellationToken ct)
    {
        banco.Set<ServicoEvento>().Add(servico);
        await banco.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Tarefa>> ListarTarefasAsync(string contaId, Guid? eventoId, CancellationToken ct)
    {
        var consulta = banco.Tarefas.AsNoTracking().Where(tarefa => tarefa.ContaId == contaId);
        if (eventoId is Guid id)
        {
            consulta = consulta.Where(tarefa => tarefa.EventoId == id);
        }

        return await consulta.OrderBy(tarefa => tarefa.Prazo).ThenBy(tarefa => tarefa.Titulo).ToListAsync(ct);
    }

    public Task<Tarefa?> ObterTarefaAsync(string contaId, Guid id, CancellationToken ct) =>
        banco.Tarefas.FirstOrDefaultAsync(tarefa => tarefa.Id == id && tarefa.ContaId == contaId, ct);

    public async Task InserirTarefaAsync(Tarefa tarefa, CancellationToken ct)
    {
        banco.Tarefas.Add(tarefa);
        await banco.SaveChangesAsync(ct);
    }

    public Task SalvarTarefaAsync(CancellationToken ct) => banco.SaveChangesAsync(ct);

    public async Task<string?> NomeDoEventoAsync(string contaId, Guid eventoId, CancellationToken ct) =>
        await banco.Eventos.AsNoTracking()
            .Where(evento => evento.Id == eventoId && evento.ContaId == contaId)
            .Select(evento => evento.Nome)
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<Cliente>> ListarClientesAsync(string contaId, string? nome, CancellationToken ct)
    {
        var consulta = banco.Clientes.AsNoTracking().Where(cliente => cliente.ContaId == contaId);
        if (!string.IsNullOrWhiteSpace(nome))
        {
            var termo = nome.Trim().ToLower();
            consulta = consulta.Where(cliente => cliente.Nome.ToLower().Contains(termo));
        }

        return await consulta.OrderBy(cliente => cliente.Nome).ToListAsync(ct);
    }

    public Task<Cliente?> ObterClienteAsync(string contaId, Guid id, CancellationToken ct) =>
        banco.Clientes.FirstOrDefaultAsync(cliente => cliente.Id == id && cliente.ContaId == contaId, ct);

    public async Task InserirClienteAsync(Cliente cliente, CancellationToken ct)
    {
        banco.Clientes.Add(cliente);
        await banco.SaveChangesAsync(ct);
    }

    public Task SalvarClienteAsync(CancellationToken ct) => banco.SaveChangesAsync(ct);

    public async Task<IReadOnlyList<(Guid Id, string Nome)>> EventosDoClienteAsync(string contaId, Guid clienteId, CancellationToken ct)
    {
        var eventos = await banco.Vinculos.AsNoTracking()
            .Where(vinculo => vinculo.ClienteId == clienteId)
            .Join(
                banco.Eventos.AsNoTracking().Where(evento => evento.ContaId == contaId),
                vinculo => vinculo.EventoId,
                evento => evento.Id,
                (_, evento) => new { evento.Id, evento.Nome })
            .OrderBy(evento => evento.Nome)
            .ToListAsync(ct);
        return eventos.Select(evento => (evento.Id, evento.Nome)).ToArray();
    }

    public async Task<IReadOnlyList<Cliente>> ClientesDoEventoAsync(string contaId, Guid eventoId, CancellationToken ct) =>
        await banco.Vinculos.AsNoTracking()
            .Where(vinculo => vinculo.EventoId == eventoId)
            .Join(
                banco.Clientes.AsNoTracking().Where(cliente => cliente.ContaId == contaId),
                vinculo => vinculo.ClienteId,
                cliente => cliente.Id,
                (_, cliente) => cliente)
            .OrderBy(cliente => cliente.Nome)
            .ToListAsync(ct);

    public async Task<bool> VincularAsync(string contaId, Guid clienteId, Guid eventoId, CancellationToken ct)
    {
        var clienteOk = await banco.Clientes.AnyAsync(cliente => cliente.Id == clienteId && cliente.ContaId == contaId, ct);
        var eventoOk = await banco.Eventos.AnyAsync(evento => evento.Id == eventoId && evento.ContaId == contaId, ct);
        if (!clienteOk || !eventoOk)
        {
            return false;
        }

        var existe = await banco.Vinculos.AnyAsync(vinculo => vinculo.ClienteId == clienteId && vinculo.EventoId == eventoId, ct);
        if (!existe)
        {
            banco.Vinculos.Add(new VinculoCliente { ClienteId = clienteId, EventoId = eventoId });
            await banco.SaveChangesAsync(ct);
        }

        return true;
    }

    public async Task<bool> DesvincularAsync(string contaId, Guid clienteId, Guid eventoId, CancellationToken ct)
    {
        var clienteOk = await banco.Clientes.AnyAsync(cliente => cliente.Id == clienteId && cliente.ContaId == contaId, ct);
        var eventoOk = await banco.Eventos.AnyAsync(evento => evento.Id == eventoId && evento.ContaId == contaId, ct);
        if (!clienteOk || !eventoOk)
        {
            return false;
        }

        var vinculo = await banco.Vinculos.FirstOrDefaultAsync(item => item.ClienteId == clienteId && item.EventoId == eventoId, ct);
        if (vinculo is not null)
        {
            banco.Vinculos.Remove(vinculo);
            await banco.SaveChangesAsync(ct);
        }

        return true;
    }

    public async Task<IReadOnlyList<GrupoConvidados>> ListarGruposAsync(string contaId, CancellationToken ct) =>
        await banco.Grupos.AsNoTracking().Where(grupo => grupo.ContaId == contaId).OrderBy(grupo => grupo.Nome).ToListAsync(ct);

    public async Task InserirGrupoAsync(GrupoConvidados grupo, CancellationToken ct)
    {
        banco.Grupos.Add(grupo);
        await banco.SaveChangesAsync(ct);
    }

    public Task<GrupoConvidados?> ObterGrupoAsync(string contaId, Guid id, CancellationToken ct) =>
        banco.Grupos.FirstOrDefaultAsync(grupo => grupo.Id == id && grupo.ContaId == contaId, ct);

    public async Task<GrupoConvidados> GarantirGrupoAsync(string contaId, string nome, CancellationToken ct)
    {
        var alvo = nome.ToLower();
        var grupo = await banco.Grupos.FirstOrDefaultAsync(item => item.ContaId == contaId && item.Nome.ToLower() == alvo, ct);
        if (grupo is not null)
        {
            return grupo;
        }

        grupo = new GrupoConvidados { Id = Guid.NewGuid(), ContaId = contaId, Nome = nome };
        banco.Grupos.Add(grupo);
        await banco.SaveChangesAsync(ct);
        return grupo;
    }

    public async Task<IReadOnlyList<Convite>> ListarConvitesAsync(string contaId, Guid eventoId, CancellationToken ct) =>
        await banco.Convites.AsNoTracking().Include(convite => convite.Convidados)
            .Where(convite => convite.ContaId == contaId && convite.EventoId == eventoId)
            .OrderBy(convite => convite.Nome)
            .ToListAsync(ct);

    public Task<Convite?> ObterConviteAsync(string contaId, Guid id, CancellationToken ct) =>
        banco.Convites.Include(convite => convite.Convidados)
            .FirstOrDefaultAsync(convite => convite.Id == id && convite.ContaId == contaId, ct);

    public Task<Convite?> ObterConvitePorTokenAsync(string token, CancellationToken ct) =>
        banco.Convites.Include(convite => convite.Convidados)
            .FirstOrDefaultAsync(convite => convite.Token == token, ct);

    public Task<bool> EventoEmAndamentoAsync(Guid eventoId, CancellationToken ct) =>
        banco.Eventos.AnyAsync(evento => evento.Id == eventoId && evento.Situacao == SituacaoEvento.EmAndamento, ct);

    public async Task InserirConvitesAsync(IReadOnlyList<Convite> convites, CancellationToken ct)
    {
        banco.Convites.AddRange(convites);
        await banco.SaveChangesAsync(ct);
    }

    public Task SalvarConvitesAsync(CancellationToken ct) => banco.SaveChangesAsync(ct);

    public async Task RemoverConvidadoAsync(Convidado convidado, CancellationToken ct)
    {
        banco.Convidados.Remove(convidado);
        await banco.SaveChangesAsync(ct);
    }

    public Task<Convidado?> ObterConvidadoAsync(string contaId, Guid id, CancellationToken ct) =>
        banco.Convidados.FirstOrDefaultAsync(
            convidado => convidado.Id == id && banco.Convites.Any(convite => convite.Id == convidado.ConviteId && convite.ContaId == contaId),
            ct);

    public async Task<IReadOnlyList<ContagemSituacao>> ContarEventosAsync(string contaId, CancellationToken ct) =>
        await banco.Eventos.AsNoTracking()
            .Where(evento => evento.ContaId == contaId)
            .GroupBy(evento => evento.Situacao)
            .Select(grupo => new ContagemSituacao(grupo.Key, grupo.Count()))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<EventoAndamento>> ListarEmAndamentoAsync(string contaId, CancellationToken ct) =>
        await banco.Eventos.AsNoTracking()
            .Where(evento => evento.ContaId == contaId && evento.Situacao == SituacaoEvento.EmAndamento)
            .OrderBy(evento => evento.Nome)
            .Select(evento => new EventoAndamento(evento.Id, evento.Nome, evento.Data))
            .ToListAsync(ct);

    public async Task<SituacaoEvento?> SituacaoDoEventoAsync(string contaId, Guid eventoId, CancellationToken ct)
    {
        var situacao = await banco.Eventos.AsNoTracking()
            .Where(evento => evento.Id == eventoId && evento.ContaId == contaId)
            .Select(evento => (SituacaoEvento?)evento.Situacao)
            .FirstOrDefaultAsync(ct);
        return situacao;
    }

    private static async Task<IReadOnlyList<T>> Lista<T>(IQueryable<T> consulta, CancellationToken ct) =>
        await consulta.ToListAsync(ct);
}
