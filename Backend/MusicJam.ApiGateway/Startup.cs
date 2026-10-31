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
        app
            .UseDefaultFiles()
            .UseStaticFiles()
            .UseRouting()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapReverseProxy();
                endpoints.MapFallbackToFile("index.html");
            });
    }
}
