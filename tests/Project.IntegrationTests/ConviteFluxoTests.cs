using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Project.Domain.Convites;

namespace Project.IntegrationTests;

[Collection(PostgresCollection.Name)]
public sealed class ConviteFluxoTests : IAsyncLifetime
{
    private const string Password = "correta-senha";
    private readonly PostgresFixture _postgres;
    private GeventsFactory _factory = null!;

    public ConviteFluxoTests(PostgresFixture postgres)
    {
        _postgres = postgres;
    }

    public Task InitializeAsync()
    {
        _factory = new GeventsFactory(_postgres.ConnectionString);
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task Convite_publico_grava_a_primeira_presenca_e_isola_o_link()
    {
        var dono = await ConfirmedClientAsync(NewEmail());
        var grupo = Nome("Padrinhos");
        await PostAsync(dono, "/Grupos", new Dictionary<string, string> { ["Nome"] = grupo });
        var evento = Nome("Casamento");
        var eventoId = await CriarEventoAsync(dono, evento);
        var convite = Nome("Mesa da familia");
        await PostAsync(dono, $"/Eventos/Convites/{eventoId}?handler=Convite", new Dictionary<string, string>
        {
            ["nomeConvite"] = convite
        });

        var painel = await HtmlAsync(await dono.GetAsync($"/Eventos/Convites/{eventoId}"));
        var link = Regex.Match(painel, "/Convite/([0-9a-fA-F-]{36})").Groups[1].Value;
        var conviteId = Regex.Match(painel, $"value=\"([0-9a-fA-F-]{{36}})\">{Regex.Escape(convite)}").Groups[1].Value;
        var grupoId = Regex.Match(painel, $"value=\"([0-9a-fA-F-]{{36}})\">{Regex.Escape(grupo)}").Groups[1].Value;
        Assert.True(Guid.TryParse(link, out _));

        var anonimo = _factory.CreateClient();
        var vazio = await HtmlAsync(await anonimo.GetAsync($"/Convite/{link}"));
        Assert.Contains("Não há quem responder.", vazio);
        Assert.Contains("Acessada", await HtmlAsync(await dono.GetAsync($"/Eventos/Convites/{eventoId}")));

        var recusado = Nome("Sem origem");
        var semOrigem = await PostAsync(dono, $"/Eventos/Convites/{eventoId}?handler=Convidado", new Dictionary<string, string>
        {
            ["input.ConviteId"] = conviteId,
            ["input.Nome"] = recusado,
            ["input.Transfer"] = "true"
        });
        Assert.Contains("Informe o local de origem do transfer.", await HtmlAsync(semOrigem));
        Assert.DoesNotContain(recusado, await HtmlAsync(await dono.GetAsync($"/Eventos/Convites/{eventoId}")));

        var ana = Nome("Ana");
        var bruno = Nome("Bruno");
        await PostAsync(dono, $"/Eventos/Convites/{eventoId}?handler=Convidado", new Dictionary<string, string>
        {
            ["input.ConviteId"] = conviteId,
            ["input.Nome"] = ana,
            ["input.Faixa"] = nameof(FaixaConvidado.Adulto),
            ["input.Custo"] = nameof(CustoConvidado.Inteira),
            ["input.Celular"] = "119111",
            ["input.GrupoId"] = grupoId,
            ["input.Observacao"] = "mesa alta",
            ["input.Transfer"] = "true",
            ["input.Origem"] = "Hotel"
        });
        await PostAsync(dono, $"/Eventos/Convites/{eventoId}?handler=Convidado", new Dictionary<string, string>
        {
            ["input.ConviteId"] = conviteId,
            ["input.Nome"] = bruno,
            ["input.Faixa"] = nameof(FaixaConvidado.Crianca),
            ["input.Custo"] = nameof(CustoConvidado.Gratuita)
        });

        var lista = await HtmlAsync(await dono.GetAsync($"/Eventos/Convites/{eventoId}?visao=convidados"));
        Assert.Contains("Pendentes: 2", lista);
        Assert.Contains("Adulto: 1", lista);
        Assert.Contains("Criança: 1", lista);
        Assert.Contains("Inteira: 1", lista);
        Assert.Contains("Gratuita: 1", lista);
        Assert.Contains(ana, lista);
        Assert.Contains(bruno, lista);

        var porGrupo = await HtmlAsync(await dono.GetAsync($"/Eventos/Convites/{eventoId}?visao=convidados&grupo={grupoId}&situacao=Pendente"));
        Assert.Contains(ana, porGrupo);
        Assert.DoesNotContain(bruno, porGrupo);

        var publica = await HtmlAsync(await anonimo.GetAsync($"/Convite/{link}"));
        Assert.Contains(ana, publica);
        Assert.Contains(bruno, publica);
        Assert.DoesNotContain("Inteira", publica);
        Assert.DoesNotContain("Hotel", publica);
        Assert.Contains("Entrar", await HtmlAsync(await anonimo.GetAsync("/Eventos")));

        var interrompida = await PostPublicoAsync(anonimo, link, publica, new Dictionary<string, string> { [ana] = "Confirmado" });
        Assert.Contains("Não foi possível salvar a resposta.", await HtmlAsync(interrompida));
        Assert.Contains("Pendentes: 2", await HtmlAsync(await dono.GetAsync($"/Eventos/Convites/{eventoId}")));

        publica = await HtmlAsync(await anonimo.GetAsync($"/Convite/{link}"));
        var enviada = await PostPublicoAsync(anonimo, link, publica, new Dictionary<string, string>
        {
            [ana] = nameof(SituacaoConvidado.Confirmado),
            [bruno] = nameof(SituacaoConvidado.NaoComparecera)
        });
        var enviadaHtml = await HtmlAsync(enviada);
        Assert.Contains("Resposta registrada.", enviadaHtml);
        Assert.Contains($"{ana} — Confirmado", enviadaHtml);
        Assert.Contains($"{bruno} — Não comparecerá", enviadaHtml);

        var home = await HtmlAsync(await dono.GetAsync($"/Eventos/Home/{eventoId}"));
        Assert.Contains("Convidados confirmados: 1", home);
        Assert.Contains("Não comparecerá: 1", home);
        Assert.Contains("Respondida", await HtmlAsync(await dono.GetAsync($"/Eventos/Convites/{eventoId}")));

        var deNovo = await PostPublicoAsync(anonimo, link, enviadaHtml, new Dictionary<string, string>
        {
            [ana] = nameof(SituacaoConvidado.NaoComparecera),
            [bruno] = nameof(SituacaoConvidado.Confirmado)
        });
        var mantida = await HtmlAsync(deNovo);
        Assert.Contains("A resposta já foi registrada.", mantida);
        Assert.Contains($"{ana} — Confirmado", mantida);
        Assert.DoesNotContain($"{ana} — Não comparecerá", mantida);

        var alterado = await anonimo.GetAsync($"/Convite/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, alterado.StatusCode);
        Assert.Contains("O convite não está disponível.", await HtmlAsync(alterado));
        Assert.DoesNotContain(ana, await HtmlAsync(alterado));

        var outro = await ConfirmedClientAsync(NewEmail());
        var negado = await outro.GetAsync($"/Eventos/Convites/{eventoId}");
        Assert.Equal(HttpStatusCode.NotFound, negado.StatusCode);
        Assert.DoesNotContain(ana, await HtmlAsync(negado));
        Assert.DoesNotContain(convite, await HtmlAsync(await outro.GetAsync("/Grupos")));
    }

    private static async Task<HttpResponseMessage> PostPublicoAsync(
        HttpClient client,
        string link,
        string html,
        Dictionary<string, string> presencas)
    {
        var ids = Regex.Matches(html, "name=\"respostas\\[(\\d+)\\]\\.ConvidadoId\" value=\"([^\"]+)\"");
        var nomes = Regex.Matches(html, "<legend>([^<]+)</legend>");
        var fields = new Dictionary<string, string> { ["__RequestVerificationToken"] = Token(html) };
        for (var indice = 0; indice < nomes.Count; indice++)
        {
            var nome = nomes[indice].Groups[1].Value;
            if (!presencas.TryGetValue(nome, out var situacao))
            {
                continue;
            }

            var id = ids.First(item => item.Groups[1].Value == indice.ToString()).Groups[2].Value;
            fields[$"respostas[{indice}].ConvidadoId"] = id;
            fields[$"respostas[{indice}].Situacao"] = situacao;
        }

        return await client.PostAsync($"/Convite/{link}", new FormUrlEncodedContent(fields));
    }

    private static async Task<Guid> CriarEventoAsync(HttpClient client, string nome)
    {
        var resposta = await PostAsync(client, "/Eventos/Criar", new Dictionary<string, string> { ["Input.Nome"] = nome });
        return Guid.Parse(resposta.RequestMessage!.RequestUri!.Segments.Last().Trim('/'));
    }

    private async Task<HttpClient> ConfirmedClientAsync(string email)
    {
        var client = _factory.CreateClient();
        await PostAsync(client, "/Account/Register", new Dictionary<string, string>
        {
            ["Input.Email"] = email,
            ["Input.Password"] = Password
        });
        var link = _factory.Services.GetRequiredService<FakeEmailSender>().Sent.Last(sent => sent.Email == email).Payload;
        await client.GetAsync(new Uri(link).PathAndQuery);
        await PostAsync(client, "/Account/Login", new Dictionary<string, string>
        {
            ["Input.Email"] = email,
            ["Input.Password"] = Password
        });
        return client;
    }

    private static async Task<HttpResponseMessage> PostAsync(HttpClient client, string path, Dictionary<string, string> fields)
    {
        if (!fields.ContainsKey("__RequestVerificationToken"))
        {
            var html = await HtmlAsync(await client.GetAsync(path.Split('?')[0]));
            fields["__RequestVerificationToken"] = Token(html);
        }

        return await client.PostAsync(path, new FormUrlEncodedContent(fields));
    }

    private static async Task<string> HtmlAsync(HttpResponseMessage response)
    {
        return WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
    }

    private static string Token(string html)
    {
        var match = Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
        if (!match.Success)
        {
            match = Regex.Match(html, "value=\"([^\"]+)\"[^>]*name=\"__RequestVerificationToken\"");
        }

        return match.Groups[1].Value;
    }

    private static string NewEmail()
    {
        return $"convite-{Guid.NewGuid():N}@example.com";
    }

    private static string Nome(string prefixo)
    {
        return $"{prefixo} {Guid.NewGuid():N}";
    }
}
