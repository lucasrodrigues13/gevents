using Microsoft.EntityFrameworkCore;
using Project.Application.Negocio;
using Project.Domain.Clientes;
using Project.Domain.Convidados;
using Project.Domain.Eventos;
using Project.Domain.Fornecedores;
using Project.Domain.Funil;
using Project.Domain.Modelos;
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

    public async Task<IReadOnlyList<ModeloEvento>> ListarModelosAsync(string contaId, string? nome, CancellationToken ct)
    {
        var consulta = ModelosCompletos().AsNoTracking().Where(modelo => modelo.ContaId == contaId);
        if (!string.IsNullOrWhiteSpace(nome))
        {
            var termo = nome.Trim().ToLower();
            consulta = consulta.Where(modelo => modelo.Nome.ToLower().Contains(termo));
        }

        return await consulta.OrderBy(modelo => modelo.Nome).ToListAsync(ct);
    }

    public Task<ModeloEvento?> ObterModeloAsync(string contaId, Guid id, CancellationToken ct) =>
        ModelosCompletos().FirstOrDefaultAsync(modelo => modelo.Id == id && modelo.ContaId == contaId, ct);

    public async Task InserirModeloAsync(ModeloEvento modelo, CancellationToken ct)
    {
        banco.Modelos.Add(modelo);
        await banco.SaveChangesAsync(ct);
    }

    public Task AtualizarModeloAsync(CancellationToken ct) => banco.SaveChangesAsync(ct);

    public async Task RemoverModeloAsync(ModeloEvento modelo, CancellationToken ct)
    {
        banco.Modelos.Remove(modelo);
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
            .Include(evento => evento.Servicos).ThenInclude(servico => servico.Subitens)
            .Include(evento => evento.Servicos).ThenInclude(servico => servico.Fornecedores)
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
                Descricao = evento.Nome,
                Data = evento.Data,
                Hora = evento.Hora,
            });
        }
        else
        {
            compromisso.Descricao = evento.Nome;
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

    public Task<ServicoEvento?> ObterServicoEventoAsync(string contaId, Guid eventoId, Guid servicoId, CancellationToken ct) =>
        banco.Set<ServicoEvento>().AsNoTracking()
            .Include(servico => servico.Parcelas)
            .Include(servico => servico.Subitens)
            .Include(servico => servico.Fornecedores)
            .FirstOrDefaultAsync(
                servico => servico.Id == servicoId
                    && servico.EventoId == eventoId
                    && banco.Eventos.Any(evento => evento.Id == eventoId && evento.ContaId == contaId),
                ct);

    public async Task RemoverConteudoModeloAsync(ModeloEvento modelo, CancellationToken ct)
    {
        foreach (var servico in modelo.Servicos)
        {
            foreach (var item in servico.Itens.ToList())
            {
                banco.Entry(item).State = EntityState.Detached;
            }

            servico.Itens.Clear();
        }

        var modeloId = modelo.Id;
        banco.Entry(modelo).State = EntityState.Detached;
        foreach (var servico in modelo.Servicos.ToList())
        {
            banco.Entry(servico).State = EntityState.Detached;
        }

        foreach (var tarefa in modelo.Tarefas.ToList())
        {
            banco.Entry(tarefa).State = EntityState.Detached;
        }

        foreach (var vinculo in modelo.Vinculos.ToList())
        {
            banco.Entry(vinculo).State = EntityState.Detached;
        }

        modelo.Servicos.Clear();
        modelo.Tarefas.Clear();
        modelo.Vinculos.Clear();

        await banco.Set<ItemServicoModelo>()
            .Where(item => banco.Set<ServicoModelo>().Any(servico => servico.Id == item.ServicoId && servico.ModeloId == modeloId))
            .ExecuteDeleteAsync(ct);
        await banco.Set<ServicoModelo>().Where(servico => servico.ModeloId == modeloId).ExecuteDeleteAsync(ct);
        await banco.Set<TarefaModelo>().Where(tarefa => tarefa.ModeloId == modeloId).ExecuteDeleteAsync(ct);
        await banco.Set<VinculoModeloEvento>().Where(vinculo => vinculo.ModeloId == modeloId).ExecuteDeleteAsync(ct);
    }

    public async Task GravarModeloAtualizadoAsync(ModeloEvento modelo, CancellationToken ct)
    {
        await banco.Modelos
            .Where(item => item.Id == modelo.Id)
            .ExecuteUpdateAsync(
                atualizacao => atualizacao
                    .SetProperty(item => item.Nome, modelo.Nome)
                    .SetProperty(item => item.Tipo, modelo.Tipo)
                    .SetProperty(item => item.PermiteGerais, modelo.PermiteGerais)
                    .SetProperty(item => item.PermitePlanejamento, modelo.PermitePlanejamento)
                    .SetProperty(item => item.PermiteTarefas, modelo.PermiteTarefas)
                    .SetProperty(item => item.PermiteServicosFornecedores, modelo.PermiteServicosFornecedores)
                    .SetProperty(item => item.PermiteConvites, modelo.PermiteConvites),
                ct);

        if (modelo.Servicos.Count > 0)
        {
            banco.Set<ServicoModelo>().AddRange(modelo.Servicos);
        }

        if (modelo.Tarefas.Count > 0)
        {
            banco.Set<TarefaModelo>().AddRange(modelo.Tarefas);
        }

        if (modelo.Vinculos.Count > 0)
        {
            banco.Set<VinculoModeloEvento>().AddRange(modelo.Vinculos);
        }

        await banco.SaveChangesAsync(ct);
    }

    public async Task InserirFornecedorNoServicoAsync(FornecedorNoServico vinculo, CancellationToken ct)
    {
        banco.Set<FornecedorNoServico>().Add(vinculo);
        await banco.SaveChangesAsync(ct);
    }

    public async Task SubstituirSubitensAsync(
        Guid servicoId,
        IReadOnlyList<SubitemServico> subitens,
        decimal? valorEstimado,
        CancellationToken ct)
    {
        await banco.Set<SubitemServico>().Where(item => item.ServicoId == servicoId).ExecuteDeleteAsync(ct);
        if (subitens.Count > 0)
        {
            banco.Set<SubitemServico>().AddRange(subitens);
        }

        await banco.Set<ServicoEvento>()
            .Where(servico => servico.Id == servicoId)
            .ExecuteUpdateAsync(atualizacao => atualizacao.SetProperty(servico => servico.ValorEstimado, valorEstimado), ct);
        if (subitens.Count > 0)
        {
            await banco.SaveChangesAsync(ct);
        }
    }

    public async Task SubstituirParcelasAsync(Guid servicoId, IReadOnlyList<ParcelaServico> parcelas, CancellationToken ct)
    {
        await banco.Set<ParcelaServico>().Where(parcela => parcela.ServicoId == servicoId).ExecuteDeleteAsync(ct);
        banco.Set<ParcelaServico>().AddRange(parcelas);
        await banco.SaveChangesAsync(ct);
    }

    public Task AtualizarEtapaNegociacaoAsync(
        Guid vinculoId,
        EtapaNegociacao etapa,
        decimal? valorOrcamento,
        CancellationToken ct) =>
        banco.Set<FornecedorNoServico>()
            .Where(vinculo => vinculo.Id == vinculoId)
            .ExecuteUpdateAsync(
                atualizacao => atualizacao
                    .SetProperty(vinculo => vinculo.Etapa, etapa)
                    .SetProperty(vinculo => vinculo.ValorOrcamento, valorOrcamento),
                ct);

    public Task MarcarServicoContratadoAsync(Guid servicoId, bool contratado, decimal? valorContratado, CancellationToken ct) =>
        banco.Set<ServicoEvento>()
            .Where(servico => servico.Id == servicoId)
            .ExecuteUpdateAsync(
                atualizacao => atualizacao
                    .SetProperty(servico => servico.Contratado, contratado)
                    .SetProperty(servico => servico.ValorContratado, valorContratado),
                ct);

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

    public Task<Cliente?> ObterClientePorUsuarioAsync(string contaId, string usuarioId, CancellationToken ct) =>
        banco.Clientes.FirstOrDefaultAsync(
            cliente => cliente.ContaId == contaId
                && cliente.UsuarioId == usuarioId
                && cliente.AcessoPlataforma,
            ct);

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

    public async Task<IReadOnlyList<Guid>> EventosDoUsuarioClienteAsync(string contaId, string usuarioId, CancellationToken ct)
    {
        var cliente = await banco.Clientes.AsNoTracking()
            .FirstOrDefaultAsync(
                item => item.ContaId == contaId && item.UsuarioId == usuarioId && item.AcessoPlataforma,
                ct);
        if (cliente is null)
        {
            return [];
        }

        return await banco.Vinculos.AsNoTracking()
            .Where(vinculo => vinculo.ClienteId == cliente.Id)
            .Join(
                banco.Eventos.AsNoTracking().Where(evento => evento.ContaId == contaId),
                vinculo => vinculo.EventoId,
                evento => evento.Id,
                (_, evento) => evento.Id)
            .ToListAsync(ct);
    }

    public async Task<bool> ClienteVinculadoAoEventoAsync(string contaId, string usuarioId, Guid eventoId, CancellationToken ct)
    {
        var cliente = await banco.Clientes.AsNoTracking()
            .FirstOrDefaultAsync(
                item => item.ContaId == contaId && item.UsuarioId == usuarioId && item.AcessoPlataforma,
                ct);
        if (cliente is null)
        {
            return false;
        }

        return await banco.Vinculos.AsNoTracking()
            .AnyAsync(vinculo => vinculo.ClienteId == cliente.Id && vinculo.EventoId == eventoId, ct);
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

    public async Task<IReadOnlyList<ContagemMes>> ContarEventosPorMesAsync(string contaId, int ano, CancellationToken ct) =>
        await banco.Eventos.AsNoTracking()
            .Where(evento => evento.ContaId == contaId && evento.Data != null && evento.Data.Value.Year == ano)
            .GroupBy(evento => evento.Data!.Value.Month)
            .Select(grupo => new ContagemMes(grupo.Key, grupo.Count()))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<ParticipacaoNome>> ParticipacaoOrcamentoConcluidosAsync(string contaId, CancellationToken ct) =>
        await banco.Set<ServicoEvento>().AsNoTracking()
            .Where(servico =>
                banco.Eventos.Any(evento =>
                    evento.Id == servico.EventoId
                    && evento.ContaId == contaId
                    && evento.Situacao == SituacaoEvento.Concluido)
                && servico.ValorContratado != null
                && servico.ValorContratado > 0)
            .GroupBy(servico => servico.Nome)
            .Select(grupo => new ParticipacaoNome(grupo.Key, grupo.Sum(servico => servico.ValorContratado!.Value)))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<ContagemNome>> ContarFornecedoresContratadosAsync(string contaId, CancellationToken ct)
    {
        var linhas = await (
            from vinculo in banco.Set<FornecedorNoServico>().AsNoTracking()
            join servico in banco.Set<ServicoEvento>().AsNoTracking() on vinculo.ServicoId equals servico.Id
            join evento in banco.Eventos.AsNoTracking() on servico.EventoId equals evento.Id
            join fornecedor in banco.Fornecedores.AsNoTracking() on vinculo.FornecedorId equals fornecedor.Id
            where evento.ContaId == contaId
                && fornecedor.ContaId == contaId
                && vinculo.Etapa == EtapaNegociacao.Contratado
            group vinculo by fornecedor.Nome into grupo
            select new ContagemNome(grupo.Key, grupo.Count())).ToListAsync(ct);
        return linhas;
    }

    public async Task<IReadOnlyList<ContagemNome>> ContarServicosUtilizadosAsync(string contaId, CancellationToken ct) =>
        await banco.Set<ServicoEvento>().AsNoTracking()
            .Where(servico => banco.Eventos.Any(evento => evento.Id == servico.EventoId && evento.ContaId == contaId))
            .GroupBy(servico => servico.Nome)
            .Select(grupo => new ContagemNome(grupo.Key, grupo.Count()))
            .ToListAsync(ct);

    public Task<int> ContarTarefasNaSemanaAsync(string contaId, DateOnly inicio, DateOnly fim, CancellationToken ct) =>
        banco.Tarefas.AsNoTracking()
            .CountAsync(
                tarefa => tarefa.ContaId == contaId
                    && tarefa.Prazo != null
                    && tarefa.Prazo >= inicio
                    && tarefa.Prazo <= fim,
                ct);

    public async Task<IReadOnlyList<ModeloBriefing>> ListarBriefingsAsync(string contaId, string? nome, CancellationToken ct)
    {
        var consulta = BriefingsCompletos().AsNoTracking().Where(modelo => modelo.ContaId == contaId);
        if (!string.IsNullOrWhiteSpace(nome))
        {
            var termo = nome.Trim().ToLower();
            consulta = consulta.Where(modelo => modelo.Nome.ToLower().Contains(termo));
        }

        return await consulta.OrderBy(modelo => modelo.Nome).ToListAsync(ct);
    }

    public Task<ModeloBriefing?> ObterBriefingAsync(string contaId, Guid id, CancellationToken ct) =>
        BriefingsCompletos().FirstOrDefaultAsync(modelo => modelo.Id == id && modelo.ContaId == contaId, ct);

    public async Task InserirBriefingAsync(ModeloBriefing modelo, CancellationToken ct)
    {
        banco.ModelosBriefing.Add(modelo);
        await banco.SaveChangesAsync(ct);
    }

    public Task SalvarBriefingAsync(CancellationToken ct) => banco.SaveChangesAsync(ct);

    public async Task RemoverBriefingAsync(ModeloBriefing modelo, CancellationToken ct)
    {
        banco.ModelosBriefing.Remove(modelo);
        await banco.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<ModeloRoteiro>> ListarRoteirosAsync(string contaId, string? nome, CancellationToken ct)
    {
        var consulta = RoteirosCompletos().AsNoTracking().Where(modelo => modelo.ContaId == contaId);
        if (!string.IsNullOrWhiteSpace(nome))
        {
            var termo = nome.Trim().ToLower();
            consulta = consulta.Where(modelo => modelo.Nome.ToLower().Contains(termo));
        }

        return await consulta.OrderBy(modelo => modelo.Nome).ToListAsync(ct);
    }

    public Task<ModeloRoteiro?> ObterRoteiroAsync(string contaId, Guid id, CancellationToken ct) =>
        RoteirosCompletos().FirstOrDefaultAsync(modelo => modelo.Id == id && modelo.ContaId == contaId, ct);

    public async Task InserirRoteiroAsync(ModeloRoteiro modelo, CancellationToken ct)
    {
        banco.ModelosRoteiro.Add(modelo);
        await banco.SaveChangesAsync(ct);
    }

    public Task SalvarRoteiroAsync(CancellationToken ct) => banco.SaveChangesAsync(ct);

    public async Task RemoverRoteiroAsync(ModeloRoteiro modelo, CancellationToken ct)
    {
        banco.ModelosRoteiro.Remove(modelo);
        await banco.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<ModeloCronograma>> ListarCronogramasAsync(string contaId, string? nome, CancellationToken ct)
    {
        var consulta = CronogramasCompletos().AsNoTracking().Where(modelo => modelo.ContaId == contaId);
        if (!string.IsNullOrWhiteSpace(nome))
        {
            var termo = nome.Trim().ToLower();
            consulta = consulta.Where(modelo => modelo.Titulo.ToLower().Contains(termo));
        }

        return await consulta.OrderBy(modelo => modelo.Titulo).ToListAsync(ct);
    }

    public Task<ModeloCronograma?> ObterCronogramaAsync(string contaId, Guid id, CancellationToken ct) =>
        CronogramasCompletos().FirstOrDefaultAsync(modelo => modelo.Id == id && modelo.ContaId == contaId, ct);

    public async Task InserirCronogramaAsync(ModeloCronograma modelo, CancellationToken ct)
    {
        banco.ModelosCronograma.Add(modelo);
        await banco.SaveChangesAsync(ct);
    }

    public Task SalvarCronogramaAsync(CancellationToken ct) => banco.SaveChangesAsync(ct);

    public async Task RemoverCronogramaAsync(ModeloCronograma modelo, CancellationToken ct)
    {
        banco.ModelosCronograma.Remove(modelo);
        await banco.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<ModeloInspiracao>> ListarInspiracoesAsync(string contaId, string? nome, CancellationToken ct)
    {
        var consulta = InspiracoesCompletas().AsNoTracking().Where(modelo => modelo.ContaId == contaId);
        if (!string.IsNullOrWhiteSpace(nome))
        {
            var termo = nome.Trim().ToLower();
            consulta = consulta.Where(modelo => modelo.Nome.ToLower().Contains(termo));
        }

        return await consulta.OrderBy(modelo => modelo.Nome).ToListAsync(ct);
    }

    public Task<ModeloInspiracao?> ObterInspiracaoAsync(string contaId, Guid id, CancellationToken ct) =>
        InspiracoesCompletas().FirstOrDefaultAsync(modelo => modelo.Id == id && modelo.ContaId == contaId, ct);

    public async Task InserirInspiracaoAsync(ModeloInspiracao modelo, CancellationToken ct)
    {
        banco.ModelosInspiracao.Add(modelo);
        await banco.SaveChangesAsync(ct);
    }

    public Task SalvarInspiracaoAsync(CancellationToken ct) => banco.SaveChangesAsync(ct);

    public async Task RemoverInspiracaoAsync(ModeloInspiracao modelo, CancellationToken ct)
    {
        banco.ModelosInspiracao.Remove(modelo);
        await banco.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<ModeloMensagem>> ListarMensagensAsync(string contaId, string? nome, CancellationToken ct)
    {
        var consulta = banco.ModelosMensagem.AsNoTracking().Where(modelo => modelo.ContaId == contaId);
        if (!string.IsNullOrWhiteSpace(nome))
        {
            var termo = nome.Trim().ToLower();
            consulta = consulta.Where(modelo => modelo.Nome.ToLower().Contains(termo));
        }

        return await consulta.OrderBy(modelo => modelo.Nome).ToListAsync(ct);
    }

    public Task<ModeloMensagem?> ObterMensagemAsync(string contaId, Guid id, CancellationToken ct) =>
        banco.ModelosMensagem.FirstOrDefaultAsync(modelo => modelo.Id == id && modelo.ContaId == contaId, ct);

    public async Task InserirMensagemAsync(ModeloMensagem modelo, CancellationToken ct)
    {
        banco.ModelosMensagem.Add(modelo);
        await banco.SaveChangesAsync(ct);
    }

    public Task SalvarMensagemAsync(CancellationToken ct) => banco.SaveChangesAsync(ct);

    public async Task RemoverMensagemAsync(ModeloMensagem modelo, CancellationToken ct)
    {
        banco.ModelosMensagem.Remove(modelo);
        await banco.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<ServicoConta>> ListarServicosContaAsync(string contaId, string? nome, CancellationToken ct)
    {
        var consulta = banco.ServicosConta.AsNoTracking().Where(servico => servico.ContaId == contaId);
        if (!string.IsNullOrWhiteSpace(nome))
        {
            var termo = nome.Trim().ToLower();
            consulta = consulta.Where(servico => servico.Nome.ToLower().Contains(termo));
        }

        return await consulta.OrderBy(servico => servico.Nome).ToListAsync(ct);
    }

    public Task<ServicoConta?> ObterServicoContaAsync(string contaId, Guid id, CancellationToken ct) =>
        banco.ServicosConta.FirstOrDefaultAsync(servico => servico.Id == id && servico.ContaId == contaId, ct);

    public Task<ServicoConta?> ObterServicoContaPorNomeAsync(string contaId, string nome, CancellationToken ct)
    {
        var alvo = nome.ToLower();
        return banco.ServicosConta.FirstOrDefaultAsync(
            servico => servico.ContaId == contaId && servico.Nome.ToLower() == alvo,
            ct);
    }

    public async Task InserirServicoContaAsync(ServicoConta servico, CancellationToken ct)
    {
        banco.ServicosConta.Add(servico);
        await banco.SaveChangesAsync(ct);
    }

    public async Task RemoverServicoContaAsync(ServicoConta servico, CancellationToken ct)
    {
        banco.ServicosConta.Remove(servico);
        await banco.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Fornecedor>> ListarFornecedoresAsync(string contaId, string? nome, CancellationToken ct)
    {
        var consulta = FornecedoresCompletos().AsNoTracking().Where(fornecedor => fornecedor.ContaId == contaId);
        if (!string.IsNullOrWhiteSpace(nome))
        {
            var termo = nome.Trim().ToLower();
            consulta = consulta.Where(fornecedor => fornecedor.Nome.ToLower().Contains(termo));
        }

        return await consulta.OrderBy(fornecedor => fornecedor.Nome).ToListAsync(ct);
    }

    public Task<Fornecedor?> ObterFornecedorAsync(string contaId, Guid id, CancellationToken ct) =>
        FornecedoresCompletos().FirstOrDefaultAsync(fornecedor => fornecedor.Id == id && fornecedor.ContaId == contaId, ct);

    public async Task InserirFornecedorAsync(Fornecedor fornecedor, CancellationToken ct)
    {
        banco.Fornecedores.Add(fornecedor);
        await banco.SaveChangesAsync(ct);
    }

    public Task SalvarFornecedorAsync(CancellationToken ct) => banco.SaveChangesAsync(ct);

    public async Task RemoverFornecedorAsync(Fornecedor fornecedor, CancellationToken ct)
    {
        banco.Fornecedores.Remove(fornecedor);
        await banco.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyDictionary<Guid, string>> NomesServicosContaAsync(string contaId, IEnumerable<Guid> ids, CancellationToken ct)
    {
        var lista = ids.Distinct().ToArray();
        if (lista.Length == 0)
        {
            return new Dictionary<Guid, string>();
        }

        return await banco.ServicosConta.AsNoTracking()
            .Where(servico => servico.ContaId == contaId && lista.Contains(servico.Id))
            .ToDictionaryAsync(servico => servico.Id, servico => servico.Nome, ct);
    }

    public async Task<IReadOnlyDictionary<Guid, string>> NomesFornecedoresAsync(string contaId, IEnumerable<Guid> ids, CancellationToken ct)
    {
        var lista = ids.Distinct().ToArray();
        if (lista.Length == 0)
        {
            return new Dictionary<Guid, string>();
        }

        return await banco.Fornecedores.AsNoTracking()
            .Where(fornecedor => fornecedor.ContaId == contaId && lista.Contains(fornecedor.Id))
            .ToDictionaryAsync(fornecedor => fornecedor.Id, fornecedor => fornecedor.Nome, ct);
    }

    public Task SalvarNegocioAsync(CancellationToken ct) => banco.SaveChangesAsync(ct);

    public async Task<IReadOnlyList<CompromissoAgenda>> ListarCompromissosAsync(
        string contaId,
        DateOnly? de,
        DateOnly? ate,
        CancellationToken ct)
    {
        var consulta = banco.Compromissos.AsNoTracking()
            .Where(item => item.ContaId == contaId && item.Data != null);
        if (de is DateOnly inicio)
        {
            consulta = consulta.Where(item => item.Data >= inicio);
        }

        if (ate is DateOnly fim)
        {
            consulta = consulta.Where(item => item.Data <= fim);
        }

        return await consulta.OrderBy(item => item.Data).ThenBy(item => item.Hora).ToListAsync(ct);
    }

    public Task<CompromissoAgenda?> ObterCompromissoAsync(string contaId, Guid id, CancellationToken ct) =>
        banco.Compromissos.FirstOrDefaultAsync(item => item.ContaId == contaId && item.Id == id, ct);

    public async Task InserirCompromissoAsync(CompromissoAgenda compromisso, CancellationToken ct)
    {
        banco.Compromissos.Add(compromisso);
        await banco.SaveChangesAsync(ct);
    }

    public async Task RemoverCompromissoAsync(CompromissoAgenda compromisso, CancellationToken ct)
    {
        banco.Compromissos.Remove(compromisso);
        await banco.SaveChangesAsync(ct);
    }

    public Task<bool> TemCompromissoNaDataAsync(string contaId, DateOnly data, CancellationToken ct) =>
        banco.Compromissos.AsNoTracking().AnyAsync(item => item.ContaId == contaId && item.Data == data, ct);

    public async Task<IReadOnlyList<Oportunidade>> ListarOportunidadesAsync(
        string contaId,
        FiltroOportunidades filtro,
        CancellationToken ct)
    {
        var consulta = OportunidadesCompletas().AsNoTracking().Where(item => item.ContaId == contaId);
        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            var nome = filtro.Nome.Trim();
            consulta = consulta.Where(item => EF.Functions.ILike(item.Nome, $"%{nome}%"));
        }

        if (!string.IsNullOrWhiteSpace(filtro.Responsavel))
        {
            var responsavel = filtro.Responsavel.Trim();
            consulta = consulta.Where(item => item.Responsavel == responsavel);
        }

        if (!string.IsNullOrWhiteSpace(filtro.TipoEvento) &&
            Enum.TryParse<TipoEvento>(filtro.TipoEvento, true, out var tipo))
        {
            consulta = consulta.Where(item => item.TipoEvento == tipo);
        }

        if (filtro.DataDe is DateOnly de)
        {
            consulta = consulta.Where(item => item.DataPrevista >= de);
        }

        if (filtro.DataAte is DateOnly ate)
        {
            consulta = consulta.Where(item => item.DataPrevista <= ate);
        }

        consulta = (filtro.Ordenacao ?? "").ToLowerInvariant() switch
        {
            "nome" => consulta.OrderBy(item => item.Nome),
            "valor" => consulta.OrderByDescending(item => item.ValorProposta),
            "data" => consulta.OrderBy(item => item.DataPrevista),
            _ => consulta.OrderByDescending(item => item.CriadoEm),
        };

        return await consulta.ToListAsync(ct);
    }

    public Task<Oportunidade?> ObterOportunidadeAsync(string contaId, Guid id, CancellationToken ct) =>
        OportunidadesCompletas().FirstOrDefaultAsync(item => item.ContaId == contaId && item.Id == id, ct);

    public async Task InserirOportunidadeAsync(Oportunidade oportunidade, CancellationToken ct)
    {
        banco.Oportunidades.Add(oportunidade);
        await banco.SaveChangesAsync(ct);
    }

    public async Task RemoverOportunidadeAsync(Oportunidade oportunidade, CancellationToken ct)
    {
        banco.Oportunidades.Remove(oportunidade);
        await banco.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<ContagemOportunidadeEstagio>> ContarOportunidadesPorEstagioAsync(
        string contaId,
        CancellationToken ct)
    {
        var grupos = await banco.Oportunidades.AsNoTracking()
            .Where(item => item.ContaId == contaId)
            .GroupBy(item => item.Estagio)
            .Select(grupo => new
            {
                Estagio = grupo.Key,
                Quantidade = grupo.Count(),
                Valor = grupo.Sum(item => item.ValorProposta ?? 0),
            })
            .ToListAsync(ct);
        return grupos.Select(g => new ContagemOportunidadeEstagio(g.Estagio, g.Quantidade, g.Valor)).ToArray();
    }

    public async Task<IReadOnlyDictionary<Guid, string>> NomesOportunidadesAsync(
        string contaId,
        IEnumerable<Guid> ids,
        CancellationToken ct)
    {
        var lista = ids.Distinct().ToArray();
        if (lista.Length == 0)
        {
            return new Dictionary<Guid, string>();
        }

        return await banco.Oportunidades.AsNoTracking()
            .Where(item => item.ContaId == contaId && lista.Contains(item.Id))
            .ToDictionaryAsync(item => item.Id, item => item.Nome, ct);
    }

    public async Task<IReadOnlyList<Oportunidade>> ListarOportunidadesPorCriacaoAsync(
        string contaId,
        DateTimeOffset inicio,
        DateTimeOffset fim,
        CancellationToken ct) =>
        await OportunidadesCompletas().AsNoTracking()
            .Where(item => item.ContaId == contaId && item.CriadoEm >= inicio && item.CriadoEm <= fim)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Oportunidade>> ListarOportunidadesPorEntradaEstagioAsync(
        string contaId,
        DateTimeOffset inicio,
        DateTimeOffset fim,
        CancellationToken ct) =>
        await OportunidadesCompletas().AsNoTracking()
            .Where(item => item.ContaId == contaId && item.EntradaNoEstagioEm >= inicio && item.EntradaNoEstagioEm <= fim)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<LeadRsvp>> ListarLeadsRsvpAtivosAsync(string contaId, string? busca, CancellationToken ct)
    {
        var consulta = banco.LeadsRsvp.AsNoTracking()
            .Where(item => item.ContaId == contaId && item.Situacao == SituacaoLeadRsvp.Ativo);
        if (!string.IsNullOrWhiteSpace(busca))
        {
            var texto = busca.Trim();
            consulta = consulta.Where(item =>
                EF.Functions.ILike(item.NomeConvite, $"%{texto}%") ||
                EF.Functions.ILike(item.NomeEvento, $"%{texto}%"));
        }

        return await consulta.OrderByDescending(item => item.CriadoEm).ToListAsync(ct);
    }

    public Task<LeadRsvp?> ObterLeadRsvpAsync(string contaId, Guid id, CancellationToken ct) =>
        banco.LeadsRsvp.FirstOrDefaultAsync(item => item.ContaId == contaId && item.Id == id, ct);

    public async Task InserirLeadRsvpAsync(LeadRsvp lead, CancellationToken ct)
    {
        banco.LeadsRsvp.Add(lead);
        await banco.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<LeadRsvp>> ListarLeadsRsvpPeriodoAsync(
        string contaId,
        DateTimeOffset inicio,
        DateTimeOffset fim,
        CancellationToken ct) =>
        await banco.LeadsRsvp.AsNoTracking()
            .Where(item => item.ContaId == contaId && item.CriadoEm >= inicio && item.CriadoEm <= fim)
            .ToListAsync(ct);

    public Task<int> ContarLeadsRsvpConvertidosAntesAsync(string contaId, DateTimeOffset limite, CancellationToken ct) =>
        banco.LeadsRsvp.AsNoTracking().CountAsync(
            item => item.ContaId == contaId &&
                    item.Situacao == SituacaoLeadRsvp.Convertido &&
                    item.CriadoEm < limite,
            ct);

    public Task<MetricasRsvpFunil?> ObterMetricasRsvpFunilAsync(string contaId, CancellationToken ct) =>
        banco.MetricasRsvpFunil.AsNoTracking().FirstOrDefaultAsync(item => item.ContaId == contaId, ct);

    public async Task IncrementarVisualizacaoRsvpAsync(string contaId, CancellationToken ct)
    {
        var metricas = await GarantirMetricasRsvpAsync(contaId, ct);
        metricas.Visualizacoes++;
        await banco.SaveChangesAsync(ct);
    }

    public async Task IncrementarCliqueRsvpAsync(string contaId, CancellationToken ct)
    {
        var metricas = await GarantirMetricasRsvpAsync(contaId, ct);
        metricas.CliquesBotao++;
        await banco.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<ContratoFunil>> ListarContratosFunilAsync(
        string contaId,
        SituacaoContratoFunil? situacao,
        string? busca,
        DateOnly? criadoDe,
        DateOnly? criadoAte,
        CancellationToken ct)
    {
        var consulta = banco.ContratosFunil.AsNoTracking().Where(item => item.ContaId == contaId);
        if (situacao is SituacaoContratoFunil filtro)
        {
            consulta = consulta.Where(item => item.Situacao == filtro);
        }

        if (criadoDe is DateOnly de)
        {
            var inicio = new DateTimeOffset(de.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
            consulta = consulta.Where(item => item.CriadoEm >= inicio);
        }

        if (criadoAte is DateOnly ate)
        {
            var fim = new DateTimeOffset(ate.ToDateTime(TimeOnly.MaxValue), TimeSpan.Zero);
            consulta = consulta.Where(item => item.CriadoEm <= fim);
        }

        var itens = await consulta.OrderByDescending(item => item.CriadoEm).ToListAsync(ct);
        if (string.IsNullOrWhiteSpace(busca))
        {
            return itens;
        }

        var texto = busca.Trim();
        var nomes = await NomesOportunidadesAsync(contaId, itens.Select(i => i.OportunidadeId), ct);
        return itens.Where(item =>
            nomes.TryGetValue(item.OportunidadeId, out var nome) &&
            nome.Contains(texto, StringComparison.OrdinalIgnoreCase)).ToArray();
    }

    public Task<ContratoFunil?> ObterContratoFunilAsync(string contaId, Guid id, CancellationToken ct) =>
        banco.ContratosFunil.FirstOrDefaultAsync(item => item.ContaId == contaId && item.Id == id, ct);

    public async Task InserirContratoFunilAsync(ContratoFunil contrato, CancellationToken ct)
    {
        banco.ContratosFunil.Add(contrato);
        await banco.SaveChangesAsync(ct);
    }

    public async Task RemoverContratoFunilAsync(ContratoFunil contrato, CancellationToken ct)
    {
        banco.ContratosFunil.Remove(contrato);
        await banco.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<OrcamentoFunil>> ListarOrcamentosFunilAsync(
        string contaId,
        SituacaoOrcamentoFunil? situacao,
        string? busca,
        DateOnly? criadoDe,
        DateOnly? criadoAte,
        CancellationToken ct)
    {
        var consulta = banco.OrcamentosFunil.AsNoTracking().Where(item => item.ContaId == contaId);
        if (situacao is SituacaoOrcamentoFunil filtro)
        {
            consulta = consulta.Where(item => item.Situacao == filtro);
        }

        if (criadoDe is DateOnly de)
        {
            var inicio = new DateTimeOffset(de.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
            consulta = consulta.Where(item => item.CriadoEm >= inicio);
        }

        if (criadoAte is DateOnly ate)
        {
            var fim = new DateTimeOffset(ate.ToDateTime(TimeOnly.MaxValue), TimeSpan.Zero);
            consulta = consulta.Where(item => item.CriadoEm <= fim);
        }

        var itens = await consulta.OrderByDescending(item => item.CriadoEm).ToListAsync(ct);
        if (string.IsNullOrWhiteSpace(busca))
        {
            return itens;
        }

        var texto = busca.Trim();
        var nomes = await NomesOportunidadesAsync(contaId, itens.Select(i => i.OportunidadeId), ct);
        return itens.Where(item =>
            nomes.TryGetValue(item.OportunidadeId, out var nome) &&
            nome.Contains(texto, StringComparison.OrdinalIgnoreCase)).ToArray();
    }

    public Task<OrcamentoFunil?> ObterOrcamentoFunilAsync(string contaId, Guid id, CancellationToken ct) =>
        banco.OrcamentosFunil.FirstOrDefaultAsync(item => item.ContaId == contaId && item.Id == id, ct);

    public async Task InserirOrcamentoFunilAsync(OrcamentoFunil orcamento, CancellationToken ct)
    {
        banco.OrcamentosFunil.Add(orcamento);
        await banco.SaveChangesAsync(ct);
    }

    public async Task RemoverOrcamentoFunilAsync(OrcamentoFunil orcamento, CancellationToken ct)
    {
        banco.OrcamentosFunil.Remove(orcamento);
        await banco.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<TarefaFunil>> ListarTarefasFunilAsync(
        string contaId,
        DateOnly? prazoDe,
        DateOnly? prazoAte,
        TipoTarefaFunil? tipo,
        string? responsavel,
        bool? completa,
        CancellationToken ct)
    {
        var consulta = banco.TarefasFunil.AsNoTracking().Where(item => item.ContaId == contaId);
        if (prazoDe is DateOnly de)
        {
            consulta = consulta.Where(item => item.Prazo >= de);
        }

        if (prazoAte is DateOnly ate)
        {
            consulta = consulta.Where(item => item.Prazo <= ate);
        }

        if (tipo is TipoTarefaFunil tipoFiltro)
        {
            consulta = consulta.Where(item => item.Tipo == tipoFiltro);
        }

        if (!string.IsNullOrWhiteSpace(responsavel))
        {
            var texto = responsavel.Trim();
            consulta = consulta.Where(item => item.Responsavel == texto);
        }

        if (completa is bool completaFiltro)
        {
            consulta = consulta.Where(item => item.Completa == completaFiltro);
        }

        return await consulta.OrderBy(item => item.Prazo).ToListAsync(ct);
    }

    public Task<TarefaFunil?> ObterTarefaFunilAsync(string contaId, Guid id, CancellationToken ct) =>
        banco.TarefasFunil.FirstOrDefaultAsync(item => item.ContaId == contaId && item.Id == id, ct);

    public async Task InserirTarefaFunilAsync(TarefaFunil tarefa, CancellationToken ct)
    {
        banco.TarefasFunil.Add(tarefa);
        await banco.SaveChangesAsync(ct);
    }

    public async Task RemoverTarefaFunilAsync(TarefaFunil tarefa, CancellationToken ct)
    {
        banco.TarefasFunil.Remove(tarefa);
        await banco.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<ModeloServicoFunil>> ListarModelosServicoFunilAsync(
        string contaId,
        string? nome,
        CancellationToken ct)
    {
        var consulta = banco.ModelosServicoFunil.AsNoTracking().Where(item => item.ContaId == contaId);
        if (!string.IsNullOrWhiteSpace(nome))
        {
            var texto = nome.Trim();
            consulta = consulta.Where(item => EF.Functions.ILike(item.Nome, $"%{texto}%"));
        }

        return await consulta.OrderBy(item => item.Nome).ToListAsync(ct);
    }

    public Task<ModeloServicoFunil?> ObterModeloServicoFunilAsync(string contaId, Guid id, CancellationToken ct) =>
        banco.ModelosServicoFunil.FirstOrDefaultAsync(item => item.ContaId == contaId && item.Id == id, ct);

    public async Task InserirModeloServicoFunilAsync(ModeloServicoFunil modelo, CancellationToken ct)
    {
        banco.ModelosServicoFunil.Add(modelo);
        await banco.SaveChangesAsync(ct);
    }

    public async Task RemoverModeloServicoFunilAsync(ModeloServicoFunil modelo, CancellationToken ct)
    {
        banco.ModelosServicoFunil.Remove(modelo);
        await banco.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<ModeloOrcamentoFunil>> ListarModelosOrcamentoFunilAsync(
        string contaId,
        string? nome,
        CancellationToken ct)
    {
        var consulta = banco.ModelosOrcamentoFunil.AsNoTracking().Where(item => item.ContaId == contaId);
        if (!string.IsNullOrWhiteSpace(nome))
        {
            var texto = nome.Trim();
            consulta = consulta.Where(item => EF.Functions.ILike(item.Nome, $"%{texto}%"));
        }

        return await consulta.OrderBy(item => item.Nome).ToListAsync(ct);
    }

    public Task<ModeloOrcamentoFunil?> ObterModeloOrcamentoFunilAsync(string contaId, Guid id, CancellationToken ct) =>
        banco.ModelosOrcamentoFunil.FirstOrDefaultAsync(item => item.ContaId == contaId && item.Id == id, ct);

    public async Task InserirModeloOrcamentoFunilAsync(ModeloOrcamentoFunil modelo, CancellationToken ct)
    {
        banco.ModelosOrcamentoFunil.Add(modelo);
        await banco.SaveChangesAsync(ct);
    }

    public async Task RemoverModeloOrcamentoFunilAsync(ModeloOrcamentoFunil modelo, CancellationToken ct)
    {
        banco.ModelosOrcamentoFunil.Remove(modelo);
        await banco.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<ModeloContratoFunil>> ListarModelosContratoFunilAsync(
        string contaId,
        string? nome,
        CancellationToken ct)
    {
        var consulta = banco.ModelosContratoFunil.AsNoTracking().Where(item => item.ContaId == contaId);
        if (!string.IsNullOrWhiteSpace(nome))
        {
            var texto = nome.Trim();
            consulta = consulta.Where(item => EF.Functions.ILike(item.Nome, $"%{texto}%"));
        }

        return await consulta.OrderBy(item => item.Nome).ToListAsync(ct);
    }

    public Task<ModeloContratoFunil?> ObterModeloContratoFunilAsync(string contaId, Guid id, CancellationToken ct) =>
        banco.ModelosContratoFunil.FirstOrDefaultAsync(item => item.ContaId == contaId && item.Id == id, ct);

    public async Task InserirModeloContratoFunilAsync(ModeloContratoFunil modelo, CancellationToken ct)
    {
        banco.ModelosContratoFunil.Add(modelo);
        await banco.SaveChangesAsync(ct);
    }

    public async Task RemoverModeloContratoFunilAsync(ModeloContratoFunil modelo, CancellationToken ct)
    {
        banco.ModelosContratoFunil.Remove(modelo);
        await banco.SaveChangesAsync(ct);
    }

    public async Task<(string ContaId, string Nome)?> ResumoEventoPorIdAsync(Guid eventoId, CancellationToken ct)
    {
        var evento = await banco.Eventos.AsNoTracking()
            .Where(item => item.Id == eventoId)
            .Select(item => new { item.ContaId, item.Nome })
            .FirstOrDefaultAsync(ct);
        return evento is null ? null : (evento.ContaId, evento.Nome);
    }

    private async Task<MetricasRsvpFunil> GarantirMetricasRsvpAsync(string contaId, CancellationToken ct)
    {
        var metricas = await banco.MetricasRsvpFunil.FirstOrDefaultAsync(item => item.ContaId == contaId, ct);
        if (metricas is not null)
        {
            return metricas;
        }

        metricas = new MetricasRsvpFunil { Id = Guid.NewGuid(), ContaId = contaId };
        banco.MetricasRsvpFunil.Add(metricas);
        await banco.SaveChangesAsync(ct);
        return metricas;
    }

    private IQueryable<Oportunidade> OportunidadesCompletas() =>
        banco.Oportunidades.Include(item => item.Contatos);

    private IQueryable<ModeloEvento> ModelosCompletos() =>
        banco.Modelos
            .Include(modelo => modelo.Servicos).ThenInclude(servico => servico.Itens)
            .Include(modelo => modelo.Tarefas)
            .Include(modelo => modelo.Vinculos);

    private IQueryable<ModeloBriefing> BriefingsCompletos() =>
        banco.ModelosBriefing
            .Include(modelo => modelo.Secoes).ThenInclude(secao => secao.Perguntas);

    private IQueryable<ModeloRoteiro> RoteirosCompletos() =>
        banco.ModelosRoteiro
            .Include(modelo => modelo.Secoes).ThenInclude(secao => secao.Itens);

    private IQueryable<ModeloCronograma> CronogramasCompletos() =>
        banco.ModelosCronograma.Include(modelo => modelo.Itens);

    private IQueryable<ModeloInspiracao> InspiracoesCompletas() =>
        banco.ModelosInspiracao.Include(modelo => modelo.Itens);

    private IQueryable<Fornecedor> FornecedoresCompletos() =>
        banco.Fornecedores
            .Include(fornecedor => fornecedor.Telefones)
            .Include(fornecedor => fornecedor.Emails)
            .Include(fornecedor => fornecedor.Orcamentos)
            .Include(fornecedor => fornecedor.Servicos);
}
