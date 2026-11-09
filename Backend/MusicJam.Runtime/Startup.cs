using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MusicJam.Runtime.Hubs;
using MusicJam.Runtime.Models;
using MusicJam.Runtime.Services.GameService;
using MusicJam.Runtime.Services.WaitingRoomService;
using MusicJam.Runtime.Storages.ConnectionStorage;
using MusicJam.Runtime.Storages.GameStorage;
using MusicJam.Runtime.Storages.TeamStorage;
using MusicJam.Studio.Configurations;
using MusicJam.Studio.Constants;
using MusicJam.Studio.Extensions;
using MusicJam.Studio.Infrustructure.Data;

namespace MusicJam.Runtime;

public class Startup(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        var jwtSettings = new JwtSettings();
        Configuration.GetSection(nameof(JwtSettings)).Bind(jwtSettings);
        
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = AuthConstants.DefaultAuthorizationScheme;
            })
            .AddJwtBearer(AuthConstants.DefaultAuthorizationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ValidateIssuerSigningKey = true,
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies[AuthConstants.AccessToken];
                        return Task.CompletedTask;
                    }
                };
            });
        services.AddAuthorization();

        services.AddControllers();
        services.AddSignalR()
            .AddJsonProtocol(o => o.PayloadSerializerOptions.Converters.Add(
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));
        services.AddSingleton<IGameStorage, InMemoryGameStorage>();
        services.AddSingleton<ITeamStorage, InMemoryTeamStorage>();
        services.AddSingleton<IConnectionStorage, InMemoryConnectionStorage>();
        services.AddSingleton<IWaitingRoomService, WaitingRoomService>();
        services.AddSingleton<IGameService, GameService>();
        
        services.AddDbContext<AppDbContext>(o =>
            o.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention());

        services
            .ConfigureSettings(Configuration)
            .AddObjectStorage()
            .AddStorages()
            .AddServices();

        services.AddScoped<IGameService, GameService>();
        
        
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
