using Microsoft.EntityFrameworkCore;
using MusicJam.Studio.Infrastructure.Data;

namespace MusicJam.Studio;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        await using (var scope = host.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await dbContext.Database.MigrateAsync();
        }

        await host.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webHostBuilder =>
            {
                webHostBuilder.UseStartup<Startup>();
            });
    }
}
