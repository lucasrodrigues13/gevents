using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Project.Application.Arquivos;
using Project.Domain;

namespace Project.Infrastructure.Arquivos;

public sealed class ArmazenamentoArquivo : IArmazenamentoArquivo
{
    private readonly string _root;
    private readonly long _maxImageBytes;
    private readonly IArquivoMetadadoRepositorio _metadados;
    private readonly ILogger<ArmazenamentoArquivo> _logger;

    public ArmazenamentoArquivo(
        IHostEnvironment environment,
        IOptions<StorageOptions> options,
        IArquivoMetadadoRepositorio metadados,
        ILogger<ArmazenamentoArquivo> logger)
    {
        var configurada = options.Value.Root;
        if (string.IsNullOrWhiteSpace(configurada))
        {
            if (environment.IsProduction())
            {
                throw new InvalidOperationException("Storage:Root é obrigatório em produção.");
            }

            configurada = Path.Combine(environment.ContentRootPath, "storage");
        }

        _root = Path.GetFullPath(configurada);
        Directory.CreateDirectory(_root);
        Directory.CreateDirectory(Path.Combine(_root, "temp"));
        _maxImageBytes = options.Value.MaxImageBytes > 0 ? options.Value.MaxImageBytes : 5 * 1024 * 1024;
        _metadados = metadados;
        _logger = logger;
    }

    public async Task<Guid> SalvarImagemAsync(Stream conteudo, string nomeOriginal, CancellationToken cancellationToken)
    {
        if (conteudo.CanSeek)
        {
            if (conteudo.Length == 0)
            {
                throw new RegraDeNegocioException("O arquivo está vazio.");
            }

            if (conteudo.Length > _maxImageBytes)
            {
                throw new RegraDeNegocioException("A imagem passa de 5 MB.");
            }
        }

        var temp = Path.Combine(_root, "temp", Guid.NewGuid().ToString("N"));
        try
        {
            var tamanho = await CopiarComLimiteAsync(conteudo, temp, cancellationToken);
            if (tamanho == 0)
            {
                throw new RegraDeNegocioException("O arquivo está vazio.");
            }

            var extensao = Extensao(nomeOriginal);
            var assinatura = await LerAssinaturaAsync(temp, cancellationToken);
            if (extensao is null || assinatura is null || Familia(extensao) != assinatura)
            {
                throw new RegraDeNegocioException("O formato da imagem não é aceito.");
            }

            var id = Guid.NewGuid();
            var agora = DateTime.UtcNow;
            var relativo = $"images/{agora:yyyy}/{agora:MM}/{id:D}.{extensao}";
            var final = CaminhoSeguro(relativo) ?? throw new InvalidOperationException("Falha ao gravar o arquivo.");
            Directory.CreateDirectory(Path.GetDirectoryName(final)!);
            await CopiarExclusivoAsync(temp, final, cancellationToken);

            var metadado = new ArquivoMetadado
            {
                Id = id,
                NomeOriginal = Sanitizar(nomeOriginal, extensao),
                CaminhoRelativo = relativo,
                Mime = Mime(assinatura),
                Tamanho = tamanho,
                Categoria = "images",
                GravadoEmUtc = agora
            };

            try
            {
                await _metadados.AdicionarAsync(metadado, cancellationToken);
            }
            catch (Exception exception)
            {
                _metadados.Descartar(metadado);
                Apagar(final);
                _logger.LogError(exception, "Falha ao gravar metadado do arquivo.");
                throw;
            }

            return id;
        }
        finally
        {
            Apagar(temp);
        }
    }

    public async Task<ArquivoBaixado?> AbrirAsync(Guid id, CancellationToken cancellationToken)
    {
        var metadado = await _metadados.ObterAsync(id, cancellationToken);
        if (metadado is null)
        {
            return null;
        }

        var caminho = CaminhoSeguro(metadado.CaminhoRelativo);
        if (caminho is null || !File.Exists(caminho))
        {
            return null;
        }

        var stream = new FileStream(caminho, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true);
        return new ArquivoBaixado(stream, metadado.NomeOriginal, metadado.Mime);
    }

    public async Task ExcluirAsync(Guid id, CancellationToken cancellationToken)
    {
        var metadado = await _metadados.ObterAsync(id, cancellationToken);
        if (metadado is null)
        {
            return;
        }

        var caminho = CaminhoSeguro(metadado.CaminhoRelativo);
        await _metadados.RemoverAsync(metadado, cancellationToken);
        if (caminho is not null)
        {
            Apagar(caminho);
        }
    }

    private async Task<long> CopiarComLimiteAsync(Stream origem, string destino, CancellationToken cancellationToken)
    {
        var buffer = new byte[81920];
        long total = 0;
        await using var saida = new FileStream(destino, FileMode.CreateNew, FileAccess.Write, FileShare.None, buffer.Length, useAsync: true);
        while (true)
        {
            var lidos = await origem.ReadAsync(buffer, cancellationToken);
            if (lidos == 0)
            {
                break;
            }

            total += lidos;
            if (total > _maxImageBytes)
            {
                throw new RegraDeNegocioException("A imagem passa de 5 MB.");
            }

            await saida.WriteAsync(buffer.AsMemory(0, lidos), cancellationToken);
        }

        return total;
    }

    private static async Task CopiarExclusivoAsync(string origem, string destino, CancellationToken cancellationToken)
    {
        await using var entrada = new FileStream(origem, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true);
        await using var saida = new FileStream(destino, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true);
        await entrada.CopyToAsync(saida, cancellationToken);
    }

    private static async Task<string?> LerAssinaturaAsync(string caminho, CancellationToken cancellationToken)
    {
        var header = new byte[12];
        await using var leitura = new FileStream(caminho, FileMode.Open, FileAccess.Read, FileShare.Read);
        var lidos = await leitura.ReadAsync(header, cancellationToken);
        return Assinatura(header.AsSpan(0, lidos));
    }

    private string? CaminhoSeguro(string relativo)
    {
        var prefixo = _root.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        var combinado = Path.GetFullPath(Path.Combine(_root, relativo.Replace('/', Path.DirectorySeparatorChar)));
        if (!combinado.StartsWith(prefixo, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return combinado;
    }

    private static string? Assinatura(ReadOnlySpan<byte> header)
    {
        if (header.Length >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
        {
            return "jpeg";
        }

        if (header.Length >= 8
            && header[0] == 0x89
            && header[1] == 0x50
            && header[2] == 0x4E
            && header[3] == 0x47
            && header[4] == 0x0D
            && header[5] == 0x0A
            && header[6] == 0x1A
            && header[7] == 0x0A)
        {
            return "png";
        }

        if (header.Length >= 12
            && header[0] == (byte)'R'
            && header[1] == (byte)'I'
            && header[2] == (byte)'F'
            && header[3] == (byte)'F'
            && header[8] == (byte)'W'
            && header[9] == (byte)'E'
            && header[10] == (byte)'B'
            && header[11] == (byte)'P')
        {
            return "webp";
        }

        return null;
    }

    private static string? Extensao(string nome)
    {
        var extensao = Path.GetExtension(nome).TrimStart('.').ToLowerInvariant();
        return extensao is "jpg" or "jpeg" or "png" or "webp" ? extensao : null;
    }

    private static string Familia(string extensao)
    {
        return extensao is "jpg" or "jpeg" ? "jpeg" : extensao;
    }

    private static string Mime(string assinatura)
    {
        return assinatura switch
        {
            "jpeg" => "image/jpeg",
            "png" => "image/png",
            "webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }

    private static string Sanitizar(string nomeOriginal, string extensao)
    {
        var nome = nomeOriginal.Replace('\\', '/');
        nome = Path.GetFileName(nome);
        var limpo = new string(nome.Where(caractere => !char.IsControl(caractere)).ToArray()).Replace("..", "", StringComparison.Ordinal);
        if (string.IsNullOrWhiteSpace(limpo) || limpo.Contains('/') || limpo.Contains('\\'))
        {
            return "imagem." + extensao;
        }

        return limpo;
    }

    private void Apagar(string caminho)
    {
        try
        {
            if (File.Exists(caminho))
            {
                File.Delete(caminho);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Falha ao apagar o arquivo.");
        }
    }
}
