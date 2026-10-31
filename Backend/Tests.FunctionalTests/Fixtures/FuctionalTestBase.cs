using Microsoft.Extensions.DependencyInjection;
using MusicJam.Studio.Infrustructure.Data;
using Tests.Infrastructure.Data;

namespace Tests.FunctionalTests.Fixtures;

public abstract class FunctionalTestBase(TestApplicationFixture fixture) : IAsyncLifetime
{
    protected TestApplicationFixture Fixture { get; } = fixture;

    protected IServiceProvider Services => Fixture.Services;

    protected HttpClient Client => Fixture.Client;

    public async Task InitializeAsync()
    {
        await Fixture.ResetDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    protected IServiceScope CreateScope()
    {
        return Fixture.CreateScope();
    }

    protected async Task SeedAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        using var scope = CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        var seeder = new TestDataSeeder(dbContext);

        await seeder.SeedAsync(entity);
    }
}
