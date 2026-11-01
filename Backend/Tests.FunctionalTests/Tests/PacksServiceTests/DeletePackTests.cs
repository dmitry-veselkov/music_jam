using MusicJam.Studio.Services.PackService;
using Shouldly;
using Tests.FunctionalTests.Fixtures;

namespace Tests.FunctionalTests.Tests.PacksServiceTests;

[Collection(TestCollection.Name)]
public sealed class DeletePackTests(TestApplicationFixture fixture) : PacksServiceTestBase(fixture)
{
    [Fact]
    public async Task Delete_WhenPackExists_ReturnsTrue()
    {
        var user = await CreateUserAsync();
        var pack = await CreatePackAsync(user.Id, TestPackName);

        var result = await PacksService.Delete(user.Id, pack.Id);

        result.IsSuccess.ShouldBeTrue();
        result.GetValueOrThrow().ShouldBeTrue();

        var getResult = await PacksService.GetPack(user.Id, pack.Id);
        getResult.IsSuccess.ShouldBeFalse();
        getResult.GetFaultOrThrow().ShouldBe(IPacksService.PackError.NotFound);
    }

    [Fact]
    public async Task Delete_WhenPackDoesNotExist_ReturnsNotFound()
    {
        var user = await CreateUserAsync();

        var result = await PacksService.Delete(user.Id, Guid.NewGuid());

        result.IsSuccess.ShouldBeFalse();
        result.GetFaultOrThrow().ShouldBe(IPacksService.PackError.NotFound);
    }

    [Fact]
    public async Task Delete_WhenPackBelongsToAnotherUser_ReturnsNotFound()
    {
        var owner = await CreateUserAsync();
        var anotherUser = await CreateUserAsync();
        var pack = await CreatePackAsync(owner.Id, TestPackName);

        var result = await PacksService.Delete(anotherUser.Id, pack.Id);

        result.IsSuccess.ShouldBeFalse();
        result.GetFaultOrThrow().ShouldBe(IPacksService.PackError.NotFound);

        var getResult = await PacksService.GetPack(owner.Id, pack.Id);
        getResult.IsSuccess.ShouldBeTrue();
        getResult.GetValueOrThrow().Id.ShouldBe(pack.Id);
    }

    [Fact]
    public async Task Delete_WhenPackWasAlreadyDeleted_ReturnsNotFound()
    {
        var user = await CreateUserAsync();
        var pack = await CreatePackAsync(user.Id, TestPackName);

        var firstResult = await PacksService.Delete(user.Id, pack.Id);
        firstResult.IsSuccess.ShouldBeTrue();

        var secondResult = await PacksService.Delete(user.Id, pack.Id);

        secondResult.IsSuccess.ShouldBeFalse();
        secondResult.GetFaultOrThrow().ShouldBe(IPacksService.PackError.NotFound);
    }
}