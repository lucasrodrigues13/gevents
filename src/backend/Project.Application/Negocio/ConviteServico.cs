using Project.Application.Comum;
using Project.Application.Convidados;
using Project.Domain.Convidados;
using Project.Domain.Eventos;

namespace Project.Application.Negocio;

public sealed record ConvidadoPedido(
    string? Nome,
    string? Ddi,
    string? Celular,
    SituacaoConvidado? Situacao,
    string? Mesa,
    GeneroConvidado? Genero,
    FaixaEtaria? Faixa,
    CustoConvidado? Custo,
    string? Rg,
    string? Cpf,
    string? Observacao);

public sealed record NovoConvite(
    string? Nome,
    Guid? GrupoId,
    string? Observacao,
    bool UsaTransfer,
    string? LocalOrigem,
    IReadOnlyList<ConvidadoPedido>? Convidados,
    int? Principal);

public sealed record ConvidadoLeitura(
    Guid Id,
    string Nome,
    string? Ddi,
    string? Celular,
    SituacaoConvidado Situacao,
    string SituacaoTela,
    string SituacaoIcone,
    string? Mesa,
    GeneroConvidado? Genero,
    FaixaEtaria? Faixa,
    CustoConvidado? Custo,
    string? Rg,
    string? Cpf,
    string? Observacao,
    bool Principal);

public sealed record ConviteLeitura(
    Guid Id,
    Guid EventoId,
    string Nome,
    Guid? GrupoId,
    string? Grupo,
    string? Observacao,
    bool UsaTransfer,
    string? LocalOrigem,
    string? Telefone,
    string Token,
    bool Acessada,
    bool Respondida,
    IReadOnlyList<ConvidadoLeitura> Convidados);

public sealed record TotaisLeitura(string Rotulo, int Quantidade);

public sealed record ListaConvites(
    IReadOnlyList<ConviteLeitura> Convites,
    IReadOnlyList<TotaisLeitura> PorSituacao,
    IReadOnlyList<TotaisLeitura> PorFaixa,
    IReadOnlyList<TotaisLeitura> PorCusto,
    IReadOnlyList<string> Mesas);

public sealed record RsvpPublico(string Convite, IReadOnlyList<ConvidadoPublico> Convidados);

public sealed record ConvidadoPublico(Guid Id, string Nome, SituacaoConvidado Situacao);

public sealed record RespostaRsvp(Guid ConvidadoId, SituacaoConvidado Situacao);

public sealed record WhatsAppLeitura(string? Url);

public sealed class ConviteServico(IDados dados, IPlanilhaConvites planilha, IGeradorQr qr)
{
    public async Task<Resultado<ListaConvites>> ListarAsync(
        string contaId,
        Guid eventoId,
        FiltroConvites filtro,
        CancellationToken ct)
    {
        if (!await dados.EventoDaContaAsync(contaId, eventoId, ct))
        {
            return Resultado<ListaConvites>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var grupos = await dados.ListarGruposAsync(contaId, ct);
        var convites = await dados.ListarConvitesAsync(contaId, eventoId, ct);
        var todos = convites.SelectMany(convite => convite.Convidados).ToArray();
        var filtrados = convites
            .Select(convite => Filtrar(convite, filtro))
            .Where(convite => convite is not null)
            .Cast<Convite>()
            .Select(convite => Ler(convite, grupos))
            .ToArray();

        return Resultado<ListaConvites>.Sucesso(new ListaConvites(
            filtrados,
            Contar(todos, item => Rotulos.SituacaoTela(item.Situacao)),
            Contar(todos.Where(item => item.Faixa is not null), item => RotuloFaixa(item.Faixa!.Value)),
            Contar(todos.Where(item => item.Custo is not null), item => RotuloCusto(item.Custo!.Value)),
            todos.Select(item => Domain.Comum.Texto.Limpo(item.Mesa)).OfType<string>().Distinct(StringComparer.OrdinalIgnoreCase).ToArray()));
    }

    public async Task<Resultado<ConviteLeitura>> CriarAsync(string contaId, Guid eventoId, NovoConvite pedido, CancellationToken ct)
    {
        var evento = await dados.ObterEventoAsync(contaId, eventoId, ct);
        if (evento is null)
        {
            return Resultado<ConviteLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        if (evento.Situacao != SituacaoEvento.EmAndamento)
        {
            return Resultado<ConviteLeitura>.Erro(Codigos.Validacao, "O evento não está em andamento.");
        }

        var montado = await MontarAsync(contaId, evento, pedido, ct);
        if (!montado.Ok)
        {
            return Resultado<ConviteLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        await dados.InserirConvitesAsync([montado.Valor!], ct);
        var grupos = await dados.ListarGruposAsync(contaId, ct);
        return Resultado<ConviteLeitura>.Sucesso(Ler(montado.Valor!, grupos));
    }

    public async Task<Resultado<ConviteLeitura>> AtualizarAsync(string contaId, Guid id, NovoConvite pedido, CancellationToken ct)
    {
        var convite = await dados.ObterConviteAsync(contaId, id, ct);
        if (convite is null)
        {
            return Resultado<ConviteLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var evento = await dados.ObterEventoAsync(contaId, convite.EventoId, ct);
        if (evento is null || evento.Situacao != SituacaoEvento.EmAndamento)
        {
            return Resultado<ConviteLeitura>.Erro(Codigos.Validacao, "O evento não está em andamento.");
        }

        var nome = Domain.Comum.Texto.Limpo(pedido.Nome);
        if (nome is null)
        {
            return Resultado<ConviteLeitura>.Erro(Codigos.Validacao, "Informe o nome do convite.");
        }

        if (pedido.GrupoId is Guid grupoId && await dados.ObterGrupoAsync(contaId, grupoId, ct) is null)
        {
            return Resultado<ConviteLeitura>.Erro(Codigos.Validacao, "O grupo não pertence à conta.");
        }

        var transfer = ValidarTransfer(evento, pedido.UsaTransfer, pedido.LocalOrigem);
        if (!transfer.Ok)
        {
            return Resultado<ConviteLeitura>.Erro(transfer.Falha!.Codigo, transfer.Falha.Mensagem);
        }

        convite.Nome = nome;
        convite.GrupoId = pedido.GrupoId;
        convite.Observacao = Domain.Comum.Texto.Limpo(pedido.Observacao);
        convite.UsaTransfer = pedido.UsaTransfer && evento.TransferAtivo;
        convite.LocalOrigem = convite.UsaTransfer ? transfer.Valor : null;
        await dados.SalvarConvitesAsync(ct);
        var grupos = await dados.ListarGruposAsync(contaId, ct);
        return Resultado<ConviteLeitura>.Sucesso(Ler(convite, grupos));
    }

    public async Task<Resultado<ConvidadoLeitura>> SalvarConvidadoAsync(
        string contaId,
        Guid conviteId,
        Guid? convidadoId,
        ConvidadoPedido pedido,
        CancellationToken ct)
    {
        var convite = await dados.ObterConviteAsync(contaId, conviteId, ct);
        if (convite is null)
        {
            return Resultado<ConvidadoLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        if (!await dados.EventoEmAndamentoAsync(convite.EventoId, ct))
        {
            return Resultado<ConvidadoLeitura>.Erro(Codigos.Validacao, "O evento não está em andamento.");
        }

        var montado = MontarConvidado(pedido);
        if (!montado.Ok)
        {
            return Resultado<ConvidadoLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        Convidado convidado;
        if (convidadoId is Guid id)
        {
            var existente = convite.Convidados.FirstOrDefault(item => item.Id == id);
            if (existente is null)
            {
                return Resultado<ConvidadoLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
            }

            convidado = existente;
            Copiar(convidado, montado.Valor!);
            if (convite.PrincipalId == convidado.Id && !ConviteRegras.TemTelefone(convidado.Celular))
            {
                convite.PrincipalId = null;
            }
        }
        else
        {
            convidado = montado.Valor!;
            convidado.ConviteId = convite.Id;
            convite.Convidados.Add(convidado);
        }

        await dados.SalvarConvitesAsync(ct);
        return Resultado<ConvidadoLeitura>.Sucesso(LerConvidado(convidado, convite.PrincipalId));
    }

    public async Task<Resultado<ConviteLeitura>> DefinirPrincipalAsync(string contaId, Guid conviteId, Guid convidadoId, CancellationToken ct)
    {
        var convite = await dados.ObterConviteAsync(contaId, conviteId, ct);
        if (convite is null)
        {
            return Resultado<ConviteLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var convidado = convite.Convidados.FirstOrDefault(item => item.Id == convidadoId);
        if (!ConviteRegras.PodeSerPrincipal(convidado is not null, convidado?.Celular))
        {
            return Resultado<ConviteLeitura>.Erro(Codigos.Validacao, "Escolha um convidado deste convite que tenha telefone.");
        }

        convite.PrincipalId = convidadoId;
        await dados.SalvarConvitesAsync(ct);
        var grupos = await dados.ListarGruposAsync(contaId, ct);
        return Resultado<ConviteLeitura>.Sucesso(Ler(convite, grupos));
    }

    public async Task<Resultado<bool>> RemoverConvidadoAsync(string contaId, Guid id, CancellationToken ct)
    {
        var convidado = await dados.ObterConvidadoAsync(contaId, id, ct);
        if (convidado is null)
        {
            return Resultado<bool>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var convite = await dados.ObterConviteAsync(contaId, convidado.ConviteId, ct);
        if (convite is null)
        {
            return Resultado<bool>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        if (convite.PrincipalId == convidado.Id)
        {
            convite.PrincipalId = null;
        }

        convite.Convidados.Remove(convidado);
        await dados.RemoverConvidadoAsync(convidado, ct);
        return Resultado<bool>.Sucesso(true);
    }

    public async Task<Resultado<int>> ImportarAsync(string contaId, Guid eventoId, Stream arquivo, string? nomeArquivo, CancellationToken ct)
    {
        var evento = await dados.ObterEventoAsync(contaId, eventoId, ct);
        if (evento is null)
        {
            return Resultado<int>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        if (evento.Situacao != SituacaoEvento.EmAndamento)
        {
            return Resultado<int>.Erro(Codigos.Validacao, "O evento não está em andamento.");
        }

        var linhas = planilha.Ler(arquivo, nomeArquivo);
        if (!linhas.Ok)
        {
            return Resultado<int>.Erro(linhas.Falha!.Codigo, linhas.Falha.Mensagem);
        }

        if (linhas.Valor!.Count == 0)
        {
            return Resultado<int>.Erro(Codigos.Validacao, "A planilha não tem convidados.");
        }

        var gruposPorNome = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        var convites = new Dictionary<string, Convite>(StringComparer.OrdinalIgnoreCase);
        foreach (var linha in linhas.Valor)
        {
            var nomeConvite = Domain.Comum.Texto.Limpo(linha.Convite);
            var nomeConvidado = Domain.Comum.Texto.Limpo(linha.Convidado);
            if (nomeConvite is null || nomeConvidado is null)
            {
                return Resultado<int>.Erro(Codigos.Validacao, "A planilha tem uma linha sem nome de convite ou de convidado.");
            }

            if (!convites.TryGetValue(nomeConvite, out var convite))
            {
                Guid? grupoId = null;
                var nomeGrupo = Domain.Comum.Texto.Limpo(linha.Grupo);
                if (nomeGrupo is not null)
                {
                    if (!gruposPorNome.TryGetValue(nomeGrupo, out var existente))
                    {
                        var grupo = await dados.GarantirGrupoAsync(contaId, nomeGrupo, ct);
                        existente = grupo.Id;
                        gruposPorNome[nomeGrupo] = existente;
                    }

                    grupoId = existente;
                }

                convite = new Convite
                {
                    Id = Guid.NewGuid(),
                    EventoId = eventoId,
                    ContaId = contaId,
                    Nome = nomeConvite,
                    GrupoId = grupoId,
                    Observacao = Domain.Comum.Texto.Limpo(linha.Observacao),
                    Token = Guid.NewGuid().ToString("N"),
                };
                convites[nomeConvite] = convite;
            }

            var convidado = ConvidadoDaPlanilha(linha, nomeConvidado);
            if (!convidado.Ok)
            {
                return Resultado<int>.Erro(convidado.Falha!.Codigo, convidado.Falha.Mensagem);
            }

            convidado.Valor!.Id = Guid.NewGuid();
            convidado.Valor.ConviteId = convite.Id;
            convite.Convidados.Add(convidado.Valor);
            if (ConviteRegras.TemTelefone(convidado.Valor.Celular))
            {
                convite.PrincipalId = convidado.Valor.Id;
            }
        }

        await dados.InserirConvitesAsync(convites.Values.ToArray(), ct);
        return Resultado<int>.Sucesso(convites.Count);
    }

    public byte[] ModeloPlanilha() => planilha.Modelo();

    public async Task<Resultado<byte[]>> QrAsync(string contaId, Guid id, string basePublica, CancellationToken ct)
    {
        var convite = await dados.ObterConviteAsync(contaId, id, ct);
        if (convite is null)
        {
            return Resultado<byte[]>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        return Resultado<byte[]>.Sucesso(qr.Png(Link(basePublica, convite.Token)));
    }

    public async Task<Resultado<WhatsAppLeitura>> WhatsAppAsync(string contaId, Guid id, string basePublica, CancellationToken ct)
    {
        var convite = await dados.ObterConviteAsync(contaId, id, ct);
        if (convite is null)
        {
            return Resultado<WhatsAppLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var principal = convite.Convidados.FirstOrDefault(item => item.Id == convite.PrincipalId);
        if (!ConviteRegras.TemTelefone(principal?.Celular))
        {
            return Resultado<WhatsAppLeitura>.Sucesso(new WhatsAppLeitura(null));
        }

        var digitos = new string($"{principal!.Ddi}{principal.Celular}".Where(char.IsDigit).ToArray());
        var texto = Uri.EscapeDataString($"Confirme presença: {Link(basePublica, convite.Token)}");
        return Resultado<WhatsAppLeitura>.Sucesso(new WhatsAppLeitura($"https://wa.me/{digitos}?text={texto}"));
    }

    public async Task<Resultado<RsvpPublico>> AbrirRsvpAsync(string token, CancellationToken ct)
    {
        var convite = await dados.ObterConvitePorTokenAsync(token, ct);
        if (convite is null || !await dados.EventoEmAndamentoAsync(convite.EventoId, ct))
        {
            return Resultado<RsvpPublico>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        if (convite.RespondidoEm is null && convite.AcessadoEm is null)
        {
            convite.AcessadoEm = DateTimeOffset.UtcNow;
            await dados.SalvarConvitesAsync(ct);
        }

        var resumo = await dados.ResumoEventoPorIdAsync(convite.EventoId, ct);
        if (resumo is { } dadosEvento)
        {
            var perfil = await dados.ObterPerfilAsync(dadosEvento.ContaId, ct);
            if (perfil?.CapturaLeadRsvp != false)
            {
                await dados.IncrementarVisualizacaoRsvpAsync(dadosEvento.ContaId, ct);
            }
        }

        return Resultado<RsvpPublico>.Sucesso(new RsvpPublico(
            convite.Nome,
            convite.Convidados.Select(item => new ConvidadoPublico(item.Id, item.Nome, item.Situacao)).ToArray()));
    }

    public async Task<Resultado<RsvpPublico>> ResponderAsync(
        string token,
        IReadOnlyList<RespostaRsvp>? respostas,
        bool? pretendeRealizarEvento,
        CancellationToken ct)
    {
        var convite = await dados.ObterConvitePorTokenAsync(token, ct);
        if (convite is null || !await dados.EventoEmAndamentoAsync(convite.EventoId, ct))
        {
            return Resultado<RsvpPublico>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        if (respostas is null || respostas.Count == 0)
        {
            return Resultado<RsvpPublico>.Erro(Codigos.Validacao, "Não foi possível salvar.");
        }

        foreach (var resposta in respostas)
        {
            if (resposta.Situacao is not (SituacaoConvidado.Confirmado or SituacaoConvidado.NaoComparecera))
            {
                return Resultado<RsvpPublico>.Erro(Codigos.Validacao, "Informe se a pessoa comparece.");
            }

            var convidado = convite.Convidados.FirstOrDefault(item => item.Id == resposta.ConvidadoId);
            if (convidado is null)
            {
                return Resultado<RsvpPublico>.Erro(Codigos.Indisponivel, "Indisponível.");
            }

            convidado.Situacao = resposta.Situacao;
        }

        convite.RespondidoEm = DateTimeOffset.UtcNow;
        await dados.SalvarConvitesAsync(ct);

        if (pretendeRealizarEvento == true)
        {
            var resumo = await dados.ResumoEventoPorIdAsync(convite.EventoId, ct);
            if (resumo is { } dadosEvento)
            {
                var perfil = await dados.ObterPerfilAsync(dadosEvento.ContaId, ct);
                if (perfil?.CapturaLeadRsvp != false)
                {
                    var telefone = convite.Convidados
                        .Select(item => Domain.Comum.Texto.Limpo(item.Celular))
                        .FirstOrDefault(item => item is not null);
                    await dados.InserirLeadRsvpAsync(new Domain.Funil.LeadRsvp
                    {
                        Id = Guid.NewGuid(),
                        ContaId = dadosEvento.ContaId,
                        ConviteId = convite.Id,
                        EventoId = convite.EventoId,
                        NomeConvite = convite.Nome,
                        NomeEvento = dadosEvento.Nome,
                        Telefone = telefone,
                        CriadoEm = DateTimeOffset.UtcNow,
                        Situacao = Domain.Funil.SituacaoLeadRsvp.Ativo,
                    }, ct);
                    await dados.IncrementarCliqueRsvpAsync(dadosEvento.ContaId, ct);
                }
            }
        }

        return Resultado<RsvpPublico>.Sucesso(new RsvpPublico(
            convite.Nome,
            convite.Convidados.Select(item => new ConvidadoPublico(item.Id, item.Nome, item.Situacao)).ToArray()));
    }

    public async Task<Resultado<GrupoConvidados>> CriarGrupoAsync(string contaId, string? nome, CancellationToken ct)
    {
        var limpo = Domain.Comum.Texto.Limpo(nome);
        if (limpo is null)
        {
            return Resultado<GrupoConvidados>.Erro(Codigos.Validacao, "Informe o nome do grupo.");
        }

        var grupo = new GrupoConvidados { Id = Guid.NewGuid(), ContaId = contaId, Nome = limpo };
        await dados.InserirGrupoAsync(grupo, ct);
        return Resultado<GrupoConvidados>.Sucesso(grupo);
    }

    public Task<IReadOnlyList<GrupoConvidados>> ListarGruposAsync(string contaId, CancellationToken ct) =>
        dados.ListarGruposAsync(contaId, ct);

    private async Task<Resultado<Convite>> MontarAsync(string contaId, Evento evento, NovoConvite pedido, CancellationToken ct)
    {
        var nome = Domain.Comum.Texto.Limpo(pedido.Nome);
        if (nome is null)
        {
            return Resultado<Convite>.Erro(Codigos.Validacao, "Informe o nome do convite.");
        }

        if (pedido.Convidados is null || pedido.Convidados.Count == 0)
        {
            return Resultado<Convite>.Erro(Codigos.Validacao, "Inclua ao menos um convidado.");
        }

        if (pedido.GrupoId is Guid grupoId && await dados.ObterGrupoAsync(contaId, grupoId, ct) is null)
        {
            return Resultado<Convite>.Erro(Codigos.Validacao, "O grupo não pertence à conta.");
        }

        var transfer = ValidarTransfer(evento, pedido.UsaTransfer, pedido.LocalOrigem);
        if (!transfer.Ok)
        {
            return Resultado<Convite>.Erro(transfer.Falha!.Codigo, transfer.Falha.Mensagem);
        }

        var convite = new Convite
        {
            Id = Guid.NewGuid(),
            EventoId = evento.Id,
            ContaId = contaId,
            Nome = nome,
            GrupoId = pedido.GrupoId,
            Observacao = Domain.Comum.Texto.Limpo(pedido.Observacao),
            UsaTransfer = pedido.UsaTransfer && evento.TransferAtivo,
            LocalOrigem = pedido.UsaTransfer && evento.TransferAtivo ? transfer.Valor : null,
            Token = Guid.NewGuid().ToString("N"),
        };

        foreach (var pedidoConvidado in pedido.Convidados)
        {
            var convidado = MontarConvidado(pedidoConvidado);
            if (!convidado.Ok)
            {
                return Resultado<Convite>.Erro(convidado.Falha!.Codigo, convidado.Falha.Mensagem);
            }

            convidado.Valor!.Id = Guid.NewGuid();
            convidado.Valor.ConviteId = convite.Id;
            convite.Convidados.Add(convidado.Valor);
        }

        if (pedido.Principal is int indice)
        {
            if (indice < 0 || indice >= convite.Convidados.Count
                || !ConviteRegras.PodeSerPrincipal(true, convite.Convidados[indice].Celular))
            {
                return Resultado<Convite>.Erro(Codigos.Validacao, "Escolha um convidado deste convite que tenha telefone.");
            }

            convite.PrincipalId = convite.Convidados[indice].Id;
        }

        return Resultado<Convite>.Sucesso(convite);
    }

    private static Resultado<Convidado> MontarConvidado(ConvidadoPedido pedido)
    {
        var nome = Domain.Comum.Texto.Limpo(pedido.Nome);
        if (nome is null)
        {
            return Resultado<Convidado>.Erro(Codigos.Validacao, "Informe o nome do convidado.");
        }

        if (pedido.Situacao is SituacaoConvidado situacao && !Enum.IsDefined(situacao))
        {
            return Resultado<Convidado>.Erro(Codigos.Validacao, "A situação do convidado não é válida.");
        }

        if (!EnumOpcional(pedido.Genero) || !EnumOpcional(pedido.Faixa) || !EnumOpcional(pedido.Custo))
        {
            return Resultado<Convidado>.Erro(Codigos.Validacao, "Há um valor de convidado que não faz parte da lista.");
        }

        var cpf = Mascaras.Cpf(pedido.Cpf);
        if (cpf == "")
        {
            return Resultado<Convidado>.Erro(Codigos.Validacao, "O CPF deve seguir a máscara 000.000.000-00.");
        }

        return Resultado<Convidado>.Sucesso(new Convidado
        {
            Nome = nome,
            Ddi = Domain.Comum.Texto.Limpo(pedido.Ddi),
            Celular = Domain.Comum.Texto.Limpo(pedido.Celular),
            Situacao = pedido.Situacao ?? SituacaoConvidado.Pendente,
            Mesa = Domain.Comum.Texto.Limpo(pedido.Mesa),
            Genero = pedido.Genero,
            Faixa = pedido.Faixa,
            Custo = pedido.Custo,
            Rg = Domain.Comum.Texto.Limpo(pedido.Rg),
            Cpf = cpf,
            Observacao = Domain.Comum.Texto.Limpo(pedido.Observacao),
        });
    }

    private static Resultado<Convidado> ConvidadoDaPlanilha(LinhaPlanilha linha, string nome)
    {
        SituacaoConvidado situacao = SituacaoConvidado.Pendente;
        if (!Domain.Comum.Texto.Vazio(linha.Situacao))
        {
            var lida = Rotulos.Situacao(linha.Situacao);
            if (lida is null)
            {
                return Resultado<Convidado>.Erro(Codigos.Validacao, "A planilha tem uma situação que não faz parte da lista.");
            }

            situacao = lida.Value;
        }

        GeneroConvidado? genero = null;
        if (!Domain.Comum.Texto.Vazio(linha.Genero))
        {
            genero = Rotulos.Genero(linha.Genero);
            if (genero is null)
            {
                return Resultado<Convidado>.Erro(Codigos.Validacao, "A planilha tem um gênero que não faz parte da lista.");
            }
        }

        FaixaEtaria? faixa = null;
        if (!Domain.Comum.Texto.Vazio(linha.Faixa))
        {
            faixa = Rotulos.Faixa(linha.Faixa);
            if (faixa is null)
            {
                return Resultado<Convidado>.Erro(Codigos.Validacao, "A planilha tem uma faixa que não faz parte da lista.");
            }
        }

        CustoConvidado? custo = null;
        if (!Domain.Comum.Texto.Vazio(linha.Custo))
        {
            custo = Rotulos.Custo(linha.Custo);
            if (custo is null)
            {
                return Resultado<Convidado>.Erro(Codigos.Validacao, "A planilha tem um custo que não faz parte da lista.");
            }
        }

        return Resultado<Convidado>.Sucesso(new Convidado
        {
            Nome = nome,
            Ddi = Domain.Comum.Texto.Limpo(linha.Ddi),
            Celular = Domain.Comum.Texto.Limpo(linha.Telefone),
            Situacao = situacao,
            Genero = genero,
            Faixa = faixa,
            Custo = custo,
            Mesa = Domain.Comum.Texto.Limpo(linha.Mesa),
        });
    }

    private static Resultado<string?> ValidarTransfer(Evento evento, bool usa, string? local)
    {
        if (!usa || !evento.TransferAtivo)
        {
            return Resultado<string?>.Sucesso(null);
        }

        var origem = Domain.Comum.Texto.Limpo(local);
        if (origem is null || evento.LocaisTransfer.All(item => !string.Equals(item.Nome, origem, StringComparison.OrdinalIgnoreCase)))
        {
            return Resultado<string?>.Erro(Codigos.Validacao, "Escolha o local de origem do transfer.");
        }

        return Resultado<string?>.Sucesso(origem);
    }

    private static Convite? Filtrar(Convite convite, FiltroConvites filtro)
    {
        if (filtro.GrupoId is Guid grupo && convite.GrupoId != grupo)
        {
            return null;
        }

        if (filtro.Transfer is bool transfer && convite.UsaTransfer != transfer)
        {
            return null;
        }

        var convidados = convite.Convidados.Where(item => ConvidadoPassa(convite, item, filtro)).ToList();
        var observacaoConvite = Contem(convite.Observacao, filtro.Observacao) || Contem(convite.Nome, filtro.Nome);
        if (convidados.Count == 0 && !observacaoConvite)
        {
            return null;
        }

        if (TemFiltroDePessoa(filtro) && convidados.Count == 0)
        {
            return null;
        }

        convite.Convidados = convidados;
        return convite;
    }

    private static bool ConvidadoPassa(Convite convite, Convidado convidado, FiltroConvites filtro)
    {
        if (filtro.SomenteConfirmados && convidado.Situacao != SituacaoConvidado.Confirmado)
        {
            return false;
        }

        if (filtro.Situacao is SituacaoConvidado situacao && convidado.Situacao != situacao)
        {
            return false;
        }

        if (!Domain.Comum.Texto.Vazio(filtro.Telefone)
            && !Contem(convidado.Celular, filtro.Telefone)
            && !Contem(convite.Telefone(convidado), filtro.Telefone))
        {
            return false;
        }

        if (!Domain.Comum.Texto.Vazio(filtro.Nome)
            && !Contem(convidado.Nome, filtro.Nome)
            && !Contem(convite.Nome, filtro.Nome))
        {
            return false;
        }

        if (!Domain.Comum.Texto.Vazio(filtro.Observacao)
            && !Contem(convidado.Observacao, filtro.Observacao)
            && !Contem(convite.Observacao, filtro.Observacao))
        {
            return false;
        }

        return true;
    }

    private static bool TemFiltroDePessoa(FiltroConvites filtro) =>
        filtro.SomenteConfirmados || filtro.Situacao is not null
        || !Domain.Comum.Texto.Vazio(filtro.Telefone)
        || !Domain.Comum.Texto.Vazio(filtro.Nome)
        || !Domain.Comum.Texto.Vazio(filtro.Observacao);

    private static bool Contem(string? texto, string? termo)
    {
        if (Domain.Comum.Texto.Vazio(termo))
        {
            return false;
        }

        return texto?.Contains(termo!.Trim(), StringComparison.OrdinalIgnoreCase) == true;
    }

    private static void Copiar(Convidado destino, Convidado origem)
    {
        destino.Nome = origem.Nome;
        destino.Ddi = origem.Ddi;
        destino.Celular = origem.Celular;
        destino.Situacao = origem.Situacao;
        destino.Mesa = origem.Mesa;
        destino.Genero = origem.Genero;
        destino.Faixa = origem.Faixa;
        destino.Custo = origem.Custo;
        destino.Rg = origem.Rg;
        destino.Cpf = origem.Cpf;
        destino.Observacao = origem.Observacao;
    }

    private static ConviteLeitura Ler(Convite convite, IReadOnlyList<GrupoConvidados> grupos)
    {
        var principal = convite.Convidados.FirstOrDefault(item => item.Id == convite.PrincipalId);
        return new ConviteLeitura(
            convite.Id,
            convite.EventoId,
            convite.Nome,
            convite.GrupoId,
            grupos.FirstOrDefault(grupo => grupo.Id == convite.GrupoId)?.Nome,
            convite.Observacao,
            convite.UsaTransfer,
            convite.LocalOrigem,
            ConviteRegras.TemTelefone(principal?.Celular) ? $"{principal!.Ddi} {principal.Celular}".Trim() : null,
            convite.Token,
            convite.AcessadoEm is not null || convite.RespondidoEm is not null,
            convite.RespondidoEm is not null,
            convite.Convidados.Select(item => LerConvidado(item, convite.PrincipalId)).ToArray());
    }

    private static ConvidadoLeitura LerConvidado(Convidado convidado, Guid? principalId) => new(
        convidado.Id,
        convidado.Nome,
        convidado.Ddi,
        convidado.Celular,
        convidado.Situacao,
        Rotulos.SituacaoTela(convidado.Situacao),
        Rotulos.RecusaNoIcone(convidado.Situacao),
        convidado.Mesa,
        convidado.Genero,
        convidado.Faixa,
        convidado.Custo,
        convidado.Rg,
        convidado.Cpf,
        convidado.Observacao,
        convidado.Id == principalId);

    private static IReadOnlyList<TotaisLeitura> Contar<T>(IEnumerable<T> itens, Func<T, string> rotulo) =>
        itens.GroupBy(rotulo).Select(grupo => new TotaisLeitura(grupo.Key, grupo.Count())).ToArray();

    private static string RotuloFaixa(FaixaEtaria faixa) => faixa switch
    {
        FaixaEtaria.Adulto => "Adulto",
        FaixaEtaria.Adolescente => "Adolescente",
        FaixaEtaria.Crianca => "Criança",
        FaixaEtaria.CriancaDeColo => "Criança de colo",
        FaixaEtaria.Idoso => "Idoso",
        _ => "Não informado",
    };

    private static string RotuloCusto(CustoConvidado custo) => custo switch
    {
        CustoConvidado.Inteira => "Inteira",
        CustoConvidado.Meia => "Meia",
        CustoConvidado.Gratuita => "Gratuita",
        _ => "Não informado",
    };

    private static string Link(string basePublica, string token) =>
        $"{basePublica.TrimEnd('/')}/rsvp/{token}";

    private static bool EnumOpcional<T>(T? valor)
        where T : struct, Enum => valor is null || Enum.IsDefined(valor.Value);
}

file static class ConviteTelefone
{
    public static string? Telefone(this Convite convite, Convidado convidado) =>
        convite.PrincipalId == convidado.Id ? convidado.Celular : null;
}
