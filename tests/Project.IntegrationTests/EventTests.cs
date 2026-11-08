using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Project.Domain;
using Project.Infrastructure.Arquivos;
using Project.Infrastructure.Persistence;

namespace Project.IntegrationTests;

[Collection(PostgresCollection.Name)]
public sealed class EventTests : IAsyncLifetime
{
    private const string Password = "correta-senha";
    private readonly PostgresFixture _postgres;
    private GeventsFactory _factory = null!;

    public EventTests(PostgresFixture postgres)
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
    public async Task Visitante_nao_ve_a_lista()
    {
        var body = await HtmlAsync(await _factory.CreateClient().GetAsync("/Eventos"));

        Assert.Contains("Entrar", body);
    }

    [Fact]
    public async Task Criar_com_nome_fica_em_andamento_e_sem_nome_nao_grava()
    {
        var client = await ConfirmedClientAsync(NewEmail());
        var nome = NomeUnico("Ana");

        var criado = await PostEventoAsync(client, "/Eventos/Criar", new Dictionary<string, string>
        {
            ["Input.Nome"] = nome
        });
        var home = await HtmlAsync(criado);
        Assert.Contains(nome, home);
        Assert.Contains("Em andamento", home);
        Assert.Contains("Tarefas concluídas: 0", home);
        Assert.Contains("Convidados confirmados: 0", home);
        Assert.DoesNotContain("Modelo", await HtmlAsync(await client.GetAsync("/Eventos/Criar")));

        var local = NomeUnico("local");
        var recusado = await PostEventoAsync(client, "/Eventos/Criar", new Dictionary<string, string>
        {
            ["Input.Nome"] = "   ",
            ["Input.LocalCerimonia"] = local
        });
        Assert.Contains("Informe o nome do evento.", await HtmlAsync(recusado));
        Assert.DoesNotContain(local, await HtmlAsync(await client.GetAsync("/Eventos")));
    }

    [Fact]
    public async Task Busca_tipo_periodo_e_tag_encontram_so_o_que_bate()
    {
        var client = await ConfirmedClientAsync(NewEmail());
        var casamento = NomeUnico("Casamento da Ana");
        var aniversario = NomeUnico("Aniversario da Bia");
        var semData = NomeUnico("Sem data");
        await PostEventoAsync(client, "/Eventos/Criar", Campos(casamento, "Casamento", "2026-08-01", "assessoria completa"));
        await PostEventoAsync(client, "/Eventos/Criar", Campos(aniversario, "Aniversario", "2026-01-15", "parcial"));
        await PostEventoAsync(client, "/Eventos/Criar", Campos(semData, "Formatura", null, "final"));

        var porNome = await HtmlAsync(await client.GetAsync("/Eventos?nome=" + Uri.EscapeDataString("ana")));
        Assert.Contains(casamento, porNome);
        Assert.DoesNotContain(aniversario, porNome);

        var porTipo = await HtmlAsync(await client.GetAsync("/Eventos?tipo=Aniversario"));
        Assert.Contains(aniversario, porTipo);
        Assert.DoesNotContain(casamento, porTipo);

        var porPeriodo = await HtmlAsync(await client.GetAsync("/Eventos?de=2026-06-01&ate=2026-12-31"));
        Assert.Contains(casamento, porPeriodo);
        Assert.DoesNotContain(aniversario, porPeriodo);
        Assert.DoesNotContain(semData, porPeriodo);

        var porTag = await HtmlAsync(await client.GetAsync("/Eventos?tag=" + Uri.EscapeDataString("Assessoria Completa")));
        Assert.Contains(casamento, porTag);
        Assert.DoesNotContain(aniversario, porTag);
    }

    [Fact]
    public async Task Outra_conta_nao_ve_evento_nem_imagem()
    {
        var dono = await ConfirmedClientAsync(NewEmail());
        var nome = NomeUnico("Privado");
        var criado = await PostEventoAsync(dono, "/Eventos/Criar", new Dictionary<string, string>
        {
            ["Input.Nome"] = nome
        }, (Png(), "foto.png"));
        var id = IdDaHome(criado);
        Assert.Contains("Imagem", await HtmlAsync(criado));

        var imagem = await dono.GetAsync($"/Eventos/Imagem/{id}");
        Assert.Equal(HttpStatusCode.OK, imagem.StatusCode);
        Assert.Equal("image/png", imagem.Content.Headers.ContentType?.MediaType);
        Assert.Equal("nosniff", imagem.Headers.GetValues("X-Content-Type-Options").Single());
        Assert.Equal("attachment", imagem.Content.Headers.ContentDisposition?.DispositionType);
        Assert.Equal(Png(), await imagem.Content.ReadAsByteArrayAsync());

        var outro = await ConfirmedClientAsync(NewEmail());
        var home = await outro.GetAsync($"/Eventos/Home/{id}");
        Assert.Equal(HttpStatusCode.NotFound, home.StatusCode);
        var homeHtml = await HtmlAsync(home);
        Assert.Contains("O evento não está disponível.", homeHtml);
        Assert.DoesNotContain(nome, homeHtml);

        var imagemAlheia = await outro.GetAsync($"/Eventos/Imagem/{id}");
        Assert.Equal(HttpStatusCode.NotFound, imagemAlheia.StatusCode);
        Assert.Contains("O evento não está disponível.", await HtmlAsync(imagemAlheia));
        Assert.Contains("Entrar", await HtmlAsync(await _factory.CreateClient().GetAsync($"/Eventos/Imagem/{id}")));
    }

    [Fact]
    public async Task Concluir_cancelar_e_desistir()
    {
        var client = await ConfirmedClientAsync(NewEmail());
        var ficar = NomeUnico("Fica");
        var concluir = NomeUnico("Fecha");
        var cancelar = NomeUnico("Cancela");
        var idFicar = IdDaHome(await PostEventoAsync(client, "/Eventos/Criar", new Dictionary<string, string> { ["Input.Nome"] = ficar }));
        var idConcluir = IdDaHome(await PostEventoAsync(client, "/Eventos/Criar", new Dictionary<string, string> { ["Input.Nome"] = concluir }));
        var idCancelar = IdDaHome(await PostEventoAsync(client, "/Eventos/Criar", new Dictionary<string, string> { ["Input.Nome"] = cancelar }));

        Assert.Contains("Concluir este evento?", await HtmlAsync(await client.GetAsync($"/Eventos/Home/{idFicar}?confirmar=concluir")));
        var depoisDeDesistir = await HtmlAsync(await client.GetAsync($"/Eventos/Home/{idFicar}"));
        Assert.Contains("Em andamento", depoisDeDesistir);
        Assert.Contains(ficar, await HtmlAsync(await client.GetAsync("/Eventos")));

        await PostConfirmacaoAsync(client, idConcluir, "Concluir");
        Assert.Contains(concluir, await HtmlAsync(await client.GetAsync("/Eventos?aba=concluido")));
        Assert.DoesNotContain(concluir, await HtmlAsync(await client.GetAsync("/Eventos")));

        await PostConfirmacaoAsync(client, idCancelar, "Cancelar");
        Assert.Contains(cancelar, await HtmlAsync(await client.GetAsync("/Eventos?aba=cancelado")));

        var editado = await PostEventoAsync(client, $"/Eventos/Editar/{idConcluir}", new Dictionary<string, string>
        {
            ["Input.Nome"] = NomeUnico("Nao muda")
        });
        Assert.Contains("O evento não está em andamento.", await HtmlAsync(editado));
        Assert.Contains(concluir, await HtmlAsync(await client.GetAsync($"/Eventos/Home/{idConcluir}")));
    }

    [Fact]
    public async Task Imagem_invalida_nao_apaga_o_evento()
    {
        var client = await ConfirmedClientAsync(NewEmail());
        var grande = NomeUnico("Grande");
        var estranho = NomeUnico("Estranho");
        var vazio = NomeUnico("Vazio");

        var acima = await PostEventoAsync(client, "/Eventos/Criar", new Dictionary<string, string>
        {
            ["Input.Nome"] = grande
        }, (ImagemGrande(), "foto.jpg"));
        var acimaHtml = await HtmlAsync(acima);
        Assert.Contains(grande, acimaHtml);
        Assert.Contains("A imagem passa de 5 MB.", acimaHtml);
        Assert.DoesNotContain("Imagem</a>", acimaHtml);

        var naoImagem = await PostEventoAsync(client, "/Eventos/Criar", new Dictionary<string, string>
        {
            ["Input.Nome"] = estranho
        }, ("nao e imagem"u8.ToArray(), "nota.png"));
        var naoImagemHtml = await HtmlAsync(naoImagem);
        Assert.Contains(estranho, naoImagemHtml);
        Assert.Contains("O formato da imagem não é aceito.", naoImagemHtml);

        var arquivoVazio = await PostEventoAsync(client, "/Eventos/Criar", new Dictionary<string, string>
        {
            ["Input.Nome"] = vazio
        }, ([], "vazio.jpg"));
        var vazioHtml = await HtmlAsync(arquivoVazio);
        Assert.Contains(vazio, vazioHtml);
        Assert.Contains("O arquivo está vazio.", vazioHtml);
    }

    [Fact]
    public async Task Mudar_a_data_nao_mexe_nas_contagens()
    {
        var client = await ConfirmedClientAsync(NewEmail());
        var nome = NomeUnico("Data");
        var id = IdDaHome(await PostEventoAsync(client, "/Eventos/Criar", Campos(nome, "Casamento", "2026-01-01", null)));

        var editado = await PostEventoAsync(client, $"/Eventos/Editar/{id}", Campos(nome, "Casamento", "2026-05-02", null));
        var home = await HtmlAsync(editado);

        Assert.Contains("02/05/2026", home);
        Assert.Contains("Tarefas concluídas: 0", home);
        Assert.Contains("Convidados confirmados: 0", home);
        Assert.DoesNotContain("01/01/2026", home);
    }

    [Fact]
    public async Task Arquivo_rejeita_assinatura_incompativel_e_apaga_temporario_se_o_metadado_falha()
    {
        var root = Path.Combine(Path.GetTempPath(), "gevents-arquivo", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            var storage = new ArmazenamentoArquivo(
                new Ambiente(),
                Options.Create(new StorageOptions { Root = root }),
                new RepositorioQueFalha(),
                NullLogger<ArmazenamentoArquivo>.Instance);

            await using (var pngErrado = new MemoryStream(Jpeg()))
            {
                var erro = await Assert.ThrowsAsync<RegraDeNegocioException>(() => storage.SalvarImagemAsync(pngErrado, "foto.png", CancellationToken.None));
                Assert.Equal("O formato da imagem não é aceito.", erro.Message);
            }

            await using var png = new MemoryStream(Png());
            await Assert.ThrowsAsync<InvalidOperationException>(() => storage.SalvarImagemAsync(png, "foto.png", CancellationToken.None));
            Assert.Empty(Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories));
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, true);
            }
        }
    }

    [Fact]
    public async Task Nome_original_nao_escolhe_o_caminho_do_volume()
    {
        var client = await ConfirmedClientAsync(NewEmail());
        var nome = NomeUnico("Travessia");
        var criado = await PostEventoAsync(client, "/Eventos/Criar", new Dictionary<string, string>
        {
            ["Input.Nome"] = nome
        }, (Jpeg(), "..\\..\\segredo.jpg"));
        var id = IdDaHome(criado);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var evento = await db.Eventos.SingleAsync(item => item.Id == id);
        var arquivo = await db.Arquivos.SingleAsync(item => item.Id == evento.ArquivoId);
        var root = scope.ServiceProvider.GetRequiredService<IOptions<StorageOptions>>().Value.Root;
        var fisico = Path.GetFullPath(Path.Combine(root, arquivo.CaminhoRelativo.Replace('/', Path.DirectorySeparatorChar)));

        Assert.DoesNotContain("..", arquivo.NomeOriginal);
        Assert.DoesNotContain("/", arquivo.NomeOriginal);
        Assert.DoesNotContain("\\", arquivo.NomeOriginal);
        Assert.StartsWith("images/", arquivo.CaminhoRelativo.Replace('\\', '/'));
        Assert.StartsWith(Path.GetFullPath(root), fisico, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("segredo", Path.GetFileName(fisico), StringComparison.OrdinalIgnoreCase);
        Assert.True(File.Exists(fisico));

        var resposta = await client.GetAsync($"/Eventos/Imagem/{id}");
        Assert.Equal("image/jpeg", resposta.Content.Headers.ContentType?.MediaType);
        Assert.DoesNotContain(root, resposta.Content.Headers.ContentDisposition?.ToString());
    }

    private async Task<HttpClient> ConfirmedClientAsync(string email)
    {
        var client = _factory.CreateClient();
        await PostFormAsync(client, "/Account/Register", new Dictionary<string, string>
        {
            ["Input.Email"] = email,
            ["Input.Password"] = Password
        });
        var link = _factory.Services.GetRequiredService<FakeEmailSender>().Sent.Last(sent => sent.Email == email).Payload;
        await client.GetAsync(new Uri(link).PathAndQuery);
        await PostFormAsync(client, "/Account/Login", new Dictionary<string, string>
        {
            ["Input.Email"] = email,
            ["Input.Password"] = Password
        });
        return client;
    }

    private static Dictionary<string, string> Campos(string nome, string tipo, string? data, string? tag)
    {
        var campos = new Dictionary<string, string>
        {
            ["Input.Nome"] = nome,
            ["Input.Tipo"] = tipo
        };
        if (data is not null)
        {
            campos["Input.Data"] = data;
        }

        if (tag is not null)
        {
            campos["Input.Tags"] = tag;
        }

        return campos;
    }

    private static async Task<HttpResponseMessage> PostConfirmacaoAsync(HttpClient client, Guid id, string handler)
    {
        var html = await HtmlAsync(await client.GetAsync($"/Eventos/Home/{id}?confirmar={handler.ToLowerInvariant()}"));
        return await client.PostAsync($"/Eventos/Home/{id}?handler={handler}", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = Token(html),
            ["confirmado"] = "sim"
        }));
    }

    private static async Task<HttpResponseMessage> PostEventoAsync(
        HttpClient client,
        string path,
        Dictionary<string, string> fields,
        (byte[] Bytes, string FileName)? arquivo = null)
    {
        var html = await HtmlAsync(await client.GetAsync(path));
        var content = new MultipartFormDataContent();
        content.Add(new StringContent(Token(html)), "__RequestVerificationToken");
        foreach (var field in fields)
        {
            content.Add(new StringContent(field.Value), field.Key);
        }

        if (arquivo is not null)
        {
            var file = new ByteArrayContent(arquivo.Value.Bytes);
            file.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            content.Add(file, "Input.Imagem", arquivo.Value.FileName);
        }

        return await client.PostAsync(path, content);
    }

    private static async Task<HttpResponseMessage> PostFormAsync(HttpClient client, string path, Dictionary<string, string> fields)
    {
        var html = await HtmlAsync(await client.GetAsync(path));
        fields["__RequestVerificationToken"] = Token(html);
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

    private static Guid IdDaHome(HttpResponseMessage response)
    {
        return Guid.Parse(response.RequestMessage!.RequestUri!.Segments.Last().Trim('/'));
    }

    private static string NewEmail()
    {
        return $"evento-{Guid.NewGuid():N}@example.com";
    }

    private static string NomeUnico(string prefixo)
    {
        return $"{prefixo} {Guid.NewGuid():N}";
    }

    private static byte[] Png()
    {
        return [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00];
    }

    private static byte[] Jpeg()
    {
        return [0xFF, 0xD8, 0xFF, 0xE0];
    }

    private static byte[] ImagemGrande()
    {
        var bytes = new byte[(5 * 1024 * 1024) + 1];
        bytes[0] = 0xFF;
        bytes[1] = 0xD8;
        bytes[2] = 0xFF;
        return bytes;
    }

    private sealed class RepositorioQueFalha : IArquivoMetadadoRepositorio
    {
        public Task AdicionarAsync(ArquivoMetadado metadado, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("banco");
        }

        public Task<ArquivoMetadado?> ObterAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult<ArquivoMetadado?>(null);
        }

        public Task RemoverAsync(ArquivoMetadado metadado, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Descartar(ArquivoMetadado metadado)
        {
        }
    }

    private sealed class Ambiente : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Testing";

        public string ApplicationName { get; set; } = "tests";

        public string ContentRootPath { get; set; } = Path.GetTempPath();

        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
