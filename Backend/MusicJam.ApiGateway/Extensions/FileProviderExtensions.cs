using Microsoft.Extensions.FileProviders;

namespace MusicJam.ApiGateway.Extensions;

public static class FileProviderExtensions
{
    public static IFileProvider CreateStaticFileProvider(IWebHostEnvironment env)
    {
        var distPath = Path.GetFullPath(Path.Combine(env.ContentRootPath, "..", "..", "Frontend", "dist"));
        return new PhysicalFileProvider(distPath);
    }
}
