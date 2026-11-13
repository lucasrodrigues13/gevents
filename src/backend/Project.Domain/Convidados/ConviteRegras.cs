namespace Project.Domain.Convidados;

public static class ConviteRegras
{
    public static bool TemTelefone(string? celular) => !Comum.Texto.Vazio(celular);

    public static bool PodeSerPrincipal(bool doMesmoConvite, string? celular) =>
        doMesmoConvite && TemTelefone(celular);
}
