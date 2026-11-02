using MusicJam.Studio.Services.PackService;
using Shouldly;
using Tests.FunctionalTests.Fixtures;

namespace Tests.FunctionalTests.Tests.PacksServiceTests;

[Collection(TestCollection.Name)]
public sealed class GetEditPackTests(TestApplicationFixture fixture)
    : PacksServiceTestBase(fixture)
{
    [Fact]
    public async Task GetEditPack_WhenPackExists_ReturnsPack()
    {
        var user = await CreateUserAsync();
        var pack = await CreatePackAsync(user.Id, TestPackName);

        var result = await PacksService.GetEditPack(user.Id, pack.Id);

        result.IsSuccess.ShouldBeTrue();

        var editPack = result.GetValueOrThrow();
        editPack.ShouldNotBeNull();
        editPack.PackId.ShouldBe(pack.Id);
        editPack.Name.ShouldBe(pack.Title);
    }

    [Fact]
    public async Task GetEditPack_WhenPackDoesNotExist_Returns404()
    {
        var user = await CreateUserAsync();
        var packId = Guid.NewGuid();

        var result = await PacksService.GetEditPack(user.Id, packId);

        result.IsSuccess.ShouldBeFalse();
        result.GetFaultOrThrow().ShouldBe(IPacksService.PackError.NotFound);
    }
}
