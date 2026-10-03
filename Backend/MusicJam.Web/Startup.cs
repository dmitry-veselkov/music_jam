using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MusicJam.Domain.Extensions;
using MusicJam.Infrastructure.Configuration;
using MusicJam.Infrastructure.Extensions;
using MusicJam.Web.Models.Constants;
using MusicJam.Web.DI;

namespace MusicJam.Web;

public class Startup(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddServices();
        services.AddInfrastructure();
        services.AddControllers();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "MusicJam API",
                Version = "v1"
            });
        });

        var jwtSettings = new JwtSettings();
        Configuration.GetSection(nameof(JwtSettings)).Bind(jwtSettings);
        services.Configure<JwtSettings>(Configuration.GetSection(nameof(JwtSettings)));

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = AuthSchemes.AccountJwtTokenScheme;
                options.DefaultChallengeScheme = AuthSchemes.AccountJwtTokenScheme;
            })
            .AddJwtBearer(
                AuthSchemes.AccountJwtTokenScheme,
                options =>
                {
                    options.TokenValidationParameters = GetTokenValidationParameters(jwtSettings);

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies["access_token"];
                            return Task.CompletedTask;
                        }
                    };
                })
            .AddCookie(AuthSchemes.GameSessionCookieScheme, options =>
            {
                options.Cookie.Name = "session.id";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
            });

        services
            .AddAuthorizationBuilder()
            .AddPolicy(GamePolicies.ManageGamePolicy, policy =>
            {
                policy.AuthenticationSchemes.Add(AuthSchemes.GameSessionCookieScheme);
                policy.RequireClaim(GameClaims.GameRole, nameof(GameRole.Admin));
            })
            .AddPolicy(GamePolicies.PlayGamePolicy, policy =>
            {
                policy.AuthenticationSchemes.Add(AuthSchemes.GameSessionCookieScheme);
                policy.RequireClaim(GameClaims.GameRole, nameof(GameRole.Player));
            })
            .AddPolicy(GamePolicies.ViewGamePolicy, policy =>
            {
                policy.AuthenticationSchemes.Add(AuthSchemes.GameSessionCookieScheme);
                policy.RequireClaim(GameClaims.GameRole, nameof(GameRole.Admin), nameof(GameRole.Player),
                    nameof(GameRole.Viewer));
            });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        var staticFileOptions = new StaticFileOptions
        {
            FileProvider = FileProviderExtensions.CreateStaticFileProvider(env)
        };

        app
            .UseRouting()
            .UseDefaultFiles(new DefaultFilesOptions { FileProvider = staticFileOptions.FileProvider })
            .UseStaticFiles(staticFileOptions)
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html", staticFileOptions);            });
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
