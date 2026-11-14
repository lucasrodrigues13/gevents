using Project.Application.Negocio;
using Project.Domain.Clientes;

namespace Project.Tests.Negocio;

public class ClientePermissoesTests
{
    [Fact]
    public void Sem_acesso_nenhuma_flag_libera_modulo()
    {
        var cliente = new Cliente { AcessoPlataforma = false, PermiteTarefas = true };

        Assert.False(cliente.TemPermissao(ModuloCliente.Tarefas));
        Assert.False(cliente.TemPermissao(ModuloCliente.Convidados));
    }

    [Fact]
    public void Com_acesso_so_flags_marcadas_liberam()
    {
        var cliente = new Cliente { AcessoPlataforma = true };
        cliente.AplicarPermissoes(
            convidados: true,
            tarefas: false,
            anexos: true,
            parcelas: false,
            rsvp: true,
            mapa: false,
            contrato: false);

        Assert.True(cliente.TemPermissao(ModuloCliente.Convidados));
        Assert.False(cliente.TemPermissao(ModuloCliente.Tarefas));
        Assert.True(cliente.TemPermissao(ModuloCliente.Anexos));
        Assert.False(cliente.TemPermissao(ModuloCliente.Parcelas));
        Assert.True(cliente.TemPermissao(ModuloCliente.Rsvp));
        Assert.False(cliente.TemPermissao(ModuloCliente.Mapa));
        Assert.False(cliente.TemPermissao(ModuloCliente.Contrato));
    }

    [Fact]
    public void Montar_grava_flags_e_acesso_no_contato()
    {
        var montado = ClienteServico.Montar(
            "org-1",
            new NovoCliente(
                Nome: "Ana",
                Papel: PapelCliente.Noiva,
                Nascimento: null,
                Email: "ana@exemplo.com",
                Ddi: "+55",
                Celular: "11999999999",
                TelefoneFixo: null,
                Cpf: null,
                Rg: null,
                Cnpj: null,
                Profissao: null,
                InscricaoMunicipal: null,
                Religiao: null,
                Observacao: null,
                Facebook: null,
                Instagram: null,
                Pais: null,
                Cep: null,
                Endereco: null,
                Numero: null,
                Estado: null,
                Cidade: null,
                Bairro: null,
                Complemento: null,
                AcessoPlataforma: true,
                Permissoes: new PermissoesClientePedido(true, true, false, false, false, false, false)),
            new DateOnly(2026, 11, 14));

        Assert.True(montado.Ok);
        Assert.True(montado.Valor!.AcessoPlataforma);
        Assert.True(montado.Valor.PermiteConvidados);
        Assert.True(montado.Valor.PermiteTarefas);
        Assert.False(montado.Valor.PermiteAnexos);
    }
}
