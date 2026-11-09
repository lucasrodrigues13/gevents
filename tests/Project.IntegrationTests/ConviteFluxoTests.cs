using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Project.Domain.Convites;
using Project.Infrastructure.Convites;

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
        var semConvidado = await HtmlAsync(await PostAsync(dono, $"/Eventos/Convites/{eventoId}?handler=Convite", new Dictionary<string, string>
        {
            ["nomeConvite"] = convite
        }));
        Assert.Contains("Informe o nome do convidado.", semConvidado);
        Assert.DoesNotContain(convite, semConvidado);
        var grupoId = Regex.Match(semConvidado, $"value=\"([0-9a-fA-F-]{{36}})\">{Regex.Escape(grupo)}").Groups[1].Value;

        var ana = Nome("Ana");
        var bruno = Nome("Bruno");
        await PostAsync(dono, $"/Eventos/Convites/{eventoId}?handler=Convite", new Dictionary<string, string>
        {
            ["nomeConvite"] = convite,
            ["convidado.Nome"] = ana,
            ["convidado.Faixa"] = nameof(FaixaConvidado.Adulto),
            ["convidado.Custo"] = nameof(CustoConvidado.Inteira),
            ["convidado.Celular"] = "119111",
            ["convidado.GrupoId"] = grupoId,
            ["convidado.Observacao"] = "mesa alta",
            ["convidado.Transfer"] = "true",
            ["convidado.Origem"] = "Hotel"
        });

        var painel = await HtmlAsync(await dono.GetAsync($"/Eventos/Convites/{eventoId}"));
        var link = Regex.Match(painel, "/Convite/([0-9a-fA-F-]{36})").Groups[1].Value;
        var conviteId = Regex.Match(painel, $"value=\"([0-9a-fA-F-]{{36}})\">{Regex.Escape(convite)}").Groups[1].Value;
        Assert.True(Guid.TryParse(link, out _));

        var anonimo = _factory.CreateClient();
        var primeira = await HtmlAsync(await anonimo.GetAsync($"/Convite/{link}"));
        Assert.Contains(ana, primeira);
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

    [Fact]
    public async Task Principal_transfer_planilha_e_qr_ficam_na_conta()
    {
        var dono = await ConfirmedClientAsync(NewEmail());
        var eventoId = await CriarEventoAsync(dono, Nome("Festa"));
        var antes = await HtmlAsync(await dono.GetAsync($"/Eventos/Convites/{eventoId}"));
        Assert.DoesNotContain("Origem do transfer", antes);

        await PostAsync(dono, $"/Eventos/Convites/{eventoId}?handler=Transfer", new Dictionary<string, string> { ["ativo"] = "true" });
        var local = Nome("Rodoviaria");
        await PostAsync(dono, $"/Eventos/Convites/{eventoId}?handler=Local", new Dictionary<string, string> { ["nomeLocal"] = local });

        var ana = Nome("Ana");
        var bruno = Nome("Bruno");
        var convite = Nome("Familia");
        await PostAsync(dono, $"/Eventos/Convites/{eventoId}?handler=Convite", new Dictionary<string, string>
        {
            ["nomeConvite"] = convite,
            ["convidado.Nome"] = ana,
            ["convidado.Ddi"] = "+55",
            ["convidado.Celular"] = "119000111"
        });
        var painel = await HtmlAsync(await dono.GetAsync($"/Eventos/Convites/{eventoId}"));
        Assert.Contains("Origem do transfer", painel);
        Assert.DoesNotContain("Telefone do convite:", painel);
        var conviteId = Regex.Match(painel, $"value=\"([0-9a-fA-F-]{{36}})\">{Regex.Escape(convite)}").Groups[1].Value;
        await PostAsync(dono, $"/Eventos/Convites/{eventoId}?handler=Convidado", new Dictionary<string, string>
        {
            ["input.ConviteId"] = conviteId,
            ["input.Nome"] = bruno,
            ["input.Ddi"] = "+55",
            ["input.Celular"] = "119000222"
        });

        painel = await HtmlAsync(await dono.GetAsync($"/Eventos/Convites/{eventoId}"));
        var anaId = Regex.Match(painel, "value=\"([0-9a-fA-F-]{36})\"[^>]*>119000111").Groups[1].Value;
        var brunoId = Regex.Match(painel, "value=\"([0-9a-fA-F-]{36})\"[^>]*>119000222").Groups[1].Value;
        await PostAsync(dono, $"/Eventos/Convites/{eventoId}?handler=Principal", new Dictionary<string, string>
        {
            ["conviteId"] = conviteId,
            ["convidadoId"] = anaId
        });
        var escolhido = await HtmlAsync(await dono.GetAsync($"/Eventos/Convites/{eventoId}"));
        Assert.Contains("Telefone do convite: +55 119000111", escolhido);
        Assert.Contains($"{ana} — Principal", escolhido);

        await PostAsync(dono, $"/Eventos/Convites/{eventoId}?handler=Principal", new Dictionary<string, string>
        {
            ["conviteId"] = conviteId,
            ["convidadoId"] = brunoId
        });
        var trocado = await HtmlAsync(await dono.GetAsync($"/Eventos/Convites/{eventoId}"));
        Assert.Contains("Telefone do convite: +55 119000222", trocado);
        Assert.DoesNotContain("Telefone do convite: +55 119000111", trocado);
        Assert.Contains($"{bruno} — Principal", trocado);
        Assert.DoesNotContain($"{ana} — Principal", trocado);

        var edicao = await HtmlAsync(await dono.GetAsync($"/Eventos/Convites/Editar/{eventoId}/{conviteId}"));
        Assert.Contains("Histórico de mensagens", edicao);
        Assert.Contains("QR Code", edicao);
        Assert.Contains("Baixar", edicao);
        var telefone = Regex.Matches(edicao, "name=\"convidadoId\" value=\"([^\"]+)\"[\\s\\S]*?name=\"celular\" value=\"([^\"]*)\"")
            .First(item => item.Groups[2].Value == "119000222");
        await PostAsync(dono, $"/Eventos/Convites/Editar/{eventoId}/{conviteId}?handler=Telefone", new Dictionary<string, string>
        {
            ["convidadoId"] = telefone.Groups[1].Value,
            ["ddi"] = "+55",
            ["celular"] = "119000333"
        });
        Assert.Contains(
            "Telefone do convite: +55 119000333",
            await HtmlAsync(await dono.GetAsync($"/Eventos/Convites/{eventoId}")));

        var semLocal = await PostAsync(dono, $"/Eventos/Convites/{eventoId}?handler=Origem", new Dictionary<string, string>
        {
            ["conviteId"] = conviteId,
            ["usa"] = "sim"
        });
        Assert.Contains("Informe o local de origem do transfer.", await HtmlAsync(semLocal));
        var localId = Regex.Match(await HtmlAsync(semLocal), $"value=\"([0-9a-fA-F-]{{36}})\">{Regex.Escape(local)}").Groups[1].Value;
        await PostAsync(dono, $"/Eventos/Convites/{eventoId}?handler=Origem", new Dictionary<string, string>
        {
            ["conviteId"] = conviteId,
            ["usa"] = "sim",
            ["localId"] = localId
        });
        Assert.Contains($"Origem: {local}", await HtmlAsync(await dono.GetAsync($"/Eventos/Convites/{eventoId}")));

        var modelo = await dono.GetAsync($"/Eventos/Convites/{eventoId}?handler=Planilha");
        var bytes = await modelo.Content.ReadAsByteArrayAsync();
        using (var zip = new ZipArchive(new MemoryStream(bytes)))
        {
            using var leitura = new StreamReader(zip.GetEntry("xl/worksheets/sheet1.xml")!.Open());
            var xml = await leitura.ReadToEndAsync();
            foreach (var coluna in ImportacaoConvites.Cabecalho)
            {
                Assert.Contains(coluna, xml);
            }

            Assert.Single(Regex.Matches(xml, "<row "));
        }

        var rejeitado = Nome("Rejeitado");
        var invalido = await PostArquivoAsync(
            dono,
            $"/Eventos/Convites/{eventoId}?handler=Importar",
            "nao e planilha"u8.ToArray(),
            "pessoas.txt");
        var invalidoHtml = await HtmlAsync(invalido);
        Assert.Contains("A planilha não é aceita.", invalidoHtml);
        Assert.DoesNotContain(rejeitado, invalidoHtml);

        var ilegivel = Nome("Ilegivel");
        var ruim = new LeitorPlanilhaConvite().Criar(
        [
            ImportacaoConvites.Cabecalho,
            [convite, "+55", "119777", "", "", ilegivel, "Z", "Adulto", "Inteira", "Pendente", "1"]
        ]);
        var genero = await PostArquivoAsync(dono, $"/Eventos/Convites/{eventoId}?handler=Importar", ruim, "convidados.xlsx");
        Assert.Contains("O valor informado não é aceito.", await HtmlAsync(genero));
        Assert.DoesNotContain(ilegivel, await HtmlAsync(await dono.GetAsync($"/Eventos/Convites/{eventoId}")));

        var importado = Nome("Importado");
        var carla = Nome("Carla");
        var diego = Nome("Diego");
        var grupo = Nome("Amigos");
        var mesa = Nome("Mesa");
        var planilha = new LeitorPlanilhaConvite().Criar(
        [
            ImportacaoConvites.Cabecalho,
            [importado, "+55", "119888", grupo, "janela", carla, "F", "Adulto", "Free", "Confirmado", mesa],
            ["", "", "", "", "", diego, "M", "Criança", "Inteira", "Pendente", mesa]
        ]);
        await PostArquivoAsync(dono, $"/Eventos/Convites/{eventoId}?handler=Importar", planilha, "convidados.xlsx");
        var lista = await HtmlAsync(await dono.GetAsync($"/Eventos/Convites/{eventoId}?visao=convidados"));
        Assert.Contains(carla, lista);
        Assert.Contains(diego, lista);
        Assert.Contains("Feminino", lista);
        Assert.Contains("Masculino", lista);
        Assert.Contains("Gratuita", lista);
        Assert.Contains(mesa, lista);
        Assert.Contains(grupo, lista);
        var convites = await HtmlAsync(await dono.GetAsync($"/Eventos/Convites/{eventoId}"));
        Assert.Contains("Telefone do convite: +55 119888", convites);
        Assert.Contains($"{carla} — Principal", convites);

        var linkImportado = Regex.Match(convites[convites.IndexOf(importado, StringComparison.Ordinal)..], "/Convite/([0-9a-fA-F-]{36})").Groups[1].Value;
        var anonimo = _factory.CreateClient();
        var publica = await HtmlAsync(await anonimo.GetAsync($"/Convite/{linkImportado}"));
        Assert.Contains(carla, publica);
        Assert.Contains(diego, publica);
        Assert.DoesNotContain(mesa, publica);
        Assert.DoesNotContain("Gratuita", publica);
        Assert.DoesNotContain("119888", publica);

        var qr = await dono.GetAsync($"/Eventos/Convites/Editar/{eventoId}/{conviteId}?handler=Qr&baixar=true");
        var png = await qr.Content.ReadAsByteArrayAsync();
        Assert.Equal(new byte[] { 0x89, 0x50, 0x4E, 0x47 }, png.Take(4));
        var whats = await PostAsync(dono, $"/Eventos/Convites/Editar/{eventoId}/{conviteId}?handler=WhatsApp", new Dictionary<string, string>
        {
            ["ddi"] = "+55",
            ["celular"] = "119000333",
            ["mensagem"] = "Seu convite"
        });
        Assert.Contains("https://wa.me/55119000333?text=Seu%20convite", await HtmlAsync(whats));

        var outro = await ConfirmedClientAsync(NewEmail());
        var negado = await outro.GetAsync($"/Eventos/Convites/{eventoId}");
        Assert.Equal(HttpStatusCode.NotFound, negado.StatusCode);
        Assert.DoesNotContain(carla, await HtmlAsync(negado));
        Assert.DoesNotContain(convite, await HtmlAsync(negado));
        var edicaoNegada = await outro.GetAsync($"/Eventos/Convites/Editar/{eventoId}/{conviteId}");
        Assert.Equal(HttpStatusCode.NotFound, edicaoNegada.StatusCode);
        Assert.DoesNotContain(ana, await HtmlAsync(edicaoNegada));
    }

    private static async Task<HttpResponseMessage> PostArquivoAsync(HttpClient client, string path, byte[] bytes, string nome)
    {
        var html = await HtmlAsync(await client.GetAsync(path.Split('?')[0]));
        var content = new MultipartFormDataContent();
        content.Add(new StringContent(Token(html)), "__RequestVerificationToken");
        var arquivo = new ByteArrayContent(bytes);
        arquivo.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        content.Add(arquivo, "arquivo", nome);
        return await client.PostAsync(path, content);
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
        var modelo = await PostAsync(client, "/Modelos/Criar", new Dictionary<string, string>
        {
            ["Nome"] = "Modelo " + Guid.NewGuid().ToString("N"),
            ["Tipo"] = "Aniversario"
        });
        var resposta = await PostAsync(client, "/Eventos/Criar", new Dictionary<string, string>
        {
            ["Input.Nome"] = nome,
            ["Input.PrimeiroNome"] = nome,
            ["Input.ModeloId"] = modelo.RequestMessage!.RequestUri!.Segments.Last().Trim('/')
        });
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
