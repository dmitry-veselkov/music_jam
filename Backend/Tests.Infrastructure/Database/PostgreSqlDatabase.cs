using Testcontainers.PostgreSql;

namespace Tests.Infrastructure.Database;

public sealed class PostgreSqlDatabase : IAsyncDisposable
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder(Constants.Database.Image)
        .WithDatabase(Constants.Database.Name)
        .WithUsername(Constants.User.Username)
        .WithPassword(Constants.User.Password)
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public Task StartAsync()
    {
        return _container.StartAsync();
    }

    public ValueTask DisposeAsync()
    {
        return _container.DisposeAsync();
    }
}
