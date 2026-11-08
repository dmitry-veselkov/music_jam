using System.Text.Json;
using System.Text.Json.Serialization;
using MusicJam.Runtime.Hubs;
using MusicJam.Runtime.Services.WaitingRoomService;
using MusicJam.Runtime.Storage;

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
        services.AddSingleton<IRoomStore, InMemoryRoomStore>();
        services.AddSingleton<IWaitingRoomService, WaitingRoomService>();
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
