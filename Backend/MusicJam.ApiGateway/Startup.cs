using MusicJam.ApiGateway.Extensions;

namespace MusicJam.ApiGateway;

public class Startup(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddReverseProxy()
            .LoadFromConfig(Configuration.GetSection("ReverseProxy"));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var staticFileOptions = new StaticFileOptions
        {
            FileProvider = FileProviderExtensions.CreateStaticFileProvider(env)
        };

        app
            .UseRouting()
            .UseDefaultFiles(new DefaultFilesOptions { FileProvider = staticFileOptions.FileProvider })
            .UseStaticFiles(staticFileOptions)
            .UseEndpoints(endpoints =>
            {
                endpoints.MapReverseProxy();
                endpoints.MapFallbackToFile("index.html", staticFileOptions);
            });
    }
}
