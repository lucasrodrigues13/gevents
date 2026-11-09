using Project.Domain;
using Project.Domain.Clientes;

namespace Project.Tests;

public class ClienteTests
{
    [Fact]
    public void Nome_vazio_e_email_invalido_sao_recusados()
    {
        Assert.Equal("Informe o nome do cliente.", Assert.Throws<RegraDeNegocioException>(() => Cliente.Criar("conta", Dados("  "))).Message);
        Assert.Equal(
            "Informe um e-mail válido.",
            Assert.Throws<RegraDeNegocioException>(() => Cliente.Criar("conta", Dados("Ana") with { Email = "sem-arroba" })).Message);
    }

    [Fact]
    public void Papel_fora_da_lista_e_cpf_incompleto_sao_recusados()
    {
        Assert.Equal("O papel do cliente não é aceito.", Assert.Throws<RegraDeNegocioException>(() => Cliente.Criar("conta", Dados("Ana") with { Papel = "Padrinho" })).Message);
        Assert.Equal("O CPF precisa ter 11 dígitos.", Assert.Throws<RegraDeNegocioException>(() => Cliente.Criar("conta", Dados("Ana") with { Cpf = "123" })).Message);
        var cliente = Cliente.Criar("conta", Dados("Ana"));
        Assert.Equal("Noiva", cliente.Papel);
        Assert.Equal("+55", cliente.Ddi);
    }

    private static ClienteDados Dados(string nome)
    {
        return new ClienteDados(nome, "Noiva", "ana@example.com", "+55", "31999990000", null, null, null, null, null, null, null, null, null, null, null, null, null);
    }
}
