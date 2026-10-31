using Npgsql;
using Respawn;

namespace Tests.Infrastructure.Database;

public sealed class DatabaseCleaner(string connectionString)
{
    private Respawner? _respawner;

    public async Task InitializeAsync()
    {
        await using var connection = new NpgsqlConnection(connectionString);

        await connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(
            connection,
            new RespawnerOptions { DbAdapter = DbAdapter.Postgres });
    }

    public async Task ResetAsync()
    {
        if (_respawner is null)
        {
            throw new InvalidOperationException("DatabaseCleaner has not been initialized.");
        }

        await using var connection = new NpgsqlConnection(connectionString);

        await connection.OpenAsync();

        await _respawner.ResetAsync(connection);
    }
}
