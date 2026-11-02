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

        result.IsSuccess.ShouldBeFalse();
        result.GetFaultOrThrow().ShouldBe(IPacksService.PackError.NotFound);
    }

    [Fact]
    public async Task GetPack_WhenPackExists_ReturnsPack()
    {
        var user = await CreateUserAsync();
        var packDb = await CreatePackAsync(user.Id, TestPackName);
       
        var result = await PacksService.GetPack(user.Id, packDb.Id);
        result.IsSuccess.ShouldBeTrue();

        var pack = result.GetValueOrThrow();
        pack.ShouldNotBeNull();
        pack.Id.ShouldBe(packDb.Id);
        pack.Name.ShouldBe(packDb.Title);
    }

    [Fact]
    public async Task GetPack_WhenPackBelongsToAnotherUser_ReturnsNotFound()
    {
        var owner = await CreateUserAsync();
        var anotherUser = await CreateUserAsync();

        var pack = await CreatePackAsync(owner.Id, TestPackName);

        var result = await PacksService.GetPack(
            anotherUser.Id,
            pack.Id);

        result.IsSuccess.ShouldBeFalse();
        result.GetFaultOrThrow().ShouldBe(IPacksService.PackError.NotFound);
    }
}
