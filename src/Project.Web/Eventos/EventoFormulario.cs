using Microsoft.AspNetCore.Http;
using Project.Application.Eventos;
using Project.Domain.Eventos;

namespace Project.Web.Eventos;

public sealed class EventoInput
{
    public string? Nome { get; set; }

    public DateOnly? Data { get; set; }

    public TimeOnly? Hora { get; set; }

    public string? Tipo { get; set; }

    public string? Tags { get; set; }

    public string? CasalNome { get; set; }

    public string? CasalPapel { get; set; }

    public string? CasalEmail { get; set; }

    public string? CasalCelular { get; set; }

    public decimal? OrcamentoEstimado { get; set; }

    public int? ConvidadosEstimados { get; set; }

    public string? LocalCerimonia { get; set; }

    public string? LocalRecepcao { get; set; }

    public Guid? ModeloId { get; set; }

    public string? PrimeiroNome { get; set; }

    public string? SegundoNome { get; set; }

    public TimeOnly? TempoDeFesta { get; set; }

    public TimeOnly? InicioCerimonia { get; set; }

    public TimeOnly? InicioRecepcao { get; set; }

    public int? Padrinhos { get; set; }

    public int? DamasEPajens { get; set; }

    public bool NotificarAssessor { get; set; } = true;

    public bool NotificarAssistentes { get; set; } = true;

    public bool NotificarClientes { get; set; } = true;

    public bool SalvarNaAgenda { get; set; }

    public string? SenhaDoSite { get; set; }

    public string? CerimoniaPais { get; set; }

    public string? CerimoniaCep { get; set; }

    public string? CerimoniaLogradouro { get; set; }

    public string? CerimoniaNumero { get; set; }

    public string? CerimoniaEstado { get; set; }

    public string? CerimoniaCidade { get; set; }

    public string? CerimoniaBairro { get; set; }

    public string? RecepcaoPais { get; set; }

    public string? RecepcaoCep { get; set; }

    public string? RecepcaoLogradouro { get; set; }

    public string? RecepcaoNumero { get; set; }

    public string? RecepcaoEstado { get; set; }

    public string? RecepcaoCidade { get; set; }

    public string? RecepcaoBairro { get; set; }

    public IFormFile? Imagem { get; set; }

    public (Stream? Conteudo, string? Nome) AbrirImagem()
    {
        if (Imagem is null || (Imagem.Length == 0 && string.IsNullOrWhiteSpace(Imagem.FileName)))
        {
            return (null, null);
        }

        return (Imagem.OpenReadStream(), Imagem.FileName);
    }
}

public static class EventoFormulario
{
    public static bool TentarCriarDados(EventoInput input, out EventoDados dados, out string? erro)
    {
        if (!TiposDeEvento.TentarLer(input.Tipo, out var tipo))
        {
            dados = new EventoDados(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true, true, true, false, null, null, null);
            erro = "O tipo do evento não é aceito.";
            return false;
        }

        var tags = string.IsNullOrWhiteSpace(input.Tags)
            ? Array.Empty<string>()
            : input.Tags.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        dados = new EventoDados(
            input.Nome,
            input.Data,
            input.Hora,
            tipo,
            tags,
            input.CasalNome,
            input.CasalPapel,
            input.CasalEmail,
            input.CasalCelular,
            input.OrcamentoEstimado,
            input.ConvidadosEstimados,
            input.LocalCerimonia,
            input.LocalRecepcao,
            input.ModeloId,
            input.PrimeiroNome,
            input.SegundoNome,
            input.TempoDeFesta,
            input.InicioCerimonia,
            input.InicioRecepcao,
            input.Padrinhos,
            input.DamasEPajens,
            input.NotificarAssessor,
            input.NotificarAssistentes,
            input.NotificarClientes,
            input.SalvarNaAgenda,
            input.SenhaDoSite,
            new EnderecoLocal(input.CerimoniaPais, input.CerimoniaCep, input.CerimoniaLogradouro, input.CerimoniaNumero, input.CerimoniaEstado, input.CerimoniaCidade, input.CerimoniaBairro),
            new EnderecoLocal(input.RecepcaoPais, input.RecepcaoCep, input.RecepcaoLogradouro, input.RecepcaoNumero, input.RecepcaoEstado, input.RecepcaoCidade, input.RecepcaoBairro));
        erro = null;
        return true;
    }

    public static EventoInput ParaInput(EventoDetalhe detalhe)
    {
        return new EventoInput
        {
            Nome = detalhe.Nome,
            Data = detalhe.Data,
            Hora = detalhe.Hora,
            Tipo = detalhe.Tipo?.ToString(),
            Tags = string.Join(", ", detalhe.Tags),
            CasalNome = detalhe.CasalNome,
            CasalPapel = detalhe.CasalPapel,
            CasalEmail = detalhe.CasalEmail,
            CasalCelular = detalhe.CasalCelular,
            OrcamentoEstimado = detalhe.OrcamentoEstimado,
            ConvidadosEstimados = detalhe.ConvidadosEstimados,
            LocalCerimonia = detalhe.LocalCerimonia,
            LocalRecepcao = detalhe.LocalRecepcao,
            PrimeiroNome = detalhe.PrimeiroNome,
            SegundoNome = detalhe.SegundoNome,
            TempoDeFesta = detalhe.TempoDeFesta,
            InicioCerimonia = detalhe.InicioCerimonia,
            InicioRecepcao = detalhe.InicioRecepcao,
            Padrinhos = detalhe.Padrinhos,
            DamasEPajens = detalhe.DamasEPajens,
            NotificarAssessor = detalhe.NotificarAssessor,
            NotificarAssistentes = detalhe.NotificarAssistentes,
            NotificarClientes = detalhe.NotificarClientes,
            SalvarNaAgenda = detalhe.SalvarNaAgenda,
            SenhaDoSite = detalhe.SenhaDoSite,
            CerimoniaPais = detalhe.Cerimonia.Pais,
            CerimoniaCep = detalhe.Cerimonia.Cep,
            CerimoniaLogradouro = detalhe.Cerimonia.Logradouro,
            CerimoniaNumero = detalhe.Cerimonia.Numero,
            CerimoniaEstado = detalhe.Cerimonia.Estado,
            CerimoniaCidade = detalhe.Cerimonia.Cidade,
            CerimoniaBairro = detalhe.Cerimonia.Bairro,
            RecepcaoPais = detalhe.Recepcao.Pais,
            RecepcaoCep = detalhe.Recepcao.Cep,
            RecepcaoLogradouro = detalhe.Recepcao.Logradouro,
            RecepcaoNumero = detalhe.Recepcao.Numero,
            RecepcaoEstado = detalhe.Recepcao.Estado,
            RecepcaoCidade = detalhe.Recepcao.Cidade,
            RecepcaoBairro = detalhe.Recepcao.Bairro
        };
    }

    public static string? AvisoPermitido(string? aviso)
    {
        return aviso is "A imagem passa de 5 MB."
            or "O arquivo está vazio."
            or "O formato da imagem não é aceito."
            or "Evento concluído."
            or "Evento cancelado."
            or "O evento não está em andamento."
            ? aviso
            : null;
    }
}
