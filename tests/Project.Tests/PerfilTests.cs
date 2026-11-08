using Project.Domain.Perfis;

namespace Project.Tests;

public class PerfilTests
{
    [Fact]
    public void Nascimento_futuro_e_recusado()
    {
        var perfil = Perfil.Criar("conta");
        var hoje = new DateOnly(2026, 11, 7);

        var error = Assert.Throws<Project.Domain.RegraDeNegocioException>(() =>
            perfil.Atualizar(null, null, null, null, hoje.AddDays(1), null, hoje));

        Assert.Equal("A data de nascimento não pode ser futura.", error.Message);
    }

    [Fact]
    public void Campos_vazios_nao_apagam_a_conta()
    {
        var perfil = Perfil.Criar("conta");
        perfil.Atualizar("  ", null, "123", null, null, null, new DateOnly(2026, 11, 7));

        Assert.Null(perfil.NomeEmpresa);
        Assert.Equal("123", perfil.Documento);
        Assert.Equal("conta", perfil.AccountId);
    }
}
