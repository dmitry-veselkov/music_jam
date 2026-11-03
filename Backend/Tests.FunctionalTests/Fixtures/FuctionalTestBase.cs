using Microsoft.Extensions.DependencyInjection;
using MusicJam.Studio.Infrustructure.Data;
using Tests.Infrastructure.Data;

namespace Tests.FunctionalTests.Fixtures;

public abstract class FunctionalTestBase(TestApplicationFixture fixture) : IAsyncLifetime
{
    protected TestApplicationFixture Fixture { get; } = fixture;

    protected IServiceProvider Services => Fixture.Services;

    protected HttpClient Client => Fixture.Client;

    private IServiceScope? _scope;

    protected IServiceProvider ScopedServices =>
        _scope?.ServiceProvider
        ?? throw new InvalidOperationException("Test scope has not been initialized.");

    public async Task InitializeAsync()
    {
        await Fixture.ResetDatabaseAsync();

        _scope = CreateScope();
    }

    public Task DisposeAsync()
    {
        _scope?.Dispose();

        return Task.CompletedTask;
    }

    protected IServiceScope CreateScope()
    {
        return Fixture.CreateScope();
    }

    protected T GetService<T>() where T : notnull
    {
        return ScopedServices.GetRequiredService<T>();
    }

    protected async Task SeedAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        using var scope = CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var seeder = new TestDataSeeder(dbContext);

        await seeder.SeedAsync(entity);
    }
}
