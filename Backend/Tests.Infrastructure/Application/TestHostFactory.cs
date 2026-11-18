using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace Tests.Infrastructure.Application;

public sealed class TestHostFactory(string dbConnectionString, string s3ConnectionString) : IAsyncDisposable
{
    private IHost? _host;

    public IServiceProvider Services =>
        _host?.Services
        ?? throw new InvalidOperationException("Test host has not been started.");

    public HttpClient Client =>
        _host?.GetTestClient()
        ?? throw new InvalidOperationException("Test host has not been started.");

    public async Task StartAsync()
    {
        var configuration = new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = dbConnectionString,
            ["JwtSettings:Issuer"] = Constants.Configuration.JwtSettingsIssuer,
            ["JwtSettings:Audience"] = Constants.Configuration.JwtSettingsAudience,
            ["JwtSettings:SecretKey"] = Constants.Configuration.JwtSettingsSecretKey,
            ["ObjectStorageSettings:AccessKey"] = Constants.ObjectStorage.AccessKey,
            ["ObjectStorageSettings:SecretKey"] = Constants.ObjectStorage.SecretKey,
            ["ObjectStorageSettings:BucketName"] = Constants.ObjectStorage.BucketName,
            ["ObjectStorageSettings:ObjectStorageBaseUrl"] = s3ConnectionString
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
