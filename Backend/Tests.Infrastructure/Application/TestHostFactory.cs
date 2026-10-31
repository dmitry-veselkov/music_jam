using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.TestHost;


namespace Tests.Infrastructure.Application;

public sealed class TestHostFactory(string connectionString) : IAsyncDisposable
{
    private IHost? _host;

    public IServiceProvider Services =>
        _host?.Services
        ?? throw new InvalidOperationException("Test host has not been started.");

    public HttpClient Client =>
        _host?.GetTestClient()
        ?? throw new InvalidOperationException(
            "Test host has not been started.");

    public async Task StartAsync()
    {
        var configuration = new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = connectionString,
            ["JwtSettings:Issuer"] = "MusicJam.Tests",
            ["JwtSettings:Audience"] = "MusicJam.Tests",
            ["JwtSettings:SecretKey"] = "TestSecretKey_AtLeast32CharactersLong!"
        };

        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((_, config) => { config.AddInMemoryCollection(configuration); })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .UseStartup<MusicJam.Studio.Startup>()
                    .UseTestServer();
            })
            .Build();

        await _host.StartAsync();
    }

    public IServiceScope CreateScope()
    {
        return Services.CreateScope();
    }

    public async ValueTask DisposeAsync()
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
    }
}
