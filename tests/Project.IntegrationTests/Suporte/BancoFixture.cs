using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Persistencia;
using Testcontainers.PostgreSql;

namespace Project.IntegrationTests.Suporte;

public sealed class BancoFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16-alpine").Build();

    public string Conexao => _postgres.GetConnectionString();

    public FabricaApi Fabrica { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(Conexao).Options;
        await using (var banco = new AppDbContext(options))
        {
            await banco.Database.MigrateAsync();
        }

        Fabrica = new FabricaApi(Conexao);
        using var cliente = Fabrica.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await Fabrica.DisposeAsync();
        await _postgres.DisposeAsync();
    }
}

[CollectionDefinition(Nome, DisableParallelization = true)]
public sealed class ColecaoApi : ICollectionFixture<BancoFixture>
{
    public const string Nome = "api";
}
