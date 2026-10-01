using Microsoft.Extensions.FileProviders;

namespace MusicJam.Web.DI;

public static class FrontendFiles
{
    public static IFileProvider CreateProvider()
    {
        var distPath = Path.GetFullPath(Path.Combine("..", "..", "Frontend", "dist"));
        return new PhysicalFileProvider(distPath);
    }
}
