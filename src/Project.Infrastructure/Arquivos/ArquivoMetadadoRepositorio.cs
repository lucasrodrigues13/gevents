using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Persistence;

namespace Project.Infrastructure.Arquivos;

public sealed class ArquivoMetadadoRepositorio : IArquivoMetadadoRepositorio
{
    private readonly AppDbContext _db;

    public ArquivoMetadadoRepositorio(AppDbContext db)
    {
        _db = db;
    }

    public async Task AdicionarAsync(ArquivoMetadado metadado, CancellationToken cancellationToken)
    {
        _db.Arquivos.Add(metadado);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task<ArquivoMetadado?> ObterAsync(Guid id, CancellationToken cancellationToken)
    {
        return _db.Arquivos.FirstOrDefaultAsync(arquivo => arquivo.Id == id, cancellationToken);
    }

    public async Task RemoverAsync(ArquivoMetadado metadado, CancellationToken cancellationToken)
    {
        _db.Arquivos.Remove(metadado);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public void Descartar(ArquivoMetadado metadado)
    {
        var entrada = _db.Entry(metadado);
        if (entrada.State != EntityState.Detached)
        {
            entrada.State = EntityState.Detached;
        }
    }
}
