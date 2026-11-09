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

    public Guid? ModeloId { get; private set; }

    public string? PrimeiroNome { get; private set; }

    public string? SegundoNome { get; private set; }

    public TimeOnly? TempoDeFesta { get; private set; }

    public TimeOnly? InicioCerimonia { get; private set; }

    public TimeOnly? InicioRecepcao { get; private set; }

    public int? Padrinhos { get; private set; }

    public int? DamasEPajens { get; private set; }

    public bool NotificarAssessor { get; private set; } = true;

    public bool NotificarAssistentes { get; private set; } = true;

    public bool NotificarClientes { get; private set; } = true;

    public bool SalvarNaAgenda { get; private set; }

    public bool TransferAtivo { get; private set; }

    public string? SenhaDoSite { get; private set; }

    public string? CerimoniaPais { get; private set; }

    public string? CerimoniaCep { get; private set; }

    public string? CerimoniaLogradouro { get; private set; }

    public string? CerimoniaNumero { get; private set; }

    public string? CerimoniaEstado { get; private set; }

    public string? CerimoniaCidade { get; private set; }

    public string? CerimoniaBairro { get; private set; }

    public string? RecepcaoPais { get; private set; }

    public string? RecepcaoCep { get; private set; }

    public string? RecepcaoLogradouro { get; private set; }

    public string? RecepcaoNumero { get; private set; }

    public string? RecepcaoEstado { get; private set; }

    public string? RecepcaoCidade { get; private set; }

    public string? RecepcaoBairro { get; private set; }

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

    public static Evento CriarComModelo(
        string accountId,
        Modelo? modelo,
        string? primeiroNome,
        string? segundoNome,
        EventoDados dados)
    {
        if (modelo is null)
        {
            throw new RegraDeNegocioException("Escolha um modelo.");
        }

        if (modelo.AccountId != accountId)
        {
            throw new RegraDeNegocioException("O modelo não está disponível.");
        }

        string nome;
        if (modelo.Tipo == TipoEvento.Casamento)
        {
            var primeiro = Modelo.ExigirNome(primeiroNome, "Informe os dois nomes.");
            var segundo = Modelo.ExigirNome(segundoNome, "Informe os dois nomes.");
            nome = $"{primeiro} & {segundo}";
        }
        else
        {
            nome = Modelo.ExigirNome(primeiroNome, "Informe o nome do evento.");
        }

        var evento = Criar(accountId, dados with
        {
            Nome = nome,
            Tipo = modelo.Tipo,
            ModeloId = modelo.Id,
            PrimeiroNome = primeiroNome,
            SegundoNome = segundoNome
        });
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

    public void DefinirTransfer(bool ativo)
    {
        ExigirAberto();
        TransferAtivo = ativo;
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
        if (!string.IsNullOrWhiteSpace(dados.PrimeiroNome))
        {
            PrimeiroNome = dados.PrimeiroNome.Trim();
            SegundoNome = Vazio(dados.SegundoNome);
            if (dados.Tipo == TipoEvento.Casamento && !string.IsNullOrWhiteSpace(dados.SegundoNome))
            {
                Nome = $"{PrimeiroNome} & {SegundoNome}";
            }
            else if (dados.Tipo != TipoEvento.Casamento)
            {
                Nome = PrimeiroNome;
            }
        }

        TempoDeFesta = dados.TempoDeFesta;
        InicioCerimonia = dados.InicioCerimonia;
        InicioRecepcao = dados.InicioRecepcao;
        Padrinhos = dados.Padrinhos;
        DamasEPajens = dados.DamasEPajens;
        NotificarAssessor = dados.NotificarAssessor;
        NotificarAssistentes = dados.NotificarAssistentes;
        NotificarClientes = dados.NotificarClientes;
        SalvarNaAgenda = dados.SalvarNaAgenda;
        SenhaDoSite = Vazio(dados.SenhaDoSite);
        ModeloId = dados.ModeloId ?? ModeloId;
        AplicarEndereco(dados.Cerimonia, cerimonia: true);
        AplicarEndereco(dados.Recepcao, cerimonia: false);
        Tags = (dados.Tags ?? [])
            .Select(tag => tag.Trim())
            .Where(tag => tag.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private void AplicarEndereco(EnderecoLocal? endereco, bool cerimonia)
    {
        var normalizado = (endereco ?? EnderecoLocal.Vazio).Normalizar();
        if (cerimonia)
        {
            CerimoniaPais = normalizado.Pais;
            CerimoniaCep = normalizado.Cep;
            CerimoniaLogradouro = normalizado.Logradouro;
            CerimoniaNumero = normalizado.Numero;
            CerimoniaEstado = normalizado.Estado;
            CerimoniaCidade = normalizado.Cidade;
            CerimoniaBairro = normalizado.Bairro;
            return;
        }

        RecepcaoPais = normalizado.Pais;
        RecepcaoCep = normalizado.Cep;
        RecepcaoLogradouro = normalizado.Logradouro;
        RecepcaoNumero = normalizado.Numero;
        RecepcaoEstado = normalizado.Estado;
        RecepcaoCidade = normalizado.Cidade;
        RecepcaoBairro = normalizado.Bairro;
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
    string? LocalRecepcao,
    Guid? ModeloId = null,
    string? PrimeiroNome = null,
    string? SegundoNome = null,
    TimeOnly? TempoDeFesta = null,
    TimeOnly? InicioCerimonia = null,
    TimeOnly? InicioRecepcao = null,
    int? Padrinhos = null,
    int? DamasEPajens = null,
    bool NotificarAssessor = true,
    bool NotificarAssistentes = true,
    bool NotificarClientes = true,
    bool SalvarNaAgenda = false,
    string? SenhaDoSite = null,
    EnderecoLocal? Cerimonia = null,
    EnderecoLocal? Recepcao = null);
