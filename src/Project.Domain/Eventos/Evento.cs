using System.Net.Mail;

namespace Project.Domain.Eventos;

public sealed class Evento
{
    private Evento()
    {
    }

    public Guid Id { get; private set; }

    public string AccountId { get; private set; } = "";

    public string Nome { get; private set; } = "";

    public SituacaoEvento Situacao { get; private set; }

    public DateOnly? Data { get; private set; }

    public TimeOnly? Hora { get; private set; }

    public TipoEvento? Tipo { get; private set; }

    public List<string> Tags { get; private set; } = [];

    public string? CasalNome { get; private set; }

    public string? CasalPapel { get; private set; }

    public string? CasalEmail { get; private set; }

    public string? CasalCelular { get; private set; }

    public decimal? OrcamentoEstimado { get; private set; }

    public int? ConvidadosEstimados { get; private set; }

    public string? LocalCerimonia { get; private set; }

    public string? LocalRecepcao { get; private set; }

    public Guid? ArquivoId { get; private set; }

    public static Evento Criar(string accountId, EventoDados dados)
    {
        var evento = new Evento
        {
            Id = Guid.NewGuid(),
            AccountId = ExigirConta(accountId),
            Situacao = SituacaoEvento.EmAndamento
        };
        evento.Aplicar(dados);
        return evento;
    }

    public void Editar(EventoDados dados)
    {
        ExigirAberto();
        Aplicar(dados);
    }

    public void DefinirArquivo(Guid? arquivoId)
    {
        ExigirAberto();
        ArquivoId = arquivoId;
    }

    public void Concluir()
    {
        ExigirAberto();
        Situacao = SituacaoEvento.Concluido;
    }

    public void Cancelar()
    {
        ExigirAberto();
        Situacao = SituacaoEvento.Cancelado;
    }

    private void Aplicar(EventoDados dados)
    {
        var nome = dados.Nome?.Trim() ?? "";
        if (nome.Length == 0)
        {
            throw new RegraDeNegocioException("Informe o nome do evento.");
        }

        if (dados.OrcamentoEstimado is < 0 || dados.ConvidadosEstimados is < 0)
        {
            throw new RegraDeNegocioException("Orçamento e número de convidados não podem ser negativos.");
        }

        var email = Vazio(dados.CasalEmail);
        if (email is not null && !EmailValido(email))
        {
            throw new RegraDeNegocioException("Informe um e-mail válido.");
        }

        Nome = nome;
        Data = dados.Data;
        Hora = dados.Hora;
        Tipo = dados.Tipo;
        CasalNome = Vazio(dados.CasalNome);
        CasalPapel = Vazio(dados.CasalPapel);
        CasalEmail = email;
        CasalCelular = Vazio(dados.CasalCelular);
        OrcamentoEstimado = dados.OrcamentoEstimado;
        ConvidadosEstimados = dados.ConvidadosEstimados;
        LocalCerimonia = Vazio(dados.LocalCerimonia);
        LocalRecepcao = Vazio(dados.LocalRecepcao);
        Tags = (dados.Tags ?? [])
            .Select(tag => tag.Trim())
            .Where(tag => tag.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private void ExigirAberto()
    {
        if (Situacao != SituacaoEvento.EmAndamento)
        {
            throw new RegraDeNegocioException("O evento não está em andamento.");
        }
    }

    private static string ExigirConta(string accountId)
    {
        if (string.IsNullOrWhiteSpace(accountId))
        {
            throw new RegraDeNegocioException("A conta do evento é obrigatória.");
        }

        return accountId;
    }

    private static string? Vazio(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    }

    private static bool EmailValido(string email)
    {
        return MailAddress.TryCreate(email, out var endereco)
            && endereco.Address.Equals(email, StringComparison.OrdinalIgnoreCase);
    }
}

public sealed record EventoDados(
    string? Nome,
    DateOnly? Data,
    TimeOnly? Hora,
    TipoEvento? Tipo,
    IReadOnlyList<string>? Tags,
    string? CasalNome,
    string? CasalPapel,
    string? CasalEmail,
    string? CasalCelular,
    decimal? OrcamentoEstimado,
    int? ConvidadosEstimados,
    string? LocalCerimonia,
    string? LocalRecepcao);
