using Project.Application.Comum;
using Project.Domain.Eventos;

namespace Project.Application.Negocio;

public sealed record ItemServicoModeloLeitura(Guid Id, TipoItemServicoModelo Tipo, string Texto);

public sealed record ServicoModeloLeitura(Guid Id, string Nome, decimal? ValorEstimado, IReadOnlyList<ItemServicoModeloLeitura> Itens);

public sealed record TarefaModeloLeitura(Guid Id, string Titulo, int? DiasAntes);

public sealed record VinculoModeloLeitura(Guid Id, TipoVinculoModelo Tipo, Guid BibliotecaId);

public sealed record PermissoesModeloLeitura(
    bool Gerais,
    bool Planejamento,
    bool Tarefas,
    bool ServicosFornecedores,
    bool Convites);

public sealed record ModeloLeitura(
    Guid Id,
    string Nome,
    TipoEvento Tipo,
    IReadOnlyList<ServicoModeloLeitura> Servicos,
    IReadOnlyList<TarefaModeloLeitura> Tarefas,
    PermissoesModeloLeitura Permissoes,
    IReadOnlyList<VinculoModeloLeitura> Vinculos);

public sealed record NovoModelo(string? Nome, TipoEvento? Tipo, IReadOnlyList<string>? Servicos, IReadOnlyList<string>? Tarefas);

public sealed record ItemServicoModeloPedido(TipoItemServicoModelo? Tipo, string? Texto);

public sealed record ServicoModeloPedido(string? Nome, decimal? ValorEstimado, IReadOnlyList<ItemServicoModeloPedido>? Itens);

public sealed record TarefaModeloPedido(string? Titulo, int? DiasAntes);

public sealed record VinculoModeloPedido(TipoVinculoModelo? Tipo, Guid? BibliotecaId);

public sealed record PermissoesModeloPedido(
    bool? Gerais,
    bool? Planejamento,
    bool? Tarefas,
    bool? ServicosFornecedores,
    bool? Convites);

public sealed record AtualizarModelo(
    string? Nome,
    TipoEvento? Tipo,
    PermissoesModeloPedido? Permissoes,
    IReadOnlyList<ServicoModeloPedido>? Servicos,
    IReadOnlyList<TarefaModeloPedido>? Tarefas,
    IReadOnlyList<VinculoModeloPedido>? Vinculos);

public sealed class ModeloServico(IDados dados)
{
    public async Task<IReadOnlyList<ModeloLeitura>> ListarAsync(string contaId, string? nome, CancellationToken ct)
    {
        var modelos = await dados.ListarModelosAsync(contaId, nome, ct);
        return modelos.Select(Ler).ToArray();
    }

    public async Task<Resultado<ModeloLeitura>> ObterAsync(string contaId, Guid id, CancellationToken ct)
    {
        var modelo = await dados.ObterModeloAsync(contaId, id, ct);
        return modelo is null
            ? Resultado<ModeloLeitura>.Erro(Codigos.Indisponivel, "Indisponível.")
            : Resultado<ModeloLeitura>.Sucesso(Ler(modelo));
    }

    public async Task<Resultado<ModeloLeitura>> CriarAsync(string contaId, NovoModelo pedido, CancellationToken ct)
    {
        var nome = Domain.Comum.Texto.Limpo(pedido.Nome);
        if (nome is null)
        {
            return Resultado<ModeloLeitura>.Erro(Codigos.Validacao, "Informe o nome do modelo.");
        }

        if (pedido.Tipo is not TipoEvento tipo || !Enum.IsDefined(tipo))
        {
            return Resultado<ModeloLeitura>.Erro(Codigos.Validacao, "Informe o tipo do modelo.");
        }

        var modelo = new ModeloEvento
        {
            Id = Guid.NewGuid(),
            ContaId = contaId,
            Nome = nome,
            Tipo = tipo,
        };
        foreach (var servico in Nomes(pedido.Servicos))
        {
            modelo.Servicos.Add(new ServicoModelo { Id = Guid.NewGuid(), ModeloId = modelo.Id, Nome = servico });
        }

        foreach (var tarefa in Nomes(pedido.Tarefas))
        {
            modelo.Tarefas.Add(new TarefaModelo { Id = Guid.NewGuid(), ModeloId = modelo.Id, Titulo = tarefa });
        }

        await dados.InserirModeloAsync(modelo, ct);
        return Resultado<ModeloLeitura>.Sucesso(Ler(modelo));
    }

    public async Task<Resultado<ModeloLeitura>> AtualizarAsync(string contaId, Guid id, AtualizarModelo pedido, CancellationToken ct)
    {
        var modelo = await dados.ObterModeloAsync(contaId, id, ct);
        if (modelo is null)
        {
            return Resultado<ModeloLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var nome = Domain.Comum.Texto.Limpo(pedido.Nome);
        if (nome is null)
        {
            return Resultado<ModeloLeitura>.Erro(Codigos.Validacao, "Informe o nome do modelo.");
        }

        if (pedido.Tipo is not TipoEvento tipo || !Enum.IsDefined(tipo))
        {
            return Resultado<ModeloLeitura>.Erro(Codigos.Validacao, "Informe o tipo do modelo.");
        }

        var validacao = ValidarConteudo(pedido);
        if (validacao is not null)
        {
            return Resultado<ModeloLeitura>.Erro(Codigos.Validacao, validacao);
        }

        var modeloId = modelo.Id;
        var atualizado = new ModeloEvento
        {
            Id = modeloId,
            ContaId = contaId,
            Nome = nome,
            Tipo = tipo,
            PermiteGerais = pedido.Permissoes?.Gerais ?? modelo.PermiteGerais,
            PermitePlanejamento = pedido.Permissoes?.Planejamento ?? modelo.PermitePlanejamento,
            PermiteTarefas = pedido.Permissoes?.Tarefas ?? modelo.PermiteTarefas,
            PermiteServicosFornecedores = pedido.Permissoes?.ServicosFornecedores ?? modelo.PermiteServicosFornecedores,
            PermiteConvites = pedido.Permissoes?.Convites ?? modelo.PermiteConvites,
        };

        await dados.RemoverConteudoModeloAsync(modelo, ct);

        foreach (var servico in pedido.Servicos ?? [])
        {
            var servicoId = Guid.NewGuid();
            var entidade = new ServicoModelo
            {
                Id = servicoId,
                ModeloId = modeloId,
                Nome = Domain.Comum.Texto.Limpo(servico.Nome)!,
                ValorEstimado = servico.ValorEstimado,
            };
            foreach (var item in servico.Itens ?? [])
            {
                entidade.Itens.Add(new ItemServicoModelo
                {
                    Id = Guid.NewGuid(),
                    ServicoId = servicoId,
                    Tipo = item.Tipo!.Value,
                    Texto = Domain.Comum.Texto.Limpo(item.Texto)!,
                });
            }

            atualizado.Servicos.Add(entidade);
        }

        foreach (var tarefa in pedido.Tarefas ?? [])
        {
            atualizado.Tarefas.Add(new TarefaModelo
            {
                Id = Guid.NewGuid(),
                ModeloId = modeloId,
                Titulo = Domain.Comum.Texto.Limpo(tarefa.Titulo)!,
                DiasAntes = tarefa.DiasAntes,
            });
        }

        foreach (var vinculo in pedido.Vinculos ?? [])
        {
            atualizado.Vinculos.Add(new VinculoModeloEvento
            {
                Id = Guid.NewGuid(),
                ModeloId = modeloId,
                Tipo = vinculo.Tipo!.Value,
                BibliotecaId = vinculo.BibliotecaId!.Value,
            });
        }

        await dados.GravarModeloAtualizadoAsync(atualizado, ct);
        return Resultado<ModeloLeitura>.Sucesso(Ler(atualizado));
    }

    public async Task<Resultado<ModeloLeitura>> ClonarAsync(string contaId, Guid id, CancellationToken ct)
    {
        var origem = await dados.ObterModeloAsync(contaId, id, ct);
        if (origem is null)
        {
            return Resultado<ModeloLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var copia = new ModeloEvento
        {
            Id = Guid.NewGuid(),
            ContaId = contaId,
            Nome = origem.Nome + " (cópia)",
            Tipo = origem.Tipo,
            PermiteGerais = origem.PermiteGerais,
            PermitePlanejamento = origem.PermitePlanejamento,
            PermiteTarefas = origem.PermiteTarefas,
            PermiteServicosFornecedores = origem.PermiteServicosFornecedores,
            PermiteConvites = origem.PermiteConvites,
        };

        foreach (var servico in origem.Servicos)
        {
            var servicoId = Guid.NewGuid();
            var novo = new ServicoModelo
            {
                Id = servicoId,
                ModeloId = copia.Id,
                Nome = servico.Nome,
                ValorEstimado = servico.ValorEstimado,
            };
            foreach (var item in servico.Itens)
            {
                novo.Itens.Add(new ItemServicoModelo
                {
                    Id = Guid.NewGuid(),
                    ServicoId = servicoId,
                    Tipo = item.Tipo,
                    Texto = item.Texto,
                });
            }

            copia.Servicos.Add(novo);
        }

        foreach (var tarefa in origem.Tarefas)
        {
            copia.Tarefas.Add(new TarefaModelo
            {
                Id = Guid.NewGuid(),
                ModeloId = copia.Id,
                Titulo = tarefa.Titulo,
                DiasAntes = tarefa.DiasAntes,
            });
        }

        foreach (var vinculo in origem.Vinculos)
        {
            copia.Vinculos.Add(new VinculoModeloEvento
            {
                Id = Guid.NewGuid(),
                ModeloId = copia.Id,
                Tipo = vinculo.Tipo,
                BibliotecaId = vinculo.BibliotecaId,
            });
        }

        await dados.InserirModeloAsync(copia, ct);
        return Resultado<ModeloLeitura>.Sucesso(Ler(copia));
    }

    public async Task<Resultado<bool>> ExcluirAsync(string contaId, Guid id, CancellationToken ct)
    {
        var modelo = await dados.ObterModeloAsync(contaId, id, ct);
        if (modelo is null)
        {
            return Resultado<bool>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        await dados.RemoverModeloAsync(modelo, ct);
        return Resultado<bool>.Sucesso(true);
    }

    private static string? ValidarConteudo(AtualizarModelo pedido)
    {
        foreach (var servico in pedido.Servicos ?? [])
        {
            if (Domain.Comum.Texto.Limpo(servico.Nome) is null)
            {
                return "Informe o nome do serviço.";
            }

            if (servico.ValorEstimado is < 0)
            {
                return "Use um número maior ou igual a zero.";
            }

            foreach (var item in servico.Itens ?? [])
            {
                if (item.Tipo is not TipoItemServicoModelo tipo || !Enum.IsDefined(tipo))
                {
                    return "Informe o tipo do item do serviço.";
                }

                if (Domain.Comum.Texto.Limpo(item.Texto) is null)
                {
                    return "Informe o texto do item do serviço.";
                }
            }
        }

        foreach (var tarefa in pedido.Tarefas ?? [])
        {
            if (Domain.Comum.Texto.Limpo(tarefa.Titulo) is null)
            {
                return "Informe o título da tarefa.";
            }
        }

        foreach (var vinculo in pedido.Vinculos ?? [])
        {
            if (vinculo.Tipo is not TipoVinculoModelo tipo || !Enum.IsDefined(tipo) || vinculo.BibliotecaId is null)
            {
                return "Informe o vínculo da biblioteca.";
            }
        }

        return null;
    }

    private static ModeloLeitura Ler(ModeloEvento modelo) => new(
        modelo.Id,
        modelo.Nome,
        modelo.Tipo,
        modelo.Servicos.Select(servico => new ServicoModeloLeitura(
            servico.Id,
            servico.Nome,
            servico.ValorEstimado,
            servico.Itens.Select(item => new ItemServicoModeloLeitura(item.Id, item.Tipo, item.Texto)).ToArray())).ToArray(),
        modelo.Tarefas.Select(tarefa => new TarefaModeloLeitura(tarefa.Id, tarefa.Titulo, tarefa.DiasAntes)).ToArray(),
        new PermissoesModeloLeitura(
            modelo.PermiteGerais,
            modelo.PermitePlanejamento,
            modelo.PermiteTarefas,
            modelo.PermiteServicosFornecedores,
            modelo.PermiteConvites),
        modelo.Vinculos.Select(vinculo => new VinculoModeloLeitura(vinculo.Id, vinculo.Tipo, vinculo.BibliotecaId)).ToArray());

    private static IEnumerable<string> Nomes(IReadOnlyList<string>? valores) =>
        (valores ?? []).Select(Domain.Comum.Texto.Limpo).OfType<string>();
}
