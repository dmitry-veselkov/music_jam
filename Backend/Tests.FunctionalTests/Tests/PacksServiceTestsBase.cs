using Microsoft.Extensions.DependencyInjection;
using MusicJam.Studio.Services.PackService;
using Tests.FunctionalTests.Fixtures;

namespace Tests.FunctionalTests.Tests;

public abstract class PacksServiceTestBase(TestApplicationFixture fixture) : FunctionalTestBase(fixture)
{
    protected static IPacksService GetPacksService(IServiceScope scope) =>
        scope.ServiceProvider.GetRequiredService<IPacksService>();
}
