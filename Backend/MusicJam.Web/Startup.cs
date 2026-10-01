using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MusicJam.Web.Configurations;
using MusicJam.Web.DI;

namespace MusicJam.Web;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // services.AddServices();
        // services.AddInfrastructure();
        services.AddControllers();

        services.AddAuthorization();

        var jwtSettings = new JwtSettings();
        Configuration.GetSection(nameof(JwtSettings)).Bind(jwtSettings);
        services.Configure<JwtSettings>(Configuration.GetSection(nameof(JwtSettings)));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
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
            });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var fileProvider = FrontendFiles.CreateProvider();
        app
            .UseRouting()
            .UseDefaultFiles(new DefaultFilesOptions { FileProvider = fileProvider })
            .UseStaticFiles(new StaticFileOptions { FileProvider = fileProvider })
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html", new StaticFileOptions { FileProvider = fileProvider });
            });
    }
}
