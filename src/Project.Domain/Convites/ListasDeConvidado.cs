namespace Project.Domain.Convites;

public enum SituacaoConvidado
{
    Pendente = 0,
    Confirmado = 1,
    NaoComparecera = 2
}

public enum FaixaConvidado
{
    Adulto = 0,
    Adolescente = 1,
    Crianca = 2,
    CriancaDeColo = 3,
    Idoso = 4,
    NaoInformado = 5
}

public enum CustoConvidado
{
    Inteira = 0,
    Meia = 1,
    Gratuita = 2,
    NaoInformado = 3
}

public enum GeneroConvidado
{
    Masculino = 0,
    Feminino = 1,
    NaoBinario = 2,
    NaoInformado = 3
}
