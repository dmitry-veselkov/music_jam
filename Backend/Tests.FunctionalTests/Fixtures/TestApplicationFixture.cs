using Microsoft.Extensions.DependencyInjection;
using MusicJam.Studio.Infrastructure.Data;
using Tests.Infrastructure.Application;
using Tests.Infrastructure.Database;

namespace Tests.FunctionalTests.Fixtures;

public sealed class TestApplicationFixture : IAsyncLifetime
{
    private PostgreSqlDatabase? _database;
    private TestHostFactory? _hostFactory;
    private DatabaseCleaner? _databaseCleaner;

    public IServiceProvider Services =>
        _hostFactory?.Services
        ?? throw new InvalidOperationException("Test application has not been initialized.");

    public HttpClient Client =>
        _hostFactory?.Client
        ?? throw new InvalidOperationException("Test application has not been initialized.");

    public async Task InitializeAsync()
    {
        _database = new PostgreSqlDatabase();

        await _database.StartAsync();

        _hostFactory = new TestHostFactory(_database.ConnectionString);

        await _hostFactory.StartAsync();

        await using (var scope = Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var initializer = new DatabaseInitializer(dbContext);

            await initializer.InitializeAsync();
        }

        _databaseCleaner = new DatabaseCleaner(_database.ConnectionString);

        await _databaseCleaner.InitializeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        if (_databaseCleaner is null)
        {
            throw new InvalidOperationException("Database cleaner has not been initialized.");
        }

        await _databaseCleaner.ResetAsync();
    }

    public IServiceScope CreateScope()
    {
        return Services.CreateScope();
    }

    public async Task DisposeAsync()
    {
        if (_hostFactory is not null)
        {
            await _hostFactory.DisposeAsync();
        }

        if (_database is not null)
        {
            await _database.DisposeAsync();
        }
    }
}
