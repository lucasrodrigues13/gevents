using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Project.Domain.Tarefas;

namespace Project.IntegrationTests;

[Collection(PostgresCollection.Name)]
public sealed class TarefaTests : IAsyncLifetime
{
    private const string Password = "correta-senha";
    private readonly PostgresFixture _postgres;
    private GeventsFactory _factory = null!;

    public TarefaTests(PostgresFixture postgres)
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
    public async Task Cria_lista_quadro_e_lista_geral_e_isola_a_conta()
    {
        var email = NewEmail();
        var client = await ConfirmedClientAsync(email);
        var evento = Nome("Evento Alfa");
        var outroEvento = Nome("Evento Beta");
        var titulo = Nome("Levar documento");
        var segundo = Nome("Reservar salao");
        var eventoId = await CriarEventoAsync(client, evento);
        var outroId = await CriarEventoAsync(client, outroEvento);

        var semPrazo = await PostAsync(client, $"/Eventos/Tarefas/{eventoId}", new Dictionary<string, string>
        {
            ["Input.Titulo"] = titulo,
            ["Input.Descricao"] = "texto da assessoria"
        });
        var lista = await HtmlAsync(semPrazo);
        Assert.Contains(titulo, lista);
        Assert.Contains("texto da assessoria", lista);
        Assert.Contains("Pendente", lista);
        Assert.Contains(email, lista);
        Assert.Contains(titulo, Coluna(lista, nameof(SituacaoTarefa.Pendente)));
        Assert.DoesNotContain("Prazo relativo", lista);

        await PostAsync(client, $"/Eventos/Tarefas/{outroId}", new Dictionary<string, string>
        {
            ["Input.Titulo"] = segundo,
            ["Input.Prazo"] = "2026-03-10"
        });
        var geral = await HtmlAsync(await client.GetAsync("/Tarefas"));
        Assert.Contains(titulo, geral);
        Assert.Contains(segundo, geral);
        Assert.Contains(evento, geral);
        Assert.Contains(outroEvento, geral);
        Assert.Contains("10/03/2026", geral);

        var tarefaId = IdDaTarefa(lista, titulo);
        foreach (var situacao in new[] { "EmAndamento", "Concluida", "Atrasada", "Pendente" })
        {
            lista = await HtmlAsync(await PostAsync(client, $"/Eventos/Tarefas/Editar/{tarefaId}", new Dictionary<string, string>
            {
                ["Input.Titulo"] = titulo,
                ["Input.Descricao"] = "texto da assessoria",
                ["Input.Situacao"] = situacao
            }));
            Assert.Contains(titulo, Coluna(lista, situacao));
        }

        var home = await HtmlAsync(await client.GetAsync($"/Eventos/Home/{eventoId}"));
        Assert.Contains("Tarefas concluídas: 0", home);

        await PostAsync(client, $"/Eventos/Tarefas/Editar/{tarefaId}", new Dictionary<string, string>
        {
            ["Input.Titulo"] = titulo,
            ["Input.Situacao"] = "Concluida"
        });
        home = await HtmlAsync(await client.GetAsync($"/Eventos/Home/{eventoId}"));
        Assert.Contains("Tarefas concluídas: 1", home);
        Assert.Contains("Convidados confirmados: 0", home);

        var outro = await ConfirmedClientAsync(NewEmail());
        var negado = await outro.GetAsync($"/Eventos/Tarefas/{eventoId}");
        Assert.Equal(HttpStatusCode.NotFound, negado.StatusCode);
        var negadoHtml = await HtmlAsync(negado);
        Assert.Contains("O evento não está disponível.", negadoHtml);
        Assert.DoesNotContain(titulo, negadoHtml);
        Assert.DoesNotContain(titulo, await HtmlAsync(await outro.GetAsync("/Tarefas")));
        Assert.Contains("Entrar", await HtmlAsync(await _factory.CreateClient().GetAsync("/Tarefas")));
    }

    [Fact]
    public async Task Sem_titulo_nao_grava_descricao_e_prazo_sobrevivem_a_data_do_evento()
    {
        var client = await ConfirmedClientAsync(NewEmail());
        var evento = Nome("Evento Prazo");
        var titulo = Nome("Confirmar buffet");
        var eventoId = await CriarEventoAsync(client, evento, "2026-01-01");
        var marcador = Nome("nao gravar");

        var recusado = await PostAsync(client, $"/Eventos/Tarefas/{eventoId}", new Dictionary<string, string>
        {
            ["Input.Titulo"] = "   ",
            ["Input.Descricao"] = marcador
        });
        Assert.Contains("Informe o título da tarefa.", await HtmlAsync(recusado));
        Assert.DoesNotContain(marcador, await HtmlAsync(await client.GetAsync($"/Eventos/Tarefas/{eventoId}")));

        var criada = await HtmlAsync(await PostAsync(client, $"/Eventos/Tarefas/{eventoId}", new Dictionary<string, string>
        {
            ["Input.Titulo"] = titulo,
            ["Input.Descricao"] = "descricao preenchida",
            ["Input.Prazo"] = "2026-03-10"
        }));
        Assert.Contains("descricao preenchida", criada);
        var tarefaId = IdDaTarefa(criada, titulo);

        var semDescricao = await HtmlAsync(await PostAsync(client, $"/Eventos/Tarefas/Editar/{tarefaId}", new Dictionary<string, string>
        {
            ["Input.Titulo"] = titulo,
            ["Input.Descricao"] = "   ",
            ["Input.Prazo"] = "2026-03-10",
            ["Input.Situacao"] = "Pendente"
        }));
        Assert.DoesNotContain("descricao preenchida", semDescricao);
        Assert.Contains("10/03/2026", semDescricao);

        await PostAsync(client, $"/Eventos/Editar/{eventoId}", new Dictionary<string, string>
        {
            ["Input.Nome"] = evento,
            ["Input.Data"] = "2026-05-02"
        });
        var depois = await HtmlAsync(await client.GetAsync($"/Eventos/Tarefas/{eventoId}"));
        Assert.Contains("10/03/2026", depois);
        Assert.DoesNotContain("02/05/2026", depois);

        await PostAsync(client, $"/Eventos/Home/{eventoId}?handler=Concluir", await ConfirmacaoAsync(client, eventoId));
        var fechado = await PostAsync(client, $"/Eventos/Tarefas/{eventoId}", new Dictionary<string, string>
        {
            ["Input.Titulo"] = Nome("Depois de encerrar")
        });
        var fechadoHtml = await HtmlAsync(fechado);
        Assert.Contains("O evento não está em andamento.", fechadoHtml);
        Assert.DoesNotContain("Depois de encerrar", fechadoHtml);
    }

    private async Task<Dictionary<string, string>> ConfirmacaoAsync(HttpClient client, Guid eventoId)
    {
        var html = await HtmlAsync(await client.GetAsync($"/Eventos/Home/{eventoId}?confirmar=concluir"));
        return new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = Token(html),
            ["confirmado"] = "sim"
        };
    }

    private static async Task<Guid> CriarEventoAsync(HttpClient client, string nome, string? data = null)
    {
        var campos = new Dictionary<string, string> { ["Input.Nome"] = nome };
        if (data is not null)
        {
            campos["Input.Data"] = data;
        }

        var resposta = await PostAsync(client, "/Eventos/Criar", campos);
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

    private static Guid IdDaTarefa(string html, string titulo)
    {
        var match = Regex.Match(html, "Editar/([0-9a-fA-F-]{36})\"[^>]*>" + Regex.Escape(titulo));
        Assert.True(match.Success);
        return Guid.Parse(match.Groups[1].Value);
    }

    private static string Coluna(string html, string situacao)
    {
        var inicio = html.IndexOf($"id=\"coluna-{situacao}\"", StringComparison.Ordinal);
        Assert.True(inicio >= 0);
        var fim = html.IndexOf("</section>", inicio, StringComparison.Ordinal);
        return html[inicio..fim];
    }

    private static string NewEmail()
    {
        return $"tarefa-{Guid.NewGuid():N}@example.com";
    }

    private static string Nome(string prefixo)
    {
        return $"{prefixo} {Guid.NewGuid():N}";
    }
}
