using MusicJam.Studio.Services.PackService;
using Shouldly;
using Tests.FunctionalTests.Fixtures;

namespace Tests.FunctionalTests.Tests.PacksServiceTests;

[Collection(TestCollection.Name)]
public sealed class GetPackTests(TestApplicationFixture fixture) : PacksServiceTestBase(fixture)
{
    [Fact]
    public async Task GetPack_WhenPackDoesNotExist_ReturnsNotFound()
    {
        var userId = Guid.NewGuid();
        var packId = Guid.NewGuid();

        var result = await PacksService.GetPack(userId, packId);

        var success = result.TryGetValue(out var pack, out var error);

        success.ShouldBeFalse();
        pack.ShouldBeNull();
        error.ShouldBe(IPacksService.PackError.NotFound);
    }
}
