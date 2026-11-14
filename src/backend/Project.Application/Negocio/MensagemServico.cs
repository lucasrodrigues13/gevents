using Project.Application.Comum;
using Project.Domain.Modelos;

namespace Project.Application.Negocio;

public sealed record MensagemLeitura(Guid Id, string Nome, string Texto, IReadOnlyList<string> Parametros);

public sealed record SalvarMensagem(string? Nome, string? Texto);

public sealed class MensagemServico(IDados dados)
{
    public async Task<IReadOnlyList<MensagemLeitura>> ListarAsync(string contaId, string? nome, CancellationToken ct)
    {
        var modelos = await dados.ListarMensagensAsync(contaId, nome, ct);
        return modelos.Select(Ler).ToArray();
    }

    public async Task<Resultado<MensagemLeitura>> ObterAsync(string contaId, Guid id, CancellationToken ct)
    {
        var modelo = await dados.ObterMensagemAsync(contaId, id, ct);
        return modelo is null
            ? Resultado<MensagemLeitura>.Erro(Codigos.Indisponivel, "Indisponível.")
            : Resultado<MensagemLeitura>.Sucesso(Ler(modelo));
    }

    public async Task<Resultado<MensagemLeitura>> CriarAsync(string contaId, SalvarMensagem pedido, CancellationToken ct)
    {
        var montado = Montar(contaId, Guid.NewGuid(), pedido);
        if (!montado.Ok)
        {
            return Resultado<MensagemLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        await dados.InserirMensagemAsync(montado.Valor!, ct);
        return Resultado<MensagemLeitura>.Sucesso(Ler(montado.Valor!));
    }

    public async Task<Resultado<MensagemLeitura>> AtualizarAsync(string contaId, Guid id, SalvarMensagem pedido, CancellationToken ct)
    {
        var modelo = await dados.ObterMensagemAsync(contaId, id, ct);
        if (modelo is null)
        {
            return Resultado<MensagemLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var montado = Montar(contaId, modelo.Id, pedido);
        if (!montado.Ok)
        {
            return Resultado<MensagemLeitura>.Erro(montado.Falha!.Codigo, montado.Falha.Mensagem);
        }

        modelo.Nome = montado.Valor!.Nome;
        modelo.Texto = montado.Valor.Texto;
        await dados.SalvarMensagemAsync(ct);
        return Resultado<MensagemLeitura>.Sucesso(Ler(modelo));
    }

    public async Task<Resultado<MensagemLeitura>> ClonarAsync(string contaId, Guid id, CancellationToken ct)
    {
        var origem = await dados.ObterMensagemAsync(contaId, id, ct);
        if (origem is null)
        {
            return Resultado<MensagemLeitura>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        var copia = new ModeloMensagem
        {
            Id = Guid.NewGuid(),
            ContaId = contaId,
            Nome = origem.Nome + " (cópia)",
            Texto = origem.Texto,
        };
        await dados.InserirMensagemAsync(copia, ct);
        return Resultado<MensagemLeitura>.Sucesso(Ler(copia));
    }

    public async Task<Resultado<bool>> ExcluirAsync(string contaId, Guid id, CancellationToken ct)
    {
        var modelo = await dados.ObterMensagemAsync(contaId, id, ct);
        if (modelo is null)
        {
            return Resultado<bool>.Erro(Codigos.Indisponivel, "Indisponível.");
        }

        await dados.RemoverMensagemAsync(modelo, ct);
        return Resultado<bool>.Sucesso(true);
    }

    private static Resultado<ModeloMensagem> Montar(string contaId, Guid id, SalvarMensagem pedido)
    {
        var nome = Domain.Comum.Texto.Limpo(pedido.Nome);
        var texto = Domain.Comum.Texto.Limpo(pedido.Texto);
        if (nome is null)
        {
            return Resultado<ModeloMensagem>.Erro(Codigos.Validacao, "Informe o nome do modelo.");
        }

        if (texto is null)
        {
            return Resultado<ModeloMensagem>.Erro(Codigos.Validacao, "Informe o texto do modelo.");
        }

        return Resultado<ModeloMensagem>.Sucesso(new ModeloMensagem
        {
            Id = id,
            ContaId = contaId,
            Nome = nome,
            Texto = texto,
        });
    }

    private static MensagemLeitura Ler(ModeloMensagem modelo) =>
        new(modelo.Id, modelo.Nome, modelo.Texto, ParametrosMensagem.Todos);
}
