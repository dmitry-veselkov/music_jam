using MusicJam.Studio.Services.PackService;
using Tests.FunctionalTests.Fixtures;

namespace Tests.FunctionalTests.Tests.PacksServiceTests;

public abstract class PacksServiceTestBase(TestApplicationFixture fixture) : FunctionalTestBase(fixture)
{
    protected IPacksService PacksService => GetService<IPacksService>();
}
