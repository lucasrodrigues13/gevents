namespace Project.Domain.Convites;

public sealed class Convidado
{
    private Convidado()
    {
    }

    public Guid Id { get; private set; }

    public Guid ConviteId { get; private set; }

    public string Nome { get; private set; } = "";

    public SituacaoConvidado Situacao { get; private set; }

    public FaixaConvidado? Faixa { get; private set; }

    public CustoConvidado? Custo { get; private set; }

    public GeneroConvidado? Genero { get; private set; }

    public string? Celular { get; private set; }

    public string? Observacao { get; private set; }

    public Guid? GrupoId { get; private set; }

    public bool Transfer { get; private set; }

    public string? Origem { get; private set; }

    public string? Ddi { get; private set; }

    public string? Mesa { get; private set; }

    public string? Rg { get; private set; }

    public string? Cpf { get; private set; }

    public static Convidado Criar(Guid conviteId, ConvidadoDados dados)
    {
        if (conviteId == Guid.Empty)
        {
            throw new RegraDeNegocioException("O convidado precisa de um convite.");
        }

        var convidado = new Convidado
        {
            Id = Guid.NewGuid(),
            ConviteId = conviteId,
            Situacao = SituacaoConvidado.Pendente
        };
        convidado.Aplicar(dados);
        if (dados.Situacao is not null)
        {
            convidado.DefinirSituacao(dados.Situacao.Value);
        }

        return convidado;
    }

    public void DefinirSituacao(SituacaoConvidado situacao)
    {
        if (!Enum.IsDefined(situacao))
        {
            throw new RegraDeNegocioException("A situação não é aceita.");
        }

        Situacao = situacao;
    }

    public void DefinirTelefone(string? ddi, string? celular)
    {
        Ddi = Vazio(ddi);
        Celular = Vazio(celular);
    }

    public void Responder(SituacaoConvidado situacao)
    {
        if (situacao is not (SituacaoConvidado.Confirmado or SituacaoConvidado.NaoComparecera))
        {
            throw new RegraDeNegocioException("A presença não é aceita.");
        }

        Situacao = situacao;
    }

    private void Aplicar(ConvidadoDados dados)
    {
        if (dados.Transfer && string.IsNullOrWhiteSpace(dados.Origem))
        {
            throw new RegraDeNegocioException("Informe o local de origem do transfer.");
        }

        Nome = Grupo.ExigirNome(dados.Nome, "Informe o nome do convidado.");
        Faixa = dados.Faixa;
        Custo = dados.Custo;
        Genero = dados.Genero;
        Celular = Vazio(dados.Celular);
        Observacao = Vazio(dados.Observacao);
        GrupoId = dados.GrupoId;
        Transfer = dados.Transfer;
        Origem = dados.Transfer ? dados.Origem!.Trim() : null;
        Ddi = Vazio(dados.Ddi);
        Mesa = Vazio(dados.Mesa);
        Rg = Vazio(dados.Rg);
        Cpf = Vazio(dados.Cpf);
    }

    private static string? Vazio(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    }
}
