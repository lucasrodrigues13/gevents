using Project.Application.Comum;
using Project.Domain.Eventos;
using Project.Domain.Funil;

namespace Project.Application.Negocio;

public sealed record ContatoOportunidadeLeitura(Guid Id, string Nome, string? Telefone, string? Email);

public sealed record OportunidadeLeitura(
    Guid Id,
    string Nome,
    string Estagio,
    string? Fonte,
    decimal? ValorProposta,
    string Responsavel,
    string? ServicoProduto,
    Guid? ModeloServicoFunilId,
    string? TipoEvento,
    int? QuantidadeConvidados,
    DateOnly? DataPrevista,
    string? Horarios,
    int? QuantidadePadrinhos,
    int? QuantidadeDamas,
    string? LocalCerimonia,
    string? EnderecoCerimonia,
    string? LocalRecepcao,
    string? EnderecoRecepcao,
    string? Uf,
    string? Cidade,
    string? MotivoDescarte,
    DateTimeOffset CriadoEm,
    DateTimeOffset EntradaNoEstagioEm,
    IReadOnlyList<ContatoOportunidadeLeitura> Contatos);

public sealed record ContatoOportunidadePedido(string? Nome, string? Telefone, string? Email);

public sealed record SalvarOportunidade(
    string? Nome,
    string? Estagio,
    string? Fonte,
    decimal? ValorProposta,
    string? Responsavel,
    string? ServicoProduto,
    Guid? ModeloServicoFunilId,
    string? TipoEvento,
    int? QuantidadeConvidados,
    DateOnly? DataPrevista,
    string? Horarios,
    int? QuantidadePadrinhos,
    int? QuantidadeDamas,
    string? LocalCerimonia,
    string? EnderecoCerimonia,
    string? LocalRecepcao,
    string? EnderecoRecepcao,
    string? Uf,
    string? Cidade,
    string? MotivoDescarte,
    IReadOnlyList<ContatoOportunidadePedido>? Contatos);

public sealed record MoverOportunidade(string? Estagio, string? MotivoDescarte);

public sealed record FiltroOportunidades(
    string? Nome,
    string? Responsavel,
    string? TipoEvento,
    DateOnly? DataDe,
    DateOnly? DataAte,
    string? Ordenacao);

public sealed record LeadPublicoPedido(
    string? Nome,
    string? ContatoNome,
    string? Telefone,
    string? Email,
    string? Fonte);

public sealed record ChecagemDataLeitura(bool TemCompromisso);

public sealed class OportunidadeServico(IDados dados)
{
    public async Task<IReadOnlyList<OportunidadeLeitura>> ListarAsync(
        string contaId,
        FiltroOportunidades filtro,
        CancellationToken ct)
    {
        var itens = await dados.ListarOportunidadesAsync(contaId, filtro, ct);
        return itens.Select(Ler).ToArray();
    }

    public async Task<Resultado<OportunidadeLeitura>> ObterAsync(string contaId, Guid id, CancellationToken ct)
    {
        var item = await dados.ObterOportunidadeAsync(contaId, id, ct);
        return item is null
            ? Resultado<OportunidadeLeitura>.Erro(Codigos.Indisponivel, "Indisponível.")
            : Resultado<OportunidadeLeitura>.Sucesso(Ler(item));
    }

    public async Task<Resultado<OportunidadeLeitura>> CriarAsync(string contaId, SalvarOportunidade pedido, CancellationToken ct)
    {
        var montado = await MontarAsync(contaId, Guid.NewGuid(), pedido, null, ct);
        if (!montado.Ok)
        {
            return Resultado<OportunidadeLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        var agora = DateTimeOffset.UtcNow;
        montado.Valor!.CriadoEm = agora;
        montado.Valor.EntradaNoEstagioEm = agora;
        await dados.InserirOportunidadeAsync(montado.Valor, ct);
        return Resultado<OportunidadeLeitura>.Sucesso(Ler(montado.Valor));
    }

    public async Task<Resultado<OportunidadeLeitura>> AtualizarAsync(
        string contaId,
        Guid id,
        SalvarOportunidade pedido,
        CancellationToken ct)
    {
        var item = await dados.ObterOportunidadeAsync(contaId, id, ct);
        if (item is null)
        {
            return Resultado<OportunidadeLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var montado = await MontarAsync(contaId, item.Id, pedido, item, ct);
        if (!montado.Ok)
        {
            return Resultado<OportunidadeLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        var novo = montado.Valor!;
        if (novo.Estagio != item.Estagio)
        {
            item.EntradaNoEstagioEm = DateTimeOffset.UtcNow;
        }

        item.Nome = novo.Nome;
        item.Estagio = novo.Estagio;
        item.Fonte = novo.Fonte;
        item.ValorProposta = novo.ValorProposta;
        item.Responsavel = novo.Responsavel;
        item.ServicoProduto = novo.ServicoProduto;
        item.ModeloServicoFunilId = novo.ModeloServicoFunilId;
        item.TipoEvento = novo.TipoEvento;
        item.QuantidadeConvidados = novo.QuantidadeConvidados;
        item.DataPrevista = novo.DataPrevista;
        item.Horarios = novo.Horarios;
        item.QuantidadePadrinhos = novo.QuantidadePadrinhos;
        item.QuantidadeDamas = novo.QuantidadeDamas;
        item.LocalCerimonia = novo.LocalCerimonia;
        item.EnderecoCerimonia = novo.EnderecoCerimonia;
        item.LocalRecepcao = novo.LocalRecepcao;
        item.EnderecoRecepcao = novo.EnderecoRecepcao;
        item.Uf = novo.Uf;
        item.Cidade = novo.Cidade;
        item.MotivoDescarte = novo.MotivoDescarte;
        item.Contatos.Clear();
        foreach (var contato in novo.Contatos)
        {
            item.Contatos.Add(contato);
        }

        await dados.SalvarNegocioAsync(ct);
        return Resultado<OportunidadeLeitura>.Sucesso(Ler(item));
    }

    public async Task<Resultado<OportunidadeLeitura>> MoverAsync(
        string contaId,
        Guid id,
        MoverOportunidade pedido,
        CancellationToken ct)
    {
        var item = await dados.ObterOportunidadeAsync(contaId, id, ct);
        if (item is null)
        {
            return Resultado<OportunidadeLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        if (!TryParseEstagio(pedido.Estagio, out var estagio))
        {
            return Resultado<OportunidadeLeitura>.Erro(Codigos.Validacao, "Informe o estágio.");
        }

        if (estagio == EstagioOportunidade.Descartado)
        {
            var motivo = Domain.Comum.Texto.Limpo(pedido.MotivoDescarte);
            if (motivo is null)
            {
                return Resultado<OportunidadeLeitura>.Erro(Codigos.Validacao, "Informe o motivo do descarte.");
            }

            item.MotivoDescarte = motivo;
        }
        else
        {
            item.MotivoDescarte = null;
        }

        if (item.Estagio != estagio)
        {
            item.Estagio = estagio;
            item.EntradaNoEstagioEm = DateTimeOffset.UtcNow;
        }

        await dados.SalvarNegocioAsync(ct);
        return Resultado<OportunidadeLeitura>.Sucesso(Ler(item));
    }

    public async Task<Resultado<bool>> ExcluirAsync(string contaId, Guid id, CancellationToken ct)
    {
        var item = await dados.ObterOportunidadeAsync(contaId, id, ct);
        if (item is null)
        {
            return Resultado<bool>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        await dados.RemoverOportunidadeAsync(item, ct);
        return Resultado<bool>.Sucesso(true);
    }

    public async Task<Resultado<OportunidadeLeitura>> CriarLeadPublicoAsync(
        string contaId,
        LeadPublicoPedido pedido,
        CancellationToken ct)
    {
        var nome = Domain.Comum.Texto.Limpo(pedido.Nome) ?? Domain.Comum.Texto.Limpo(pedido.ContatoNome);
        var contatoNome = Domain.Comum.Texto.Limpo(pedido.ContatoNome) ?? nome;
        if (nome is null || contatoNome is null)
        {
            return Resultado<OportunidadeLeitura>.Erro(Codigos.Validacao, "Informe o nome e o contato.");
        }

        var agora = DateTimeOffset.UtcNow;
        var item = new Oportunidade
        {
            Id = Guid.NewGuid(),
            ContaId = contaId,
            Nome = nome,
            Estagio = EstagioOportunidade.ContatoInicial,
            Fonte = Domain.Comum.Texto.Limpo(pedido.Fonte) ?? "Formulário público",
            Responsavel = contaId,
            CriadoEm = agora,
            EntradaNoEstagioEm = agora,
            Contatos =
            [
                new ContatoOportunidade
                {
                    Id = Guid.NewGuid(),
                    Nome = contatoNome,
                    Telefone = Domain.Comum.Texto.Limpo(pedido.Telefone),
                    Email = Domain.Comum.Texto.Limpo(pedido.Email),
                },
            ],
        };
        item.Contatos[0].OportunidadeId = item.Id;
        await dados.InserirOportunidadeAsync(item, ct);
        return Resultado<OportunidadeLeitura>.Sucesso(Ler(item));
    }

    public async Task<ChecagemDataLeitura> ChecarDataAsync(string contaId, DateOnly data, CancellationToken ct) =>
        new(await dados.TemCompromissoNaDataAsync(contaId, data, ct));

    public async Task<IReadOnlyList<FunilEstagio>> ContarPorEstagioAsync(string contaId, CancellationToken ct)
    {
        var contagens = await dados.ContarOportunidadesPorEstagioAsync(contaId, ct);
        return RotulosEstagio.Todos.Select(estagio =>
        {
            var item = contagens.FirstOrDefault(c => c.Estagio == estagio);
            return new FunilEstagio(RotulosEstagio.Texto(estagio), item?.Quantidade ?? 0, item?.Valor ?? 0);
        }).ToArray();
    }

    private async Task<Resultado<Oportunidade>> MontarAsync(
        string contaId,
        Guid id,
        SalvarOportunidade pedido,
        Oportunidade? existente,
        CancellationToken ct)
    {
        var nome = Domain.Comum.Texto.Limpo(pedido.Nome);
        if (nome is null)
        {
            return Resultado<Oportunidade>.Erro(Codigos.Validacao, "Informe o nome da oportunidade.");
        }

        if (!TryParseEstagio(pedido.Estagio, out var estagio))
        {
            return Resultado<Oportunidade>.Erro(Codigos.Validacao, "Informe o estágio.");
        }

        var contatos = (pedido.Contatos ?? [])
            .Select(contato => new ContatoOportunidade
            {
                Id = Guid.NewGuid(),
                OportunidadeId = id,
                Nome = Domain.Comum.Texto.Limpo(contato.Nome) ?? "",
                Telefone = Domain.Comum.Texto.Limpo(contato.Telefone),
                Email = Domain.Comum.Texto.Limpo(contato.Email),
            })
            .Where(contato => !string.IsNullOrWhiteSpace(contato.Nome))
            .ToList();
        if (contatos.Count == 0)
        {
            return Resultado<Oportunidade>.Erro(Codigos.Validacao, "Informe ao menos um contato.");
        }

        if (pedido.ValorProposta is < 0)
        {
            return Resultado<Oportunidade>.Erro(Codigos.Validacao, "O valor não pode ser negativo.");
        }

        if (estagio == EstagioOportunidade.Descartado)
        {
            var motivo = Domain.Comum.Texto.Limpo(pedido.MotivoDescarte);
            if (motivo is null)
            {
                return Resultado<Oportunidade>.Erro(Codigos.Validacao, "Informe o motivo do descarte.");
            }
        }

        string? servicoProduto = Domain.Comum.Texto.Limpo(pedido.ServicoProduto);
        Guid? modeloServicoId = pedido.ModeloServicoFunilId;
        if (modeloServicoId is Guid modeloId)
        {
            var modelo = await dados.ObterModeloServicoFunilAsync(contaId, modeloId, ct);
            if (modelo is null)
            {
                return Resultado<Oportunidade>.Erro(Codigos.Validacao, "Serviço/produto inválido.");
            }

            servicoProduto ??= modelo.Nome;
        }

        TipoEvento? tipo = null;
        if (!string.IsNullOrWhiteSpace(pedido.TipoEvento))
        {
            if (!Enum.TryParse<TipoEvento>(pedido.TipoEvento, true, out var lido) || !Enum.IsDefined(lido))
            {
                return Resultado<Oportunidade>.Erro(Codigos.Validacao, "Tipo de evento inválido.");
            }

            tipo = lido;
        }

        return Resultado<Oportunidade>.Sucesso(new Oportunidade
        {
            Id = id,
            ContaId = contaId,
            Nome = nome,
            Estagio = estagio,
            Fonte = Domain.Comum.Texto.Limpo(pedido.Fonte),
            ValorProposta = pedido.ValorProposta,
            Responsavel = Domain.Comum.Texto.Limpo(pedido.Responsavel) ?? contaId,
            ServicoProduto = servicoProduto,
            ModeloServicoFunilId = modeloServicoId,
            TipoEvento = tipo,
            QuantidadeConvidados = pedido.QuantidadeConvidados,
            DataPrevista = pedido.DataPrevista,
            Horarios = Domain.Comum.Texto.Limpo(pedido.Horarios),
            QuantidadePadrinhos = pedido.QuantidadePadrinhos,
            QuantidadeDamas = pedido.QuantidadeDamas,
            LocalCerimonia = Domain.Comum.Texto.Limpo(pedido.LocalCerimonia),
            EnderecoCerimonia = Domain.Comum.Texto.Limpo(pedido.EnderecoCerimonia),
            LocalRecepcao = Domain.Comum.Texto.Limpo(pedido.LocalRecepcao),
            EnderecoRecepcao = Domain.Comum.Texto.Limpo(pedido.EnderecoRecepcao),
            Uf = Domain.Comum.Texto.Limpo(pedido.Uf),
            Cidade = Domain.Comum.Texto.Limpo(pedido.Cidade),
            MotivoDescarte = estagio == EstagioOportunidade.Descartado
                ? Domain.Comum.Texto.Limpo(pedido.MotivoDescarte)
                : null,
            CriadoEm = existente?.CriadoEm ?? DateTimeOffset.UtcNow,
            EntradaNoEstagioEm = existente?.EntradaNoEstagioEm ?? DateTimeOffset.UtcNow,
            Contatos = contatos,
        });
    }

    private static bool TryParseEstagio(string? valor, out EstagioOportunidade estagio)
    {
        estagio = default;
        if (string.IsNullOrWhiteSpace(valor))
        {
            return false;
        }

        if (Enum.TryParse(valor, true, out estagio) && Enum.IsDefined(estagio))
        {
            return true;
        }

        foreach (var item in RotulosEstagio.Todos)
        {
            if (string.Equals(RotulosEstagio.Texto(item), valor.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                estagio = item;
                return true;
            }
        }

        return false;
    }

    internal static OportunidadeLeitura Ler(Oportunidade item) =>
        new(
            item.Id,
            item.Nome,
            RotulosEstagio.Texto(item.Estagio),
            item.Fonte,
            item.ValorProposta,
            item.Responsavel,
            item.ServicoProduto,
            item.ModeloServicoFunilId,
            item.TipoEvento?.ToString(),
            item.QuantidadeConvidados,
            item.DataPrevista,
            item.Horarios,
            item.QuantidadePadrinhos,
            item.QuantidadeDamas,
            item.LocalCerimonia,
            item.EnderecoCerimonia,
            item.LocalRecepcao,
            item.EnderecoRecepcao,
            item.Uf,
            item.Cidade,
            item.MotivoDescarte,
            item.CriadoEm,
            item.EntradaNoEstagioEm,
            item.Contatos.Select(c => new ContatoOportunidadeLeitura(c.Id, c.Nome, c.Telefone, c.Email)).ToArray());
}
