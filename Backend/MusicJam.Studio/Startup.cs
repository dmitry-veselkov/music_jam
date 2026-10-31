using System.Buffers;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MusicJam.Studio.Configurations;
using MusicJam.Studio.Constants;
using MusicJam.Studio.Extensions;
using MusicJam.Studio.Infrustructure.Data.Configurations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Text.Json.Serialization;

namespace MusicJam.Studio;

public class Startup(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddServices();
        services.AddStorages();
        services.AddControllers(options =>
        {
            options.OutputFormatters.Insert(0, new
                NewtonsoftJsonOutputFormatter(new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Converters = { new StringEnumConverter() }
                }, ArrayPool<char>.Shared, options));
            options.RespectBrowserAcceptHeader = true;
        })
        // enum'ы во входящем JSON — строками, как их шлёт фронт ("Hosted"/"Hostless")
        .AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        var jwtSettings = new JwtSettings();
        Configuration.GetSection(nameof(JwtSettings)).Bind(jwtSettings);
        services.Configure<JwtSettings>(Configuration.GetSection(nameof(JwtSettings)));

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = AuthConstants.DefaultAuthorizationScheme;
            })
            .AddJwtBearer(
                AuthConstants.DefaultAuthorizationScheme,
                options =>
                {
                    options.TokenValidationParameters = GetTokenValidationParameters(jwtSettings);
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
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention()
                .EnableSensitiveDataLogging());
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app
            .UseHttpsRedirection()
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    private static TokenValidationParameters GetTokenValidationParameters(JwtSettings jwtSettings) => new()
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ValidateIssuerSigningKey = true,
    };
}
