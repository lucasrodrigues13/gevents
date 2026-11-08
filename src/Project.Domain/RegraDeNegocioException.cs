namespace Project.Domain;

public sealed class RegraDeNegocioException : Exception
{
    public RegraDeNegocioException(string message)
        : base(message)
    {
    }
}
