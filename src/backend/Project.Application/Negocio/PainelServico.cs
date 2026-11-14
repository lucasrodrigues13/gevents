using Project.Domain.Eventos;
using Project.Domain.Funil;

namespace Project.Application.Negocio;

public sealed record FunilEstagio(string Estagio, int Quantidade, decimal Valor);

public sealed record PainelLeitura(
    int EmAndamento,
    int Concluidos,
    int Cancelados,
    IReadOnlyList<EventoAndamento> EventosEmAndamento,
    IReadOnlyList<FunilEstagio> Funil);

public sealed class PainelServico(IDados dados)
{
    private static readonly FunilEstagio[] FunilZerado =
    [
        new("Contato inicial", 0, 0),
        new("Orçamento enviado", 0, 0),
        new("Contratado", 0, 0),
        new("Descartado", 0, 0),
    ];

    public async Task<PainelLeitura> ObterAsync(string contaId, CancellationToken ct)
    {
        var contagens = await dados.ContarEventosAsync(contaId, ct);
        var andamento = await dados.ListarEmAndamentoAsync(contaId, ct);
        var funil = await dados.ContarOportunidadesPorEstagioAsync(contaId, ct);
        var funilLeitura = FunilZerado.Select(baseEstagio =>
        {
            var estagio = RotulosEstagio.Todos.First(e => RotulosEstagio.Texto(e) == baseEstagio.Estagio);
            var item = funil.FirstOrDefault(c => c.Estagio == estagio);
            return new FunilEstagio(baseEstagio.Estagio, item?.Quantidade ?? 0, item?.Valor ?? 0);
        }).ToArray();
        return new PainelLeitura(
            Quantidade(contagens, SituacaoEvento.EmAndamento),
            Quantidade(contagens, SituacaoEvento.Concluido),
            Quantidade(contagens, SituacaoEvento.Cancelado),
            andamento,
            funilLeitura);
    }

    private static int Quantidade(IReadOnlyList<ContagemSituacao> contagens, SituacaoEvento situacao) =>
        contagens.FirstOrDefault(item => item.Situacao == situacao)?.Quantidade ?? 0;
}
