using Project.Domain.Funil;

namespace Project.Tests.Negocio;

public class FunilRegrasTests
{
    [Fact]
    public void Rotulos_estagio_seguem_spec_007()
    {
        Assert.Equal("Contato inicial", RotulosEstagio.Texto(EstagioOportunidade.ContatoInicial));
        Assert.Equal("Orçamento enviado", RotulosEstagio.Texto(EstagioOportunidade.OrcamentoEnviado));
        Assert.Equal("Contratado", RotulosEstagio.Texto(EstagioOportunidade.Contratado));
        Assert.Equal("Descartado", RotulosEstagio.Texto(EstagioOportunidade.Descartado));
        Assert.Equal(4, RotulosEstagio.Todos.Length);
    }
}
