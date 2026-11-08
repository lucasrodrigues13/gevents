using Project.Domain.Convites;

namespace Project.Web.Convites;

public static class ConviteTexto
{
    public static string Situacao(SituacaoConvidado situacao)
    {
        return situacao switch
        {
            SituacaoConvidado.Pendente => "Pendente",
            SituacaoConvidado.Confirmado => "Confirmado",
            SituacaoConvidado.NaoComparecera => "Não comparecerá",
            _ => throw new InvalidOperationException("Situação desconhecida.")
        };
    }

    public static string Faixa(FaixaConvidado faixa)
    {
        return faixa switch
        {
            FaixaConvidado.Adulto => "Adulto",
            FaixaConvidado.Adolescente => "Adolescente",
            FaixaConvidado.Crianca => "Criança",
            FaixaConvidado.CriancaDeColo => "Criança de colo",
            FaixaConvidado.Idoso => "Idoso",
            FaixaConvidado.NaoInformado => "Não informado",
            _ => throw new InvalidOperationException("Faixa desconhecida.")
        };
    }

    public static string Custo(CustoConvidado custo)
    {
        return custo switch
        {
            CustoConvidado.Inteira => "Inteira",
            CustoConvidado.Meia => "Meia",
            CustoConvidado.Gratuita => "Gratuita",
            CustoConvidado.NaoInformado => "Não informado",
            _ => throw new InvalidOperationException("Custo desconhecido.")
        };
    }

    public static string Genero(GeneroConvidado genero)
    {
        return genero switch
        {
            GeneroConvidado.Masculino => "Masculino",
            GeneroConvidado.Feminino => "Feminino",
            GeneroConvidado.NaoBinario => "Não binário",
            GeneroConvidado.NaoInformado => "Não informado",
            _ => throw new InvalidOperationException("Gênero desconhecido.")
        };
    }
}
