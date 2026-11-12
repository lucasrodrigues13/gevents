namespace Project.Application.Conta;

public interface IEmissorJwt
{
    string Emitir(string usuarioId, string email, IReadOnlyList<string> papeis);
}
