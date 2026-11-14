using Project.Application.Comum;
using Project.Domain.Funil;

namespace Project.Application.Negocio;

public sealed record LeadRsvpLeitura(
    Guid Id,
    string Convite,
    string Evento,
    DateTimeOffset CriadoEm,
    string? Telefone,
    Guid ConviteId,
    Guid EventoId);

public sealed class LeadRsvpServico(IDados dados)
{
    public async Task<IReadOnlyList<LeadRsvpLeitura>> ListarAsync(string contaId, string? busca, CancellationToken ct)
    {
        var itens = await dados.ListarLeadsRsvpAtivosAsync(contaId, busca, ct);
        return itens.Select(Ler).ToArray();
    }

    public async Task<Resultado<OportunidadeLeitura>> GerarOportunidadeAsync(string contaId, Guid id, CancellationToken ct)
    {
        var lead = await dados.ObterLeadRsvpAsync(contaId, id, ct);
        if (lead is null || lead.Situacao != SituacaoLeadRsvp.Ativo)
        {
            return Resultado<OportunidadeLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var agora = DateTimeOffset.UtcNow;
        var oportunidade = new Oportunidade
        {
            Id = Guid.NewGuid(),
            ContaId = contaId,
            Nome = lead.NomeConvite,
            Estagio = EstagioOportunidade.ContatoInicial,
            Fonte = "RSVP",
            Responsavel = contaId,
            CriadoEm = agora,
            EntradaNoEstagioEm = agora,
            Contatos =
            [
                new ContatoOportunidade
                {
                    Id = Guid.NewGuid(),
                    Nome = lead.NomeConvite,
                    Telefone = lead.Telefone,
                },
            ],
        };
        oportunidade.Contatos[0].OportunidadeId = oportunidade.Id;
        await dados.InserirOportunidadeAsync(oportunidade, ct);

        lead.Situacao = SituacaoLeadRsvp.Convertido;
        lead.OportunidadeId = oportunidade.Id;
        await dados.SalvarNegocioAsync(ct);
        return Resultado<OportunidadeLeitura>.Sucesso(OportunidadeServico.Ler(oportunidade));
    }

    public async Task<Resultado<bool>> DescartarAsync(string contaId, Guid id, CancellationToken ct)
    {
        var lead = await dados.ObterLeadRsvpAsync(contaId, id, ct);
        if (lead is null || lead.Situacao != SituacaoLeadRsvp.Ativo)
        {
            return Resultado<bool>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        lead.Situacao = SituacaoLeadRsvp.Descartado;
        await dados.SalvarNegocioAsync(ct);
        return Resultado<bool>.Sucesso(true);
    }

    public async Task<Resultado<bool>> MarcarContatadoAsync(string contaId, Guid id, CancellationToken ct)
    {
        var lead = await dados.ObterLeadRsvpAsync(contaId, id, ct);
        if (lead is null || lead.Situacao != SituacaoLeadRsvp.Ativo)
        {
            return Resultado<bool>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        lead.ContatadoEm ??= DateTimeOffset.UtcNow;
        await dados.SalvarNegocioAsync(ct);
        return Resultado<bool>.Sucesso(true);
    }

    private static LeadRsvpLeitura Ler(LeadRsvp item) =>
        new(item.Id, item.NomeConvite, item.NomeEvento, item.CriadoEm, item.Telefone, item.ConviteId, item.EventoId);
}
