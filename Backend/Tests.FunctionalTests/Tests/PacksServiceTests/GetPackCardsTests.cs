using MusicJam.Studio.Requests.Packs;
using Shouldly;
using Tests.FunctionalTests.Fixtures;

namespace Tests.FunctionalTests.Tests.PacksServiceTests;

[Collection(TestCollection.Name)]
public sealed class GetPackCardsTests(TestApplicationFixture fixture) : PacksServiceTestBase(fixture)
{
    [Fact]
    public async Task GetPackCards_WhenUserHasNoPacks_ReturnsEmptyArray()
    {
        var user = await CreateUserAsync();

        var result = await PacksService.GetPackCards(user.Id, new GetPackRequest());

        result.IsSuccess.ShouldBeTrue();
        result.GetValueOrThrow().ShouldBeEmpty();
    }

    [Fact]
    public async Task GetPackCards_WhenUserHasPacks_ReturnsAllPacks()
    {
        var user = await CreateUserAsync();
        var packs = await CreatePacksAsync(user.Id, 3);

        var result = await PacksService.GetPackCards(user.Id, new GetPackRequest());
        result.IsSuccess.ShouldBeTrue();

        var packCards = result.GetValueOrThrow();

        packCards.Length.ShouldBe(3);
        packCards
            .Select(x => x.PackId)
            .ShouldBeEquivalentTo(packs.Select(x => x.Id));
    }

    [Fact]
    public async Task GetPackCards_WhenOtherUsersHavePacks_ReturnsOnlyOwnPacks()
    {
        var user = await CreateUserAsync();
        var anotherUser = await CreateUserAsync();

        var ownPacks = await CreatePacksAsync(user.Id, 2);
        await CreatePacksAsync(anotherUser.Id, 3);

        var result = await PacksService.GetPackCards(user.Id, new GetPackRequest());
        result.IsSuccess.ShouldBeTrue();

        var packCards = result.GetValueOrThrow();

        packCards.Length.ShouldBe(2);
        packCards
            .Select(x => x.PackId)
            .ShouldBeEquivalentTo(ownPacks.Select(x => x.Id));
    }

    [Fact]
    public async Task GetPackCards_WhenTakeIsSpecified_ReturnsLimitedNumberOfPacks()
    {
        var user = await CreateUserAsync();
        var packs = await CreatePacksAsync(user.Id, 5);

        var result = await PacksService.GetPackCards(user.Id, new GetPackRequest { Take = 2 });

        result.IsSuccess.ShouldBeTrue();

        var packCards = result.GetValueOrThrow();

        packCards.Length.ShouldBe(2);
        packCards
            .Select(x => x.PackId)
            .ShouldBeEquivalentTo(packs
                .Take(2)
                .Select(x => x.Id));
    }

    [Fact]
    public async Task GetPackCards_WhenSkipIsSpecified_SkipsRequestedNumberOfPacks()
    {
        var user = await CreateUserAsync();
        var packs = await CreatePacksAsync(user.Id, 5);

        var result = await PacksService.GetPackCards(
            user.Id,
            new GetPackRequest { Skip = 2 });

        result.IsSuccess.ShouldBeTrue();

        var packCards = result.GetValueOrThrow();

        packCards.Length.ShouldBe(3);
        packCards
            .Select(x => x.PackId)
            .ShouldBeEquivalentTo(packs
                .Skip(2)
                .Select(x => x.Id));
    }
}
