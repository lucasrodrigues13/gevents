using Project.Application.Cep;
using Project.Application.Comum;
using Project.Domain.Clientes;
using Project.Domain.Eventos;
using Project.Domain.Fornecedores;
using Project.Domain.Tarefas;

namespace Project.Application.Negocio;

public sealed record LocalPedido(
    string? Nome,
    string? Pais,
    string? Cep,
    string? Endereco,
    string? Numero,
    string? Estado,
    string? Cidade,
    string? Bairro);

public sealed record SalvarEvento(
    Guid? ModeloId,
    string? PrimeiroNome,
    string? SegundoNome,
    DateOnly? Data,
    TimeOnly? Hora,
    decimal? OrcamentoEstimado,
    int? ConvidadosEstimados,
    LocalPedido? Cerimonia,
    LocalPedido? Recepcao,
    IReadOnlyList<string>? Tags,
    bool? NotificarAssessor,
    bool? NotificarAssistentes,
    bool? NotificarClientes,
    bool? SalvarNaAgenda,
    TimeOnly? TempoFesta,
    TimeOnly? InicioCerimonia,
    TimeOnly? InicioRecepcao,
    int? QuantidadePadrinhos,
    int? QuantidadeDamas,
    string? SenhaSite,
    bool CadastrarCliente,
    NovoCliente? Cliente);

public sealed record AssessoriaPedido(
    decimal? ValorTotal,
    int? QuantidadeParcelas,
    DateOnly? PrimeiraParcela,
    bool ComEntrada,
    BaseEntrada? BaseEntrada,
    decimal? ValorEntrada,
    DateOnly? DataEntrada);

public sealed record ParcelaLeitura(int Numero, DateOnly Vencimento, decimal Valor, bool Entrada, SituacaoParcela Situacao);

public sealed record SubitemLeitura(Guid Id, string Descricao, int Quantidade, decimal ValorUnitario, decimal Total);

public sealed record FornecedorNoServicoLeitura(
    Guid Id,
    Guid FornecedorId,
    string Fornecedor,
    EtapaNegociacao Etapa,
    decimal? ValorOrcamento,
    string? Observacoes);

public sealed record ServicoLeitura(
    Guid Id,
    string Nome,
    bool Contratado,
    decimal? ValorEstimado,
    decimal? ValorContratado,
    bool OcultoParaClient,
    int Ordem,
    Guid? ServicoContaId,
    IReadOnlyList<ParcelaLeitura> Parcelas,
    IReadOnlyList<SubitemLeitura> Subitens,
    IReadOnlyList<FornecedorNoServicoLeitura> Fornecedores);

public sealed record LocalLeitura(
    string? Nome,
    string? Pais,
    string? Cep,
    string? Endereco,
    string? Numero,
    string? Estado,
    string? Cidade,
    string? Bairro);

public sealed record EventoDetalhe(
    Guid Id,
    string Nome,
    string? PrimeiroNome,
    string? SegundoNome,
    TipoEvento Tipo,
    SituacaoEvento Situacao,
    DateOnly? Data,
    TimeOnly? Hora,
    int? DiasRestantes,
    decimal? OrcamentoEstimado,
    int? ConvidadosEstimados,
    TimeOnly? TempoFesta,
    TimeOnly? InicioCerimonia,
    TimeOnly? InicioRecepcao,
    int? QuantidadePadrinhos,
    int? QuantidadeDamas,
    string? UrlSite,
    string? SenhaSite,
    bool NotificarAssessor,
    bool NotificarAssistentes,
    bool NotificarClientes,
    bool SalvarNaAgenda,
    string? VinculoGoogle,
    Guid? ImagemId,
    bool TransferAtivo,
    IReadOnlyList<string> LocaisTransfer,
    LocalLeitura Cerimonia,
    LocalLeitura Recepcao,
    IReadOnlyList<string> Tags,
    IReadOnlyList<ServicoLeitura> Servicos,
    int TarefasConcluidas,
    int Tarefas,
    int ConvidadosConfirmados,
    int ConvidadosPendentes,
    int ConvidadosRecusados,
    int Convidados);

public sealed record EventoResumo(
    Guid Id,
    string Nome,
    SituacaoEvento Situacao,
    TipoEvento Tipo,
    DateOnly? Data,
    int? DiasRestantes,
    IReadOnlyList<string> Tags,
    int TarefasConcluidas,
    int Tarefas,
    int ConvidadosConfirmados,
    int Convidados);

public sealed class EventoServico(IDados dados, IConsultaCep cep, ClienteServico clientes)
{
    public async Task<IReadOnlyList<EventoResumo>> ListarAsync(
        string contaId,
        FiltroEventos filtro,
        DateOnly hoje,
        CancellationToken ct)
    {
        var linhas = await dados.ListarEventosAsync(contaId, filtro, ct);
        return linhas.Select(linha => new EventoResumo(
            linha.Id,
            linha.Nome,
            linha.Situacao,
            linha.Tipo,
            linha.Data,
            Dias(linha.Data, hoje),
            linha.Tags,
            linha.TarefasConcluidas,
            linha.Tarefas,
            linha.ConvidadosConfirmados,
            linha.Convidados)).ToArray();
    }

    public async Task<Resultado<EventoDetalhe>> ObterAsync(string contaId, Guid id, DateOnly hoje, CancellationToken ct)
    {
        var evento = await dados.ObterEventoAsync(contaId, id, ct);
        if (evento is null)
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        return Resultado<EventoDetalhe>.Sucesso(await DetalheAsync(contaId, evento, hoje, ct));
    }

    public async Task<Resultado<EventoDetalhe>> CriarAsync(string contaId, SalvarEvento pedido, DateOnly hoje, CancellationToken ct)
    {
        if (pedido.ModeloId is not Guid modeloId)
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Validacao, "Escolha um modelo.");
        }

        var modelo = await dados.ObterModeloAsync(contaId, modeloId, ct);
        if (modelo is null)
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Validacao, "Escolha um modelo.");
        }

        var nome = NomeDoEvento.Montar(modelo.Tipo, pedido.PrimeiroNome, pedido.SegundoNome);
        if (nome is null)
        {
            var mensagem = modelo.Tipo == TipoEvento.Casamento
                ? "Informe os dois nomes do casamento."
                : "Informe o nome.";
            return Resultado<EventoDetalhe>.Erro(Codigos.Validacao, mensagem);
        }

        var numeros = ValidarNumeros(pedido);
        if (numeros is not null)
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Validacao, numeros);
        }

        var cerimonia = await LocalAsync(pedido.Cerimonia, ct);
        if (!cerimonia.Ok)
        {
            return Resultado<EventoDetalhe>.Erro(cerimonia.Falha!.Codigo, cerimonia.Falha.Mensagem);
        }

        var recepcao = await LocalAsync(pedido.Recepcao, ct);
        if (!recepcao.Ok)
        {
            return Resultado<EventoDetalhe>.Erro(recepcao.Falha!.Codigo, recepcao.Falha.Mensagem);
        }

        Cliente? cliente = null;
        if (pedido.CadastrarCliente)
        {
            var montado = ClienteServico.Montar(contaId, pedido.Cliente, hoje);
            if (!montado.Ok)
            {
                return Resultado<EventoDetalhe>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
            }

            cliente = montado.Valor;
            if (cliente!.Cep is not null)
            {
                var enderecoCliente = await cep.ConsultarAsync(Mascaras.SoDigitos(cliente.Cep)!, ct);
                if (enderecoCliente is not null)
                {
                    cliente.Pais = enderecoCliente.Pais;
                    cliente.Endereco = enderecoCliente.Endereco;
                    cliente.Estado = enderecoCliente.Estado;
                    cliente.Cidade = enderecoCliente.Cidade;
                    cliente.Bairro = enderecoCliente.Bairro;
                }
            }

            var acesso = await clientes.AplicarAcessoAsync(contaId, cliente, pedido.Cliente?.AcessoPlataforma == true, ct);
            if (!acesso.Ok)
            {
                return Resultado<EventoDetalhe>.Erro(acesso.Falha!.Codigo, acesso.Falha.Mensagem);
            }
        }

        var evento = new Evento
        {
            Id = Guid.NewGuid(),
            ContaId = contaId,
            ModeloId = modelo.Id,
            Nome = nome,
            PrimeiroNome = Domain.Comum.Texto.Limpo(pedido.PrimeiroNome),
            SegundoNome = Domain.Comum.Texto.Limpo(pedido.SegundoNome),
            Tipo = modelo.Tipo,
            Situacao = SituacaoEvento.EmAndamento,
            Data = pedido.Data,
            Hora = pedido.Hora,
            OrcamentoEstimado = pedido.OrcamentoEstimado,
            ConvidadosEstimados = pedido.ConvidadosEstimados,
            Cerimonia = cerimonia.Valor!,
            Recepcao = recepcao.Valor!,
            NotificarAssessor = pedido.NotificarAssessor ?? true,
            NotificarAssistentes = pedido.NotificarAssistentes ?? true,
            NotificarClientes = pedido.NotificarClientes ?? true,
            SalvarNaAgenda = pedido.SalvarNaAgenda ?? false,
            TempoFesta = pedido.TempoFesta,
            InicioCerimonia = pedido.InicioCerimonia,
            InicioRecepcao = pedido.InicioRecepcao,
            QuantidadePadrinhos = pedido.QuantidadePadrinhos,
            QuantidadeDamas = pedido.QuantidadeDamas,
        };

        var ordem = 0;
        foreach (var servico in modelo.Servicos)
        {
            evento.Servicos.Add(new ServicoEvento
            {
                Id = Guid.NewGuid(),
                EventoId = evento.Id,
                Nome = servico.Nome,
                ValorEstimado = servico.ValorEstimado,
                Ordem = ordem++,
            });
        }

        var tarefas = modelo.Tarefas.Select(tarefa =>
        {
            DateOnly? prazo = null;
            if (tarefa.DiasAntes is int dias && evento.Data is DateOnly data)
            {
                prazo = data.AddDays(-dias);
            }

            return new Tarefa
            {
                Id = Guid.NewGuid(),
                EventoId = evento.Id,
                ContaId = contaId,
                Titulo = tarefa.Titulo,
                Prazo = prazo,
                Situacao = SituacaoTarefa.Pendente,
                ResponsavelContaId = contaId,
            };
        }).ToArray();

        var agenda = evento.SalvarNaAgenda
            ? new CompromissoAgenda
            {
                Id = Guid.NewGuid(),
                ContaId = contaId,
                EventoId = evento.Id,
                Descricao = evento.Nome,
                Data = evento.Data,
                Hora = evento.Hora,
            }
            : null;

        if (cliente is not null)
        {
            cliente.Vinculos.Add(new VinculoCliente { ClienteId = cliente.Id, EventoId = evento.Id });
        }

        await dados.InserirEventoAsync(evento, tarefas, agenda, cliente, ct);
        await dados.SubstituirTagsAsync(evento, LimparTags(pedido.Tags), ct);
        return Resultado<EventoDetalhe>.Sucesso(await DetalheAsync(contaId, evento, hoje, ct));
    }

    public async Task<Resultado<EventoDetalhe>> AtualizarAsync(
        string contaId,
        Guid id,
        SalvarEvento pedido,
        DateOnly hoje,
        CancellationToken ct)
    {
        var evento = await dados.ObterEventoAsync(contaId, id, ct);
        if (evento is null)
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        if (evento.Situacao != SituacaoEvento.EmAndamento)
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Validacao, "O evento não está em andamento.");
        }

        var nome = NomeDoEvento.Montar(evento.Tipo, pedido.PrimeiroNome, pedido.SegundoNome);
        if (nome is null)
        {
            var mensagem = evento.Tipo == TipoEvento.Casamento
                ? "Informe os dois nomes do casamento."
                : "Informe o nome.";
            return Resultado<EventoDetalhe>.Erro(Codigos.Validacao, mensagem);
        }

        var numeros = ValidarNumeros(pedido);
        if (numeros is not null)
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Validacao, numeros);
        }

        var cerimonia = await LocalAsync(pedido.Cerimonia, ct);
        if (!cerimonia.Ok)
        {
            return Resultado<EventoDetalhe>.Erro(cerimonia.Falha!.Codigo, cerimonia.Falha.Mensagem);
        }

        var recepcao = await LocalAsync(pedido.Recepcao, ct);
        if (!recepcao.Ok)
        {
            return Resultado<EventoDetalhe>.Erro(recepcao.Falha!.Codigo, recepcao.Falha.Mensagem);
        }

        evento.Nome = nome;
        evento.PrimeiroNome = Domain.Comum.Texto.Limpo(pedido.PrimeiroNome);
        evento.SegundoNome = Domain.Comum.Texto.Limpo(pedido.SegundoNome);
        evento.Data = pedido.Data;
        evento.Hora = pedido.Hora;
        evento.OrcamentoEstimado = pedido.OrcamentoEstimado;
        evento.ConvidadosEstimados = pedido.ConvidadosEstimados;
        evento.Cerimonia ??= new LocalEvento();
        evento.Recepcao ??= new LocalEvento();
        CopiarLocal(evento.Cerimonia, cerimonia.Valor!);
        CopiarLocal(evento.Recepcao, recepcao.Valor!);
        evento.NotificarAssessor = pedido.NotificarAssessor ?? evento.NotificarAssessor;
        evento.NotificarAssistentes = pedido.NotificarAssistentes ?? evento.NotificarAssistentes;
        evento.NotificarClientes = pedido.NotificarClientes ?? evento.NotificarClientes;
        evento.SalvarNaAgenda = pedido.SalvarNaAgenda ?? evento.SalvarNaAgenda;
        evento.TempoFesta = pedido.TempoFesta;
        evento.InicioCerimonia = pedido.InicioCerimonia;
        evento.InicioRecepcao = pedido.InicioRecepcao;
        evento.QuantidadePadrinhos = pedido.QuantidadePadrinhos;
        evento.QuantidadeDamas = pedido.QuantidadeDamas;
        evento.SenhaSite = Domain.Comum.Texto.Limpo(pedido.SenhaSite);
        await dados.SubstituirTagsAsync(evento, LimparTags(pedido.Tags), ct);
        await dados.SincronizarAgendaAsync(evento, ct);
        await dados.SalvarEventoAsync(ct);
        return Resultado<EventoDetalhe>.Sucesso(await DetalheAsync(contaId, evento, hoje, ct));
    }

    public async Task<Resultado<EventoDetalhe>> EncerrarAsync(
        string contaId,
        Guid id,
        SituacaoEvento destino,
        DateOnly hoje,
        CancellationToken ct)
    {
        var evento = await dados.ObterEventoAsync(contaId, id, ct);
        if (evento is null)
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        if (evento.Situacao != SituacaoEvento.EmAndamento || destino == SituacaoEvento.EmAndamento)
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Validacao, "O evento não está em andamento.");
        }

        evento.Situacao = destino;
        await dados.SalvarEventoAsync(ct);
        return Resultado<EventoDetalhe>.Sucesso(await DetalheAsync(contaId, evento, hoje, ct));
    }

    public async Task<Resultado<EventoDetalhe>> DefinirAssessoriaAsync(
        string contaId,
        Guid id,
        AssessoriaPedido pedido,
        DateOnly hoje,
        CancellationToken ct)
    {
        var evento = await dados.ObterEventoAsync(contaId, id, ct);
        if (evento is null)
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        if (await dados.TemAssessoriaAsync(evento.Id, ct))
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Validacao, "O valor da assessoria já foi informado.");
        }

        if (pedido.ValorTotal is not decimal total || total <= 0
            || pedido.QuantidadeParcelas is not int quantidade || quantidade <= 0
            || pedido.PrimeiraParcela is not DateOnly primeira)
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Validacao, "Informe o valor total, a quantidade de parcelas e a data da primeira parcela.");
        }

        var parcelas = ParcelasDaAssessoria.Calcular(
            total,
            quantidade,
            primeira,
            pedido.ComEntrada,
            pedido.BaseEntrada,
            pedido.ValorEntrada,
            pedido.DataEntrada);
        if (parcelas.Count == 0)
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Validacao, "A entrada informada não fecha com o valor total.");
        }

        var servico = new ServicoEvento
        {
            Id = Guid.NewGuid(),
            EventoId = evento.Id,
            Nome = ParcelasDaAssessoria.NomeServico,
            FornecedorContaId = contaId,
            Contratado = true,
            ValorContratado = total,
            Parcelas = parcelas.Select(parcela => new ParcelaServico
            {
                Id = Guid.NewGuid(),
                Numero = parcela.Numero,
                Vencimento = parcela.Vencimento,
                Valor = parcela.Valor,
                Entrada = parcela.Entrada,
                Situacao = SituacaoParcela.Pendente,
            }).ToList(),
        };
        await dados.AdicionarServicoAsync(servico, ct);
        return await ObterAsync(contaId, id, hoje, ct);
    }

    public async Task<Resultado<EventoDetalhe>> DefinirTransferAsync(
        string contaId,
        Guid id,
        bool ativo,
        IReadOnlyList<string>? locais,
        DateOnly hoje,
        CancellationToken ct)
    {
        var evento = await dados.ObterEventoAsync(contaId, id, ct);
        if (evento is null)
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        evento.TransferAtivo = ativo;
        evento.LocaisTransfer.Clear();
        if (ativo)
        {
            foreach (var nome in (locais ?? []).Select(Domain.Comum.Texto.Limpo).OfType<string>().Distinct(StringComparer.OrdinalIgnoreCase))
            {
                evento.LocaisTransfer.Add(new LocalTransfer { Nome = nome });
            }
        }

        await dados.SalvarEventoAsync(ct);
        return Resultado<EventoDetalhe>.Sucesso(await DetalheAsync(contaId, evento, hoje, ct));
    }

    public async Task<Resultado<Guid?>> GuardarImagemAsync(string contaId, Guid id, Guid imagemId, CancellationToken ct)
    {
        var evento = await dados.ObterEventoAsync(contaId, id, ct);
        if (evento is null)
        {
            return Resultado<Guid?>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var anterior = evento.ImagemId;
        evento.ImagemId = imagemId;
        await dados.SalvarEventoAsync(ct);
        return Resultado<Guid?>.Sucesso(anterior);
    }

    private async Task<Resultado<LocalEvento>> LocalAsync(LocalPedido? pedido, CancellationToken ct)
    {
        var local = new LocalEvento
        {
            Nome = Domain.Comum.Texto.Limpo(pedido?.Nome),
            Pais = Domain.Comum.Texto.Limpo(pedido?.Pais),
            Numero = Domain.Comum.Texto.Limpo(pedido?.Numero),
            Endereco = Domain.Comum.Texto.Limpo(pedido?.Endereco),
            Estado = Domain.Comum.Texto.Limpo(pedido?.Estado),
            Cidade = Domain.Comum.Texto.Limpo(pedido?.Cidade),
            Bairro = Domain.Comum.Texto.Limpo(pedido?.Bairro),
        };
        if (Domain.Comum.Texto.Vazio(pedido?.Cep))
        {
            return Resultado<LocalEvento>.Sucesso(local);
        }

        var cepMascarado = Mascaras.Cep(pedido!.Cep);
        if (string.IsNullOrEmpty(cepMascarado))
        {
            return Resultado<LocalEvento>.Erro(Codigos.Validacao, "O CEP deve ter 8 dígitos.");
        }

        local.Cep = cepMascarado;
        var encontrado = await cep.ConsultarAsync(Mascaras.SoDigitos(cepMascarado)!, ct);
        if (encontrado is not null)
        {
            local.Pais = encontrado.Pais;
            local.Endereco = encontrado.Endereco;
            local.Estado = encontrado.Estado;
            local.Cidade = encontrado.Cidade;
            local.Bairro = encontrado.Bairro;
        }

        return Resultado<LocalEvento>.Sucesso(local);
    }

    private Task<EventoDetalhe> DetalheAsync(string contaId, Evento evento, DateOnly hoje, CancellationToken ct) =>
        EventoMapeamento.DetalheAsync(dados, contaId, evento, hoje, ct);

    private static void CopiarLocal(LocalEvento destino, LocalEvento origem)
    {
        destino.Nome = origem.Nome;
        destino.Pais = origem.Pais;
        destino.Cep = origem.Cep;
        destino.Endereco = origem.Endereco;
        destino.Numero = origem.Numero;
        destino.Estado = origem.Estado;
        destino.Cidade = origem.Cidade;
        destino.Bairro = origem.Bairro;
    }

    private static LocalLeitura LerLocal(LocalEvento local) => EventoMapeamento.LerLocal(local);

    private static int? Dias(DateOnly? data, DateOnly hoje) => EventoMapeamento.Dias(data, hoje);

    private static string? ValidarNumeros(SalvarEvento pedido)
    {
        if (pedido.OrcamentoEstimado is < 0 || pedido.ConvidadosEstimados is < 0
            || pedido.QuantidadePadrinhos is < 0 || pedido.QuantidadeDamas is < 0)
        {
            return "Use um número maior ou igual a zero.";
        }

        return null;
    }

    private static List<string> LimparTags(IReadOnlyList<string>? tags) =>
        (tags ?? []).Select(Domain.Comum.Texto.Limpo).OfType<string>()
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
}
