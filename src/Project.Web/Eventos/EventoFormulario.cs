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
            dados = new EventoDados(null, null, null, null, null, null, null, null, null, null, null, null, null);
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
            input.LocalRecepcao);
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
            LocalRecepcao = detalhe.LocalRecepcao
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
