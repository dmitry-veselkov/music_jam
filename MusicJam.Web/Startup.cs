using System.Text;
using Microsoft.IdentityModel.Tokens;
using MusicJam.Domain.Extensions;
using MusicJam.Web.Configurations;
using MusicJam.Web.Models.Constants;

namespace MusicJam.Web;

public class Startup(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddServices();
        // services.AddInfrastructure();
        services.AddControllers();

        var jwtSettings = new JwtSettings();
        Configuration.GetSection(nameof(JwtSettings)).Bind(jwtSettings);
        services.Configure<JwtSettings>(Configuration.GetSection(nameof(JwtSettings)));

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = AuthSchemes.AccountJwtTokenScheme;
                options.DefaultChallengeScheme = AuthSchemes.AccountJwtTokenScheme;
            })
            .AddJwtBearer(AuthSchemes.AccountJwtTokenScheme,
                options => { options.TokenValidationParameters = GetTokenValidationParameters(jwtSettings); })
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
        app
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToController("Index", "Home");
            });
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