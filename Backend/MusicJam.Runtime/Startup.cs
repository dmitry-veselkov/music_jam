using System.Text.Json;
using System.Text.Json.Serialization;
using MusicJam.Runtime.Hubs;
using MusicJam.Runtime.Models;
using MusicJam.Runtime.Services.WaitingRoomService;
using MusicJam.Runtime.Storages.ConnectionStorage;
using MusicJam.Runtime.Storages.GameStorage;
using MusicJam.Runtime.Storages.TeamStorage;

namespace MusicJam.Runtime;

public class Startup(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSignalR()
            .AddJsonProtocol(o => o.PayloadSerializerOptions.Converters.Add(
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));
        services.AddSingleton<IGameStorage, InMemoryGameStorage>();
        services.AddSingleton<ITeamStorage, InMemoryTeamStorage>();
        services.AddSingleton<IConnectionStorage, InMemoryConnectionStorage>();
        services.AddScoped<IWaitingRoomService, WaitingRoomService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<GameHub>("/api/runtime/games");
            });
    }
}
