using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Project.Application.Conta;
using Project.Infrastructure.Conta;
using Project.IntegrationTests.Suporte;

namespace Project.IntegrationTests.Negocio;

[Collection(ColecaoApi.Nome)]
public class ClienteAcessoFluxoTests
{
    private const string Senha = "abcdefgh";
    private readonly BancoFixture _banco;

    public ClienteAcessoFluxoTests(BancoFixture banco) => _banco = banco;

    [Fact]
    public async Task Organizer_concede_acesso_client_vinculado_e_flag_nega_tarefas()
    {
        var dono = await EntrarOrganizer();
        var modelo = await Ler(await dono.PostAsJsonAsync("/api/v1/modelos", new { nome = "Modelo", tipo = "Aniversario" }));
        var evento = await Ler(await dono.PostAsJsonAsync("/api/v1/eventos", new
        {
            modeloId = modelo.GetProperty("id").GetGuid(),
            primeiroNome = "Festa",
        }));
        var eventoId = evento.GetProperty("id").GetGuid();

        var emailCliente = $"client-{Guid.NewGuid():N}@gevents.test";
        var contato = await Ler(await dono.PostAsJsonAsync("/api/v1/clientes", new
        {
            nome = "Cliente Portal",
            papel = "Noiva",
            email = emailCliente,
            ddi = "+55",
            celular = "11988887777",
            acessoPlataforma = true,
            permissoes = new
            {
                convidados = true,
                tarefas = false,
                anexos = false,
                parcelas = false,
                rsvp = false,
                mapa = false,
                contrato = false,
            },
        }));

        Assert.True(contato.GetProperty("acessoPlataforma").GetBoolean());
        Assert.False(string.IsNullOrWhiteSpace(contato.GetProperty("usuarioId").GetString()));
        Assert.True(contato.GetProperty("permissoes").GetProperty("convidados").GetBoolean());
        Assert.False(contato.GetProperty("permissoes").GetProperty("tarefas").GetBoolean());

        Assert.Equal(HttpStatusCode.OK, (await dono.PostAsJsonAsync($"/api/v1/eventos/{eventoId}/clientes", new
        {
            clienteId = contato.GetProperty("id").GetGuid(),
        })).StatusCode);

        var reset = _banco.Fabrica.Email.Mensagens.Last(item => item.Email == emailCliente && item.Tipo == "redefinicao");
        var query = QueryHelpers.ParseQuery(new Uri(reset.Corpo).Query);
        var redefinicao = await dono.PostAsJsonAsync("/api/v1/conta/redefinir-senha", new
        {
            usuarioId = query["usuarioId"].ToString(),
            token = query["token"].ToString(),
            senha = Senha,
            confirmacao = Senha,
        });
        Assert.Equal(HttpStatusCode.OK, redefinicao.StatusCode);

        var clientHttp = _banco.Fabrica.CreateClient();
        var entrada = await clientHttp.PostAsJsonAsync("/api/v1/conta/entrar", new { email = emailCliente, senha = Senha });
        var sessao = await entrada.Content.ReadFromJsonAsync<Sessao>();
        Assert.Equal(HttpStatusCode.OK, entrada.StatusCode);
        Assert.Contains(Papeis.Client, sessao!.Papeis);
        Assert.DoesNotContain(Papeis.Organizer, sessao.Papeis);
        clientHttp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessao.Token);

        await using (var escopo = _banco.Fabrica.Services.CreateAsyncScope())
        {
            var usuarios = escopo.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var usuario = await usuarios.FindByEmailAsync(emailCliente);
            Assert.NotNull(usuario);
            var organizerEmail = (await Ler(await dono.GetAsync("/api/v1/conta/area"))).GetProperty("email").GetString()!;
            var organizer = await usuarios.FindByEmailAsync(organizerEmail);
            Assert.Equal(organizer!.Id, usuario!.OrganizerId);
            Assert.True(await usuarios.IsInRoleAsync(usuario, Papeis.Client));
        }

        var eventos = await Ler(await clientHttp.GetAsync("/api/v1/eventos"));
        Assert.Single(eventos.EnumerateArray());
        Assert.Equal(eventoId, eventos.EnumerateArray().First().GetProperty("id").GetGuid());

        Assert.Equal(HttpStatusCode.Forbidden, (await clientHttp.GetAsync($"/api/v1/eventos/{eventoId}/tarefas")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await clientHttp.GetAsync("/api/v1/clientes")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await clientHttp.GetAsync($"/api/v1/eventos/{eventoId}/convites")).StatusCode);
    }

    [Fact]
    public async Task Client_sem_vinculo_ao_evento_e_isolado_de_outro_organizer()
    {
        var dono = await EntrarOrganizer();
        var outro = await EntrarOrganizer();
        var modelo = await Ler(await dono.PostAsJsonAsync("/api/v1/modelos", new { nome = "Modelo", tipo = "Aniversario" }));
        var evento = await Ler(await dono.PostAsJsonAsync("/api/v1/eventos", new
        {
            modeloId = modelo.GetProperty("id").GetGuid(),
            primeiroNome = "Privado",
        }));
        var eventoId = evento.GetProperty("id").GetGuid();

        var emailCliente = $"client-{Guid.NewGuid():N}@gevents.test";
        var contato = await Ler(await dono.PostAsJsonAsync("/api/v1/clientes", new
        {
            nome = "Sem Evento",
            papel = "Noivo",
            email = emailCliente,
            ddi = "+55",
            celular = "11977776666",
            acessoPlataforma = true,
            permissoes = new { convidados = true, tarefas = true, anexos = true, parcelas = true, rsvp = true, mapa = true, contrato = true },
        }));

        var reset = _banco.Fabrica.Email.Mensagens.Last(item => item.Email == emailCliente && item.Tipo == "redefinicao");
        var query = QueryHelpers.ParseQuery(new Uri(reset.Corpo).Query);
        Assert.Equal(HttpStatusCode.OK, (await dono.PostAsJsonAsync("/api/v1/conta/redefinir-senha", new
        {
            usuarioId = query["usuarioId"].ToString(),
            token = query["token"].ToString(),
            senha = Senha,
            confirmacao = Senha,
        })).StatusCode);

        var clientHttp = await EntrarComo(emailCliente, Senha);
        Assert.Equal(HttpStatusCode.NotFound, (await clientHttp.GetAsync($"/api/v1/eventos/{eventoId}")).StatusCode);
        Assert.Empty((await Ler(await clientHttp.GetAsync("/api/v1/eventos"))).EnumerateArray());

        Assert.Equal(HttpStatusCode.OK, (await dono.PostAsJsonAsync($"/api/v1/eventos/{eventoId}/clientes", new
        {
            clienteId = contato.GetProperty("id").GetGuid(),
        })).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await clientHttp.GetAsync($"/api/v1/eventos/{eventoId}")).StatusCode);

        var buscaAlheia = await Ler(await outro.GetAsync($"/api/v1/clientes?nome=Sem"));
        Assert.Empty(buscaAlheia.EnumerateArray());
        Assert.Equal(HttpStatusCode.NotFound, (await outro.GetAsync($"/api/v1/eventos/{eventoId}")).StatusCode);
    }

    [Fact]
    public async Task Client_sem_OrganizerId_nao_entra()
    {
        var email = $"solto-{Guid.NewGuid():N}@gevents.test";
        await using (var escopo = _banco.Fabrica.Services.CreateAsyncScope())
        {
            var usuarios = escopo.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var usuario = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
            Assert.True((await usuarios.CreateAsync(usuario, Senha)).Succeeded);
            Assert.True((await usuarios.AddToRoleAsync(usuario, Papeis.Client)).Succeeded);
        }

        var cliente = _banco.Fabrica.CreateClient();
        var entrada = await cliente.PostAsJsonAsync("/api/v1/conta/entrar", new { email, senha = Senha });
        Assert.Equal(HttpStatusCode.Unauthorized, entrada.StatusCode);
    }

    private async Task<HttpClient> EntrarOrganizer()
    {
        var email = $"org-{Guid.NewGuid():N}@gevents.test";
        var cliente = _banco.Fabrica.CreateClient();
        Assert.Equal(HttpStatusCode.Created, (await cliente.PostAsJsonAsync("/api/v1/conta/cadastro", new { email, senha = Senha, confirmacao = Senha })).StatusCode);
        var mensagem = _banco.Fabrica.Email.Mensagens.Last(item => item.Email == email && item.Tipo == "confirmacao");
        var query = QueryHelpers.ParseQuery(new Uri(mensagem.Corpo).Query);
        Assert.Equal(HttpStatusCode.OK, (await cliente.PostAsJsonAsync("/api/v1/conta/confirmar-email", new
        {
            usuarioId = query["usuarioId"].ToString(),
            token = query["token"].ToString(),
        })).StatusCode);
        var entrada = await cliente.PostAsJsonAsync("/api/v1/conta/entrar", new { email, senha = Senha });
        var sessao = await entrada.Content.ReadFromJsonAsync<Sessao>();
        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessao!.Token);
        return cliente;
    }

    private async Task<HttpClient> EntrarComo(string email, string senha)
    {
        var cliente = _banco.Fabrica.CreateClient();
        var entrada = await cliente.PostAsJsonAsync("/api/v1/conta/entrar", new { email, senha });
        var sessao = await entrada.Content.ReadFromJsonAsync<Sessao>();
        Assert.Equal(HttpStatusCode.OK, entrada.StatusCode);
        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessao!.Token);
        return cliente;
    }

    private static async Task<JsonElement> Ler(HttpResponseMessage resposta)
    {
        var texto = await resposta.Content.ReadAsStringAsync();
        Assert.True(resposta.IsSuccessStatusCode, texto);
        using var documento = JsonDocument.Parse(texto);
        return documento.RootElement.Clone();
    }

    private sealed record Sessao(string Token, string Email, string[] Papeis);
}
