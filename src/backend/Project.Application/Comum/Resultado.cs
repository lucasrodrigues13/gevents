namespace Project.Application.Comum;

public sealed record Falha(string Codigo, string Mensagem);

public sealed class Resultado<T>
{
    private Resultado(T? valor, Falha? falha)
    {
        Valor = valor;
        Falha = falha;
    }

    public T? Valor { get; }

    public Falha? Falha { get; }

    public bool Ok => Falha is null;

    public static Resultado<T> Sucesso(T valor) => new(valor, null);

    public static Resultado<T> Erro(string codigo, string mensagem) => new(default, new Falha(codigo, mensagem));
}

public static class Codigos
{
    public const string Validacao = "validacao";
    public const string Indisponivel = "indisponivel";
}
