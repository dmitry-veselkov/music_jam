using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MusicJam.Runtime.Clients.StudioClient;
using MusicJam.Runtime.Hubs;
using MusicJam.Runtime.Middlewares;
using MusicJam.Runtime.Services.GameService;
using MusicJam.Runtime.Services.InvitesService;
using MusicJam.Runtime.Services.UserService;
using MusicJam.Runtime.Services.WaitingRoomService;
using MusicJam.Runtime.Storages.ConnectionStorage;
using MusicJam.Runtime.Storages.GameStorage;
using MusicJam.Runtime.Storages.InvitesStorage;
using MusicJam.Studio.Constants;
using Shared;
using Shared.Auth;

namespace MusicJam.Runtime;

public class Startup(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOptions<EnvironmentSettings>().BindConfiguration(EnvironmentSettings.SectionName);

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

        services.AddControllers()
            .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));


        services.AddSignalR()
            .AddJsonProtocol(o => o.PayloadSerializerOptions.Converters.Add(
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));
        services.AddSingleton<IGameStorage, GameStorage>();
        services.AddSingleton<IConnectionStorage, InMemoryConnectionStorage>();
        services.AddSingleton<IWaitingRoomService, WaitingRoomService>();
        services.AddSingleton<IInvitesStorage, InMemoryInvitesStorage>();
        services.AddScoped<IGameService, GameService>();
        services.Decorate<IGameService, GameServiceLoggerDecorator>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IInvitesService, InvitesService>();
        services.Decorate<IInvitesService, InvitesServiceLoggerDecorator>();

        services.AddHttpContextAccessor();
        services.AddTransient<ForwardAccessTokenHandler>();
        services.AddHttpClient<IStudioClient, StudioClient>(client =>
            {
                var studioUrl = Configuration["Services:Studio"]
                                ?? throw new InvalidOperationException("Не задан адрес Studio: Services:Studio");
                client.BaseAddress = new Uri(studioUrl);
            })
            .AddHttpMessageHandler<ForwardAccessTokenHandler>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app
            .UseMiddleware<SessionMiddleware>()
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<GameHub>(GameHub.Path);
            });
    }
}
