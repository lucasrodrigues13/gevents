using Project.Domain.Eventos;

namespace Project.Web.Eventos;

public static class EventoTexto
{
    public static string Situacao(SituacaoEvento situacao)
    {
        return situacao switch
        {
            SituacaoEvento.EmAndamento => "Em andamento",
            SituacaoEvento.Concluido => "Concluído",
            SituacaoEvento.Cancelado => "Cancelado",
            _ => throw new InvalidOperationException("Situação desconhecida.")
        };
    }

    public static string? Tipo(TipoEvento? tipo)
    {
        return tipo switch
        {
            TipoEvento.Casamento => "Casamento",
            TipoEvento.QuinzeAnos => "15 anos",
            TipoEvento.Aniversario => "Aniversário",
            TipoEvento.Corporativo => "Corporativo",
            TipoEvento.Formatura => "Formatura",
            null => null,
            _ => throw new InvalidOperationException("Tipo desconhecido.")
        };
    }

    public static string? Contagem(DateOnly? data, DateOnly hoje)
    {
        if (data is null)
        {
            return null;
        }

        var dias = data.Value.DayNumber - hoje.DayNumber;
        if (dias > 1)
        {
            return $"Faltam {dias} dias";
        }

        if (dias == 1)
        {
            return "Falta 1 dia";
        }

        if (dias == 0)
        {
            return "É hoje";
        }

        return "A data já passou";
    }
}
