using Microsoft.Extensions.FileProviders;

namespace MusicJam.Web.DI;

public static class FileProviderExtensions
{
    public static IFileProvider CreateStaticFileProvider(IWebHostEnvironment env)
    {
        var distPath = Path.GetFullPath(Path.Combine(env.ContentRootPath, "..", "..", "Frontend", "dist"));
        return new PhysicalFileProvider(distPath);
    }
}
