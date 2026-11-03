using MusicJam.Studio.Requests.Packs;
using Shouldly;
using Tests.FunctionalTests.Fixtures;

namespace Tests.FunctionalTests.Tests.PacksServiceTests;

[Collection(TestCollection.Name)]
public sealed class GetShortPacksTests(TestApplicationFixture fixture) : PacksServiceTestBase(fixture)
{
    [Fact]
    public async Task GetShortPacks_WhenUserHasNoPacks_ReturnsEmptyArray()
    {
        var user = await CreateUserAsync();

        var result = await PacksService.GetShortPacks(user.Id, new GetPackRequest());

        result.IsSuccess.ShouldBeTrue();
        result.GetValueOrThrow().ShouldBeEmpty();
    }

    [Fact]
    public async Task GetShortPacks_WhenUserHasPacks_ReturnsUserPacks()
    {
        var user = await CreateUserAsync();
        var createdPacks = await CreatePacksAsync(user.Id, 3);

        var result = await PacksService.GetShortPacks(user.Id, new GetPackRequest());

        result.IsSuccess.ShouldBeTrue();

        var packs = result.GetValueOrThrow();
        packs.Length.ShouldBe(3);

        packs
            .Select(x => x.PackId)
            .ShouldBeEquivalentTo(createdPacks.Select(x => x.Id));
    }

    [Fact]
    public async Task GetShortPacks_DoesNotReturnOtherUsersPacks()
    {
        var user = await CreateUserAsync();
        var anotherUser = await CreateUserAsync();

        await CreatePacksAsync(user.Id, 2);
        await CreatePacksAsync(anotherUser.Id, 3);

        var result = await PacksService.GetShortPacks(user.Id, new GetPackRequest());

        result.IsSuccess.ShouldBeTrue();
        result.GetValueOrThrow().Length.ShouldBe(2);
    }

    [Fact]
    public async Task GetShortPacks_WhenTakeSpecified_ReturnsLimitedNumberOfPacks()
    {
        var user = await CreateUserAsync();
        await CreatePacksAsync(user.Id, 5);

        var result = await PacksService.GetShortPacks(user.Id, new GetPackRequest { Take = 2 });
        result.IsSuccess.ShouldBeTrue();
        result.GetValueOrThrow().Length.ShouldBe(2);
    }

    [Fact]
    public async Task GetShortPacks_WhenSkipSpecified_SkipsRequestedNumberOfPacks()
    {
        var user = await CreateUserAsync();
        var createdPacks = await CreatePacksAsync(user.Id, 5);

        var result = await PacksService.GetShortPacks(user.Id, new GetPackRequest { Skip = 2 });
        result.IsSuccess.ShouldBeTrue();

        var packs = result.GetValueOrThrow();
        packs.Length.ShouldBe(3);

        packs
            .Select(x => x.PackId)
            .ShouldBeEquivalentTo(createdPacks
                .Skip(2)
                .Select(x => x.Id));
    }
}
