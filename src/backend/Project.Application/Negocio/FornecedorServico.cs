using Project.Application.Comum;
using Project.Domain.Eventos;
using Project.Domain.Fornecedores;

namespace Project.Application.Negocio;

public sealed record ServicoContaLeitura(Guid Id, string Nome, bool Padrao);

public sealed record NovoServicoConta(string? Nome);

public sealed record TelefoneFornecedorPedido(string? Responsavel, string? Ddi, string? Numero, bool? Movel);

public sealed record EmailFornecedorPedido(string? Endereco);

public sealed record OrcamentoFornecedorPedido(string? Titulo, string? Link, Guid? AnexoId, bool? VisivelClientes);

public sealed record SalvarFornecedor(
    string? Nome,
    Guid? FotoId,
    string? RazaoSocial,
    string? Cnpj,
    string? Cpf,
    string? NomeResponsavel,
    decimal? PrecoDe,
    decimal? PrecoAte,
    string? Site,
    string? Facebook,
    string? Instagram,
    string? Youtube,
    string? Pinterest,
    string? Pais,
    string? Cep,
    string? Endereco,
    string? Numero,
    string? Estado,
    string? Cidade,
    string? Bairro,
    string? Complemento,
    string? Banco,
    string? Agencia,
    string? ContaBancaria,
    string? TitularConta,
    string? TipoConta,
    string? DocumentosBancarios,
    string? PixTipo,
    string? PixChave,
    string? Diferencial,
    bool? DiferencialVisivelCliente,
    string? InformacoesGerais,
    string? Portfolio,
    IReadOnlyList<Guid>? ServicoContaIds,
    IReadOnlyList<TelefoneFornecedorPedido>? Telefones,
    IReadOnlyList<EmailFornecedorPedido>? Emails,
    IReadOnlyList<OrcamentoFornecedorPedido>? Orcamentos);

public sealed record TelefoneFornecedorLeitura(Guid Id, string? Responsavel, string? Ddi, string Numero, bool Movel);

public sealed record EmailFornecedorLeitura(Guid Id, string Endereco);

public sealed record OrcamentoFornecedorLeitura(Guid Id, string Titulo, string? Link, Guid? AnexoId, bool VisivelClientes);

public sealed record FornecedorResumo(Guid Id, string Nome, IReadOnlyList<string> Servicos, bool Completo);

public sealed record FornecedorLeitura(
    Guid Id,
    string Nome,
    Guid? FotoId,
    string? RazaoSocial,
    string? Cnpj,
    string? Cpf,
    string? NomeResponsavel,
    decimal? PrecoDe,
    decimal? PrecoAte,
    string? Site,
    string? Facebook,
    string? Instagram,
    string? Youtube,
    string? Pinterest,
    string? Pais,
    string? Cep,
    string? Endereco,
    string? Numero,
    string? Estado,
    string? Cidade,
    string? Bairro,
    string? Complemento,
    string? Banco,
    string? Agencia,
    string? ContaBancaria,
    string? TitularConta,
    string? TipoConta,
    string? DocumentosBancarios,
    string? PixTipo,
    string? PixChave,
    string? Diferencial,
    bool DiferencialVisivelCliente,
    string? InformacoesGerais,
    string? Portfolio,
    bool Completo,
    IReadOnlyList<Guid> ServicoContaIds,
    IReadOnlyList<string> Servicos,
    IReadOnlyList<TelefoneFornecedorLeitura> Telefones,
    IReadOnlyList<EmailFornecedorLeitura> Emails,
    IReadOnlyList<OrcamentoFornecedorLeitura> Orcamentos);

public sealed record AdicionarServicoEvento(string? Nome);

public sealed record AtualizarEstimadoServico(decimal? ValorEstimado, IReadOnlyList<SubitemPedido>? Subitens);

public sealed record SubitemPedido(string? Descricao, int? Quantidade, decimal? ValorUnitario);

public sealed record AdicionarFornecedorServico(Guid? FornecedorId, EtapaNegociacao? Etapa);

public sealed record ContratarFornecedorPedido(
    decimal? Valor,
    int? QuantidadeParcelas,
    DateOnly? PrimeiraParcela,
    bool ComEntrada,
    BaseEntrada? BaseEntrada,
    decimal? ValorEntrada,
    DateOnly? DataEntrada);

public sealed record BudgetServicoLinha(Guid Id, string Nome, decimal Estimado, decimal Contratado);

public sealed record BudgetEvento(
    decimal Budget,
    decimal Estimado,
    decimal Contratado,
    decimal ProvaReal,
    decimal? CustoPorConvidado,
    IReadOnlyList<BudgetServicoLinha> Servicos);

public sealed class FornecedorServico(IDados dados)
{
    public async Task<IReadOnlyList<ServicoContaLeitura>> ListarServicosAsync(string contaId, string? nome, CancellationToken ct)
    {
        var servicos = await dados.ListarServicosContaAsync(contaId, nome, ct);
        return servicos.Select(servico => new ServicoContaLeitura(servico.Id, servico.Nome, servico.Padrao)).ToArray();
    }

    public async Task<Resultado<ServicoContaLeitura>> CriarServicoAsync(string contaId, NovoServicoConta pedido, CancellationToken ct)
    {
        var nome = Domain.Comum.Texto.Limpo(pedido.Nome);
        if (nome is null)
        {
            return Resultado<ServicoContaLeitura>.Erro(Codigos.Validacao, "Informe o nome do serviço.");
        }

        if (nome.Length > 200)
        {
            return Resultado<ServicoContaLeitura>.Erro(Codigos.Validacao, "O nome do serviço pode ter no máximo 200 caracteres.");
        }

        var existente = await dados.ObterServicoContaPorNomeAsync(contaId, nome, ct);
        if (existente is not null)
        {
            return Resultado<ServicoContaLeitura>.Sucesso(new ServicoContaLeitura(existente.Id, existente.Nome, existente.Padrao));
        }

        var servico = new ServicoConta
        {
            Id = Guid.NewGuid(),
            ContaId = contaId,
            Nome = nome,
            Padrao = false,
        };
        await dados.InserirServicoContaAsync(servico, ct);
        return Resultado<ServicoContaLeitura>.Sucesso(new ServicoContaLeitura(servico.Id, servico.Nome, servico.Padrao));
    }

    public async Task<Resultado<bool>> ExcluirServicoAsync(string contaId, Guid id, CancellationToken ct)
    {
        var servico = await dados.ObterServicoContaAsync(contaId, id, ct);
        if (servico is null)
        {
            return Resultado<bool>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        await dados.RemoverServicoContaAsync(servico, ct);
        return Resultado<bool>.Sucesso(true);
    }

    public async Task<IReadOnlyList<FornecedorResumo>> ListarAsync(string contaId, string? nome, CancellationToken ct)
    {
        var fornecedores = await dados.ListarFornecedoresAsync(contaId, nome, ct);
        var nomes = await dados.NomesServicosContaAsync(
            contaId,
            fornecedores.SelectMany(item => item.Servicos.Select(ligacao => ligacao.ServicoContaId)),
            ct);
        return fornecedores.Select(fornecedor => new FornecedorResumo(
            fornecedor.Id,
            fornecedor.Nome,
            fornecedor.Servicos
                .Select(ligacao => nomes.GetValueOrDefault(ligacao.ServicoContaId, ""))
                .Where(texto => texto.Length > 0)
                .ToArray(),
            fornecedor.CadastroCompleto)).ToArray();
    }

    public async Task<Resultado<FornecedorLeitura>> ObterAsync(string contaId, Guid id, CancellationToken ct)
    {
        var fornecedor = await dados.ObterFornecedorAsync(contaId, id, ct);
        if (fornecedor is null)
        {
            return Resultado<FornecedorLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        return Resultado<FornecedorLeitura>.Sucesso(await LerAsync(contaId, fornecedor, ct));
    }

    public async Task<Resultado<FornecedorLeitura>> CriarAsync(string contaId, SalvarFornecedor pedido, CancellationToken ct)
    {
        var montado = await MontarAsync(contaId, Guid.NewGuid(), pedido, ct);
        if (!montado.Ok)
        {
            return Resultado<FornecedorLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        await dados.InserirFornecedorAsync(montado.Valor!, ct);
        return Resultado<FornecedorLeitura>.Sucesso(await LerAsync(contaId, montado.Valor!, ct));
    }

    public async Task<Resultado<FornecedorLeitura>> AtualizarAsync(string contaId, Guid id, SalvarFornecedor pedido, CancellationToken ct)
    {
        var fornecedor = await dados.ObterFornecedorAsync(contaId, id, ct);
        if (fornecedor is null)
        {
            return Resultado<FornecedorLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var montado = await MontarAsync(contaId, fornecedor.Id, pedido, ct);
        if (!montado.Ok)
        {
            return Resultado<FornecedorLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        Copiar(fornecedor, montado.Valor!);
        await dados.SalvarFornecedorAsync(ct);
        return Resultado<FornecedorLeitura>.Sucesso(await LerAsync(contaId, fornecedor, ct));
    }

    public async Task<Resultado<bool>> ExcluirAsync(string contaId, Guid id, CancellationToken ct)
    {
        var fornecedor = await dados.ObterFornecedorAsync(contaId, id, ct);
        if (fornecedor is null)
        {
            return Resultado<bool>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        await dados.RemoverFornecedorAsync(fornecedor, ct);
        return Resultado<bool>.Sucesso(true);
    }

    public async Task<Resultado<EventoDetalhe>> AdicionarServicoNoEventoAsync(
        string contaId,
        Guid eventoId,
        AdicionarServicoEvento pedido,
        DateOnly hoje,
        CancellationToken ct)
    {
        if (!await dados.EventoDaContaAsync(contaId, eventoId, ct))
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var nome = Domain.Comum.Texto.Limpo(pedido.Nome);
        if (nome is null)
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Validacao, "Informe o nome do serviço.");
        }

        var catalogo = await dados.ObterServicoContaPorNomeAsync(contaId, nome, ct);
        if (catalogo is null)
        {
            catalogo = new ServicoConta
            {
                Id = Guid.NewGuid(),
                ContaId = contaId,
                Nome = nome,
                Padrao = false,
            };
            await dados.InserirServicoContaAsync(catalogo, ct);
        }

        await dados.AdicionarServicoAsync(new ServicoEvento
        {
            Id = Guid.NewGuid(),
            EventoId = eventoId,
            Nome = catalogo.Nome,
            ServicoContaId = catalogo.Id,
            Ordem = DateTime.UtcNow.Millisecond,
        }, ct);
        return await DetalheEventoAsync(contaId, eventoId, hoje, ct);
    }

    public async Task<Resultado<EventoDetalhe>> AtualizarEstimadoAsync(
        string contaId,
        Guid eventoId,
        Guid servicoId,
        AtualizarEstimadoServico pedido,
        DateOnly hoje,
        CancellationToken ct)
    {
        var servico = await dados.ObterServicoEventoAsync(contaId, eventoId, servicoId, ct);
        if (servico is null)
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        if (pedido.Subitens is { Count: > 0 })
        {
            var subitens = new List<SubitemServico>();
            foreach (var item in pedido.Subitens)
            {
                var descricao = Domain.Comum.Texto.Limpo(item.Descricao);
                if (descricao is null || item.Quantidade is not int quantidade || quantidade <= 0
                    || item.ValorUnitario is not decimal unitario || unitario < 0)
                {
                    return Resultado<EventoDetalhe>.Erro(Codigos.Validacao, "Informe descrição, quantidade e valor unitário válidos.");
                }

                subitens.Add(new SubitemServico
                {
                    Id = Guid.NewGuid(),
                    ServicoId = servico.Id,
                    Descricao = descricao,
                    Quantidade = quantidade,
                    ValorUnitario = unitario,
                });
            }

            await dados.SubstituirSubitensAsync(servico.Id, subitens, BudgetCalculo.SomaSubitens(subitens), ct);
        }
        else
        {
            if (pedido.ValorEstimado is < 0)
            {
                return Resultado<EventoDetalhe>.Erro(Codigos.Validacao, "Use um número maior ou igual a zero.");
            }

            await dados.SubstituirSubitensAsync(servico.Id, [], pedido.ValorEstimado, ct);
        }

        return await DetalheEventoAsync(contaId, eventoId, hoje, ct);
    }

    public async Task<Resultado<EventoDetalhe>> AdicionarFornecedorNoServicoAsync(
        string contaId,
        Guid eventoId,
        Guid servicoId,
        AdicionarFornecedorServico pedido,
        DateOnly hoje,
        CancellationToken ct)
    {
        var servico = await dados.ObterServicoEventoAsync(contaId, eventoId, servicoId, ct);
        if (servico is null)
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        if (pedido.FornecedorId is not Guid fornecedorId)
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Validacao, "Escolha um fornecedor.");
        }

        var fornecedor = await dados.ObterFornecedorAsync(contaId, fornecedorId, ct);
        if (fornecedor is null)
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Validacao, "Escolha um fornecedor.");
        }

        var etapa = pedido.Etapa ?? EtapaNegociacao.Recomendado;
        if (!Enum.IsDefined(etapa))
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Validacao, "Informe a etapa da negociação.");
        }

        if (servico.Fornecedores.Any(item => item.FornecedorId == fornecedorId))
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Validacao, "Este fornecedor já está no serviço.");
        }

        await dados.InserirFornecedorNoServicoAsync(new FornecedorNoServico
        {
            Id = Guid.NewGuid(),
            ServicoId = servico.Id,
            FornecedorId = fornecedorId,
            Etapa = etapa,
        }, ct);

        if (etapa == EtapaNegociacao.Contratado)
        {
            await dados.MarcarServicoContratadoAsync(servico.Id, true, servico.ValorContratado, ct);
        }

        return await DetalheEventoAsync(contaId, eventoId, hoje, ct);
    }

    public async Task<Resultado<EventoDetalhe>> AvancarEtapaAsync(
        string contaId,
        Guid eventoId,
        Guid servicoId,
        Guid vinculoId,
        DateOnly hoje,
        CancellationToken ct) =>
        await MoverEtapaAsync(contaId, eventoId, servicoId, vinculoId, avancar: true, hoje, ct);

    public async Task<Resultado<EventoDetalhe>> VoltarEtapaAsync(
        string contaId,
        Guid eventoId,
        Guid servicoId,
        Guid vinculoId,
        DateOnly hoje,
        CancellationToken ct) =>
        await MoverEtapaAsync(contaId, eventoId, servicoId, vinculoId, avancar: false, hoje, ct);

    public async Task<Resultado<EventoDetalhe>> ContratarAsync(
        string contaId,
        Guid eventoId,
        Guid servicoId,
        Guid vinculoId,
        ContratarFornecedorPedido pedido,
        DateOnly hoje,
        CancellationToken ct)
    {
        var servico = await dados.ObterServicoEventoAsync(contaId, eventoId, servicoId, ct);
        var vinculo = servico?.Fornecedores.FirstOrDefault(item => item.Id == vinculoId);
        if (servico is null || vinculo is null)
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        if (pedido.Valor is not decimal valor || valor < 0
            || pedido.QuantidadeParcelas is not int quantidade || quantidade <= 0
            || pedido.PrimeiraParcela is not DateOnly primeira)
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Validacao, "Informe o valor, a quantidade de parcelas e a data da primeira parcela.");
        }

        var parcelas = ParcelasDaAssessoria.Calcular(
            valor,
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

        await dados.AtualizarEtapaNegociacaoAsync(vinculo.Id, EtapaNegociacao.Contratado, valor, ct);
        await dados.MarcarServicoContratadoAsync(servico.Id, true, valor, ct);
        await dados.SubstituirParcelasAsync(
            servico.Id,
            parcelas.Select(parcela => new ParcelaServico
            {
                Id = Guid.NewGuid(),
                ServicoId = servico.Id,
                Numero = parcela.Numero,
                Vencimento = parcela.Vencimento,
                Valor = parcela.Valor,
                Entrada = parcela.Entrada,
                Situacao = SituacaoParcela.Pendente,
            }).ToArray(),
            ct);
        return await DetalheEventoAsync(contaId, eventoId, hoje, ct);
    }

    public async Task<Resultado<BudgetEvento>> BudgetAsync(string contaId, Guid eventoId, CancellationToken ct)
    {
        var evento = await dados.ObterEventoAsync(contaId, eventoId, ct);
        if (evento is null)
        {
            return Resultado<BudgetEvento>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var (estimado, contratado, provaReal, custo) = BudgetCalculo.Resumo(evento);
        return Resultado<BudgetEvento>.Sucesso(new BudgetEvento(
            evento.OrcamentoEstimado ?? 0m,
            estimado,
            contratado,
            provaReal,
            custo,
            evento.Servicos
                .OrderBy(servico => servico.Ordem)
                .Select(servico => new BudgetServicoLinha(
                    servico.Id,
                    servico.Nome,
                    BudgetCalculo.EstimadoDoServico(servico),
                    BudgetCalculo.ContratadoDoServico(servico)))
                .ToArray()));
    }

    private async Task<Resultado<EventoDetalhe>> MoverEtapaAsync(
        string contaId,
        Guid eventoId,
        Guid servicoId,
        Guid vinculoId,
        bool avancar,
        DateOnly hoje,
        CancellationToken ct)
    {
        var servico = await dados.ObterServicoEventoAsync(contaId, eventoId, servicoId, ct);
        var vinculo = servico?.Fornecedores.FirstOrDefault(item => item.Id == vinculoId);
        if (servico is null || vinculo is null)
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var destino = avancar ? NegociacaoRegras.Avancar(vinculo.Etapa) : NegociacaoRegras.Voltar(vinculo.Etapa);
        if (destino is null)
        {
            return Resultado<EventoDetalhe>.Erro(
                Codigos.Validacao,
                avancar ? "Não é possível avançar esta etapa." : "Não é possível voltar esta etapa.");
        }

        await dados.AtualizarEtapaNegociacaoAsync(vinculo.Id, destino.Value, vinculo.ValorOrcamento, ct);
        if (destino == EtapaNegociacao.Contratado)
        {
            await dados.MarcarServicoContratadoAsync(servico.Id, true, servico.ValorContratado, ct);
        }
        else if (!servico.Fornecedores.Any(item => item.Id != vinculo.Id && item.Etapa == EtapaNegociacao.Contratado))
        {
            await dados.MarcarServicoContratadoAsync(servico.Id, false, servico.ValorContratado, ct);
        }

        return await DetalheEventoAsync(contaId, eventoId, hoje, ct);
    }

    private async Task<Resultado<EventoDetalhe>> DetalheEventoAsync(string contaId, Guid eventoId, DateOnly hoje, CancellationToken ct)
    {
        var evento = await dados.ObterEventoAsync(contaId, eventoId, ct);
        if (evento is null)
        {
            return Resultado<EventoDetalhe>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        return Resultado<EventoDetalhe>.Sucesso(await EventoMapeamento.DetalheAsync(dados, contaId, evento, hoje, ct));
    }

    private async Task<Resultado<Fornecedor>> MontarAsync(string contaId, Guid id, SalvarFornecedor pedido, CancellationToken ct)
    {
        var nome = Domain.Comum.Texto.Limpo(pedido.Nome);
        if (nome is null)
        {
            return Resultado<Fornecedor>.Erro(Codigos.Validacao, "Informe o nome do fornecedor.");
        }

        var servicoIds = (pedido.ServicoContaIds ?? []).Distinct().ToArray();
        if (servicoIds.Length == 0)
        {
            return Resultado<Fornecedor>.Erro(Codigos.Validacao, "Informe ao menos um serviço.");
        }

        var nomes = await dados.NomesServicosContaAsync(contaId, servicoIds, ct);
        if (nomes.Count != servicoIds.Length)
        {
            return Resultado<Fornecedor>.Erro(Codigos.Validacao, "Informe serviços válidos da conta.");
        }

        if (pedido.PrecoDe is < 0 || pedido.PrecoAte is < 0)
        {
            return Resultado<Fornecedor>.Erro(Codigos.Validacao, "Use um número maior ou igual a zero.");
        }

        var fornecedor = new Fornecedor
        {
            Id = id,
            ContaId = contaId,
            Nome = nome,
            FotoId = pedido.FotoId,
            RazaoSocial = Domain.Comum.Texto.Limpo(pedido.RazaoSocial),
            Cnpj = Mascaras.Cnpj(pedido.Cnpj) is { Length: > 0 } cnpj ? cnpj : Domain.Comum.Texto.Limpo(pedido.Cnpj),
            Cpf = Mascaras.Cpf(pedido.Cpf) is { Length: > 0 } cpf ? cpf : Domain.Comum.Texto.Limpo(pedido.Cpf),
            NomeResponsavel = Domain.Comum.Texto.Limpo(pedido.NomeResponsavel),
            PrecoDe = pedido.PrecoDe,
            PrecoAte = pedido.PrecoAte,
            Site = Domain.Comum.Texto.Limpo(pedido.Site),
            Facebook = Domain.Comum.Texto.Limpo(pedido.Facebook),
            Instagram = Domain.Comum.Texto.Limpo(pedido.Instagram),
            Youtube = Domain.Comum.Texto.Limpo(pedido.Youtube),
            Pinterest = Domain.Comum.Texto.Limpo(pedido.Pinterest),
            Pais = Domain.Comum.Texto.Limpo(pedido.Pais),
            Cep = Mascaras.Cep(pedido.Cep) is { Length: > 0 } cep ? cep : Domain.Comum.Texto.Limpo(pedido.Cep),
            Endereco = Domain.Comum.Texto.Limpo(pedido.Endereco),
            Numero = Domain.Comum.Texto.Limpo(pedido.Numero),
            Estado = Domain.Comum.Texto.Limpo(pedido.Estado),
            Cidade = Domain.Comum.Texto.Limpo(pedido.Cidade),
            Bairro = Domain.Comum.Texto.Limpo(pedido.Bairro),
            Complemento = Domain.Comum.Texto.Limpo(pedido.Complemento),
            Banco = Domain.Comum.Texto.Limpo(pedido.Banco),
            Agencia = Domain.Comum.Texto.Limpo(pedido.Agencia),
            ContaBancaria = Domain.Comum.Texto.Limpo(pedido.ContaBancaria),
            TitularConta = Domain.Comum.Texto.Limpo(pedido.TitularConta),
            TipoConta = Domain.Comum.Texto.Limpo(pedido.TipoConta),
            DocumentosBancarios = Domain.Comum.Texto.Limpo(pedido.DocumentosBancarios),
            PixTipo = Domain.Comum.Texto.Limpo(pedido.PixTipo),
            PixChave = Domain.Comum.Texto.Limpo(pedido.PixChave),
            Diferencial = Domain.Comum.Texto.Limpo(pedido.Diferencial),
            DiferencialVisivelCliente = pedido.DiferencialVisivelCliente ?? false,
            InformacoesGerais = Domain.Comum.Texto.Limpo(pedido.InformacoesGerais),
            Portfolio = Domain.Comum.Texto.Limpo(pedido.Portfolio),
        };

        foreach (var servicoId in servicoIds)
        {
            fornecedor.Servicos.Add(new Domain.Fornecedores.FornecedorServico { FornecedorId = id, ServicoContaId = servicoId });
        }

        foreach (var telefone in pedido.Telefones ?? [])
        {
            var numero = Domain.Comum.Texto.Limpo(telefone.Numero);
            if (numero is null)
            {
                continue;
            }

            fornecedor.Telefones.Add(new TelefoneFornecedor
            {
                Id = Guid.NewGuid(),
                FornecedorId = id,
                Responsavel = Domain.Comum.Texto.Limpo(telefone.Responsavel),
                Ddi = Domain.Comum.Texto.Limpo(telefone.Ddi),
                Numero = numero,
                Movel = telefone.Movel ?? true,
            });
        }

        foreach (var email in pedido.Emails ?? [])
        {
            var endereco = Domain.Comum.Texto.Limpo(email.Endereco);
            if (endereco is null)
            {
                continue;
            }

            if (!Domain.Comum.Texto.EmailValido(endereco))
            {
                return Resultado<Fornecedor>.Erro(Codigos.Validacao, "Informe um e-mail válido.");
            }

            fornecedor.Emails.Add(new EmailFornecedor
            {
                Id = Guid.NewGuid(),
                FornecedorId = id,
                Endereco = endereco,
            });
        }

        foreach (var orcamento in pedido.Orcamentos ?? [])
        {
            var titulo = Domain.Comum.Texto.Limpo(orcamento.Titulo);
            if (titulo is null)
            {
                continue;
            }

            fornecedor.Orcamentos.Add(new OrcamentoFornecedor
            {
                Id = Guid.NewGuid(),
                FornecedorId = id,
                Titulo = titulo,
                Link = Domain.Comum.Texto.Limpo(orcamento.Link),
                AnexoId = orcamento.AnexoId,
                VisivelClientes = orcamento.VisivelClientes ?? false,
            });
        }

        return Resultado<Fornecedor>.Sucesso(fornecedor);
    }

    private static void Copiar(Fornecedor destino, Fornecedor origem)
    {
        destino.Nome = origem.Nome;
        destino.FotoId = origem.FotoId;
        destino.RazaoSocial = origem.RazaoSocial;
        destino.Cnpj = origem.Cnpj;
        destino.Cpf = origem.Cpf;
        destino.NomeResponsavel = origem.NomeResponsavel;
        destino.PrecoDe = origem.PrecoDe;
        destino.PrecoAte = origem.PrecoAte;
        destino.Site = origem.Site;
        destino.Facebook = origem.Facebook;
        destino.Instagram = origem.Instagram;
        destino.Youtube = origem.Youtube;
        destino.Pinterest = origem.Pinterest;
        destino.Pais = origem.Pais;
        destino.Cep = origem.Cep;
        destino.Endereco = origem.Endereco;
        destino.Numero = origem.Numero;
        destino.Estado = origem.Estado;
        destino.Cidade = origem.Cidade;
        destino.Bairro = origem.Bairro;
        destino.Complemento = origem.Complemento;
        destino.Banco = origem.Banco;
        destino.Agencia = origem.Agencia;
        destino.ContaBancaria = origem.ContaBancaria;
        destino.TitularConta = origem.TitularConta;
        destino.TipoConta = origem.TipoConta;
        destino.DocumentosBancarios = origem.DocumentosBancarios;
        destino.PixTipo = origem.PixTipo;
        destino.PixChave = origem.PixChave;
        destino.Diferencial = origem.Diferencial;
        destino.DiferencialVisivelCliente = origem.DiferencialVisivelCliente;
        destino.InformacoesGerais = origem.InformacoesGerais;
        destino.Portfolio = origem.Portfolio;
        destino.Telefones.Clear();
        foreach (var telefone in origem.Telefones)
        {
            destino.Telefones.Add(telefone);
        }

        destino.Emails.Clear();
        foreach (var email in origem.Emails)
        {
            destino.Emails.Add(email);
        }

        destino.Orcamentos.Clear();
        foreach (var orcamento in origem.Orcamentos)
        {
            destino.Orcamentos.Add(orcamento);
        }

        destino.Servicos.Clear();
        foreach (var servico in origem.Servicos)
        {
            destino.Servicos.Add(servico);
        }
    }

    private async Task<FornecedorLeitura> LerAsync(string contaId, Fornecedor fornecedor, CancellationToken ct)
    {
        var nomes = await dados.NomesServicosContaAsync(
            contaId,
            fornecedor.Servicos.Select(item => item.ServicoContaId),
            ct);
        return new FornecedorLeitura(
            fornecedor.Id,
            fornecedor.Nome,
            fornecedor.FotoId,
            fornecedor.RazaoSocial,
            fornecedor.Cnpj,
            fornecedor.Cpf,
            fornecedor.NomeResponsavel,
            fornecedor.PrecoDe,
            fornecedor.PrecoAte,
            fornecedor.Site,
            fornecedor.Facebook,
            fornecedor.Instagram,
            fornecedor.Youtube,
            fornecedor.Pinterest,
            fornecedor.Pais,
            fornecedor.Cep,
            fornecedor.Endereco,
            fornecedor.Numero,
            fornecedor.Estado,
            fornecedor.Cidade,
            fornecedor.Bairro,
            fornecedor.Complemento,
            fornecedor.Banco,
            fornecedor.Agencia,
            fornecedor.ContaBancaria,
            fornecedor.TitularConta,
            fornecedor.TipoConta,
            fornecedor.DocumentosBancarios,
            fornecedor.PixTipo,
            fornecedor.PixChave,
            fornecedor.Diferencial,
            fornecedor.DiferencialVisivelCliente,
            fornecedor.InformacoesGerais,
            fornecedor.Portfolio,
            fornecedor.CadastroCompleto,
            fornecedor.Servicos.Select(item => item.ServicoContaId).ToArray(),
            fornecedor.Servicos
                .Select(item => nomes.GetValueOrDefault(item.ServicoContaId, ""))
                .Where(texto => texto.Length > 0)
                .ToArray(),
            fornecedor.Telefones.Select(item => new TelefoneFornecedorLeitura(
                item.Id, item.Responsavel, item.Ddi, item.Numero, item.Movel)).ToArray(),
            fornecedor.Emails.Select(item => new EmailFornecedorLeitura(item.Id, item.Endereco)).ToArray(),
            fornecedor.Orcamentos.Select(item => new OrcamentoFornecedorLeitura(
                item.Id, item.Titulo, item.Link, item.AnexoId, item.VisivelClientes)).ToArray());
    }
}
