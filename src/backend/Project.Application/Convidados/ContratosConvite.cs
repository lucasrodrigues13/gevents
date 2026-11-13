namespace Project.Application.Convidados;

public sealed record LinhaPlanilha(
    string? Convite,
    string? Ddi,
    string? Telefone,
    string? Grupo,
    string? Observacao,
    string? Convidado,
    string? Genero,
    string? Faixa,
    string? Custo,
    string? Situacao,
    string? Mesa);

public interface IPlanilhaConvites
{
    byte[] Modelo();

    Comum.Resultado<IReadOnlyList<LinhaPlanilha>> Ler(Stream arquivo, string? nomeArquivo);
}

public interface IGeradorQr
{
    byte[] Png(string texto);
}
