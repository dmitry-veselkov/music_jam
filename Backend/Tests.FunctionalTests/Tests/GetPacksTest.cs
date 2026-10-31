using FluentAssertions;
using MusicJam.Studio.Services.PackService;
using Tests.FunctionalTests.Fixtures;

namespace Tests.FunctionalTests.Tests;

[Collection(TestCollection.Name)]
public sealed class GetPackTests(TestApplicationFixture fixture) : PacksServiceTestBase(fixture)
{
    [Fact]
    public async Task GetPack_WhenPackDoesNotExist_ReturnsNotFound()
    {
        var userId = Guid.NewGuid();
        var packId = Guid.NewGuid();

        using var scope = CreateScope();
        var service = GetPacksService(scope);

        var result = await service.GetPack(userId, packId);

        var success = result.TryGetValue(out var pack, out var error);

        success.Should().BeFalse();
        pack.Should().BeNull();
        error.Should().Be(IPacksService.PackError.NotFound);
    }
}
