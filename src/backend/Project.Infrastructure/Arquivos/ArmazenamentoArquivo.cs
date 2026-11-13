using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Project.Application.Arquivos;
using Project.Application.Comum;
using Project.Infrastructure.Persistencia;

namespace Project.Infrastructure.Arquivos;

public sealed class ArmazenamentoOpcoes
{
    public string Root { get; set; } = "storage";

    public long MaxImageBytes { get; set; } = 5 * 1024 * 1024;
}

public sealed class ArmazenamentoArquivo(
    AppDbContext banco,
    IOptions<ArmazenamentoOpcoes> opcoes,
    ILogger<ArmazenamentoArquivo> logs) : IArmazenamentoArquivo
{
    private const string Mensagem = "A imagem não foi aceita. Use JPG, PNG ou WebP de até 5 MB.";

    public async Task<Resultado<ArquivoGuardado>> GuardarImagemAsync(
        Stream conteudo,
        string? nomeOriginal,
        string contaId,
        CancellationToken ct)
    {
        var raiz = Raiz();
        var tempDir = Path.Combine(raiz, "temp");
        Directory.CreateDirectory(tempDir);
        var temp = Path.Combine(tempDir, Guid.NewGuid().ToString("N"));
        string? final = null;
        try
        {
            long total = 0;
            await using (var destino = new FileStream(temp, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true))
            {
                var buffer = new byte[81920];
                int lidos;
                while ((lidos = await conteudo.ReadAsync(buffer, ct)) > 0)
                {
                    total += lidos;
                    if (total > opcoes.Value.MaxImageBytes)
                    {
                        return Resultado<ArquivoGuardado>.Erro(Codigos.Validacao, Mensagem);
                    }

                    await destino.WriteAsync(buffer.AsMemory(0, lidos), ct);
                }
            }

            if (total == 0)
            {
                return Resultado<ArquivoGuardado>.Erro(Codigos.Validacao, Mensagem);
            }

            var tipo = Detectar(temp);
            if (tipo is null || !ExtensaoConfere(nomeOriginal, tipo.Value.Extensao))
            {
                return Resultado<ArquivoGuardado>.Erro(Codigos.Validacao, Mensagem);
            }

            var agora = DateTime.UtcNow;
            var nomeFisico = $"{Guid.NewGuid():N}{tipo.Value.Extensao}";
            var relativo = Path.Combine("images", agora.Year.ToString("0000"), agora.Month.ToString("00"), nomeFisico);
            final = Path.GetFullPath(Path.Combine(raiz, relativo));
            if (!final.StartsWith(raiz, StringComparison.OrdinalIgnoreCase))
            {
                return Resultado<ArquivoGuardado>.Erro(Codigos.Validacao, Mensagem);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(final)!);
            File.Move(temp, final, overwrite: false);
            temp = "";

            var registro = new ArquivoRegistro
            {
                Id = Guid.NewGuid(),
                ContaId = contaId,
                NomeOriginal = Sanitizar(nomeOriginal, tipo.Value.Extensao),
                TipoConteudo = tipo.Value.Mime,
                Tamanho = total,
                CaminhoRelativo = relativo.Replace('\\', '/'),
            };
            try
            {
                banco.Arquivos.Add(registro);
                await banco.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                logs.LogError(ex, "Falha ao gravar o metadado da imagem {ArquivoId}.", registro.Id);
                ApagarSilencioso(final);
                throw;
            }

            return Resultado<ArquivoGuardado>.Sucesso(new ArquivoGuardado(registro.Id, registro.NomeOriginal, registro.TipoConteudo));
        }
        finally
        {
            if (!string.IsNullOrEmpty(temp))
            {
                ApagarSilencioso(temp);
            }
        }
    }

    public async Task<ArquivoAberto?> AbrirAsync(Guid id, string contaId, CancellationToken ct)
    {
        var registro = await banco.Arquivos.AsNoTracking().FirstOrDefaultAsync(arquivo => arquivo.Id == id && arquivo.ContaId == contaId, ct);
        if (registro is null || registro.CaminhoRelativo.Contains("..", StringComparison.Ordinal))
        {
            return null;
        }

        var raiz = Raiz();
        var completo = Path.GetFullPath(Path.Combine(raiz, registro.CaminhoRelativo.Replace('/', Path.DirectorySeparatorChar)));
        if (!completo.StartsWith(raiz, StringComparison.OrdinalIgnoreCase) || !File.Exists(completo))
        {
            return null;
        }

        var fluxo = new FileStream(completo, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true);
        return new ArquivoAberto(registro.NomeOriginal, registro.TipoConteudo, fluxo);
    }

    public async Task DescartarAsync(Guid id, CancellationToken ct)
    {
        var registro = await banco.Arquivos.FirstOrDefaultAsync(arquivo => arquivo.Id == id, ct);
        if (registro is null)
        {
            return;
        }

        var raiz = Raiz();
        var completo = Path.GetFullPath(Path.Combine(raiz, registro.CaminhoRelativo.Replace('/', Path.DirectorySeparatorChar)));
        banco.Arquivos.Remove(registro);
        await banco.SaveChangesAsync(ct);
        if (completo.StartsWith(raiz, StringComparison.OrdinalIgnoreCase))
        {
            ApagarSilencioso(completo);
        }
    }

    private string Raiz()
    {
        var cheia = Path.GetFullPath(opcoes.Value.Root);
        return cheia.EndsWith(Path.DirectorySeparatorChar) ? cheia : cheia + Path.DirectorySeparatorChar;
    }

    private static (string Extensao, string Mime)? Detectar(string caminho)
    {
        Span<byte> cabecalho = stackalloc byte[16];
        using var leitura = File.OpenRead(caminho);
        var lidos = leitura.Read(cabecalho);
        if (lidos >= 3 && cabecalho[0] == 0xFF && cabecalho[1] == 0xD8 && cabecalho[2] == 0xFF)
        {
            return (".jpg", "image/jpeg");
        }

        if (lidos >= 8 && cabecalho[0] == 0x89 && cabecalho[1] == 0x50 && cabecalho[2] == 0x4E && cabecalho[3] == 0x47)
        {
            return (".png", "image/png");
        }

        if (lidos >= 12
            && cabecalho[0] == (byte)'R' && cabecalho[1] == (byte)'I' && cabecalho[2] == (byte)'F' && cabecalho[3] == (byte)'F'
            && cabecalho[8] == (byte)'W' && cabecalho[9] == (byte)'E' && cabecalho[10] == (byte)'B' && cabecalho[11] == (byte)'P')
        {
            return (".webp", "image/webp");
        }

        return null;
    }

    private static bool ExtensaoConfere(string? nomeOriginal, string extensaoDetectada)
    {
        var extensao = Path.GetExtension(Sanitizar(nomeOriginal, extensaoDetectada)).ToLowerInvariant();
        if (extensao is ".jpeg")
        {
            extensao = ".jpg";
        }

        return extensao == extensaoDetectada;
    }

    private static string Sanitizar(string? nomeOriginal, string extensao)
    {
        var nome = Path.GetFileName(nomeOriginal ?? "");
        var limpo = new string(nome.Where(caractere => !char.IsControl(caractere)).ToArray()).Replace("..", "", StringComparison.Ordinal);
        return string.IsNullOrWhiteSpace(limpo) ? $"imagem{extensao}" : limpo;
    }

    private static void ApagarSilencioso(string caminho)
    {
        try
        {
            if (File.Exists(caminho))
            {
                File.Delete(caminho);
            }
        }
        catch (IOException)
        {
        }
    }
}
