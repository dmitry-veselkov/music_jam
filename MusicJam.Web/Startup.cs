namespace MusicJam.Web;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // services.AddServices();
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app
            .UseRouting()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToController("Index", "Home");
            });
    }
}