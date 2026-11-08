using Project.Domain;
using Project.Domain.Clientes;

namespace Project.Tests;

public class ClienteTests
{
    [Fact]
    public void Nome_vazio_e_email_invalido_sao_recusados()
    {
        Assert.Equal("Informe o nome do cliente.", Assert.Throws<RegraDeNegocioException>(() => Cliente.Criar("conta", "  ", null, null)).Message);
        Assert.Equal(
            "Informe um e-mail válido.",
            Assert.Throws<RegraDeNegocioException>(() => Cliente.Criar("conta", "Ana", null, "sem-arroba")).Message);
    }
}
