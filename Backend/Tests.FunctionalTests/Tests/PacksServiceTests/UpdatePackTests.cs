using MusicJam.Studio.Requests.Packs;
using Shouldly;
using Tests.FunctionalTests.Fixtures;

namespace Tests.FunctionalTests.Tests.PacksServiceTests;

[Collection(TestCollection.Name)]
public sealed class UpdatePackTests(TestApplicationFixture fixture) : PacksServiceTestBase(fixture)
{
    [Fact]
    public async Task Update_WhenPackExists_UpdatesPackName()
    {
        var user = await CreateUserAsync();
        var pack = await CreatePackAsync(user.Id, TestPackName);
        const string newName = "Updated pack";

        var request = new UpdatePackRequest
        {
            Name = newName,
            Rounds = [BuildCreateRoundRequest()]
        };

        var result = await PacksService.Update(user.Id, pack.Id, request);

        result.IsSuccess.ShouldBeTrue();

        var updatedPack = result.GetValueOrThrow();
        updatedPack.Id.ShouldBe(pack.Id);
        updatedPack.Name.ShouldBe(newName);

        var getResult = await PacksService.GetPack(user.Id, pack.Id);

        getResult.IsSuccess.ShouldBeTrue();
        getResult.GetValueOrThrow().Name.ShouldBe(newName);
    }

    [Fact]
    public async Task Update_WhenPackExists_UpdatesRounds()
    {
        var user = await CreateUserAsync();
        var pack = await CreatePackAsync(user.Id, TestPackName);

        var request = new UpdatePackRequest
        {
            Name = pack.Title,
            Rounds =
            [
                BuildCreateRoundRequest("Updated round 1"),
                BuildCreateRoundRequest("Updated round 2")
            ]
        };

        var result = await PacksService.Update(user.Id, pack.Id, request);

        result.IsSuccess.ShouldBeTrue();

        var updatedPack = result.GetValueOrThrow();
        updatedPack.Rounds.Count.ShouldBe(2);
        updatedPack.Rounds
            .Select(x => x.Name)
            .ShouldBeEquivalentTo(new[] { "Updated round 1", "Updated round 2" });
    }

    [Fact]
    public async Task Update_WhenPackDoesNotExist_CreatesPack()
    {
        var user = await CreateUserAsync();
        var packId = Guid.NewGuid();

        var request = new UpdatePackRequest
        {
            Name = TestPackName,
            Rounds = [BuildCreateRoundRequest()]
        };

        var result = await PacksService.Update(user.Id, packId, request);

        result.IsSuccess.ShouldBeTrue();

        var createdPack = result.GetValueOrThrow();
        createdPack.Id.ShouldBe(packId);
        createdPack.Name.ShouldBe(TestPackName);
        createdPack.Rounds.ShouldHaveSingleItem();

        var getResult = await PacksService.GetPack(user.Id, packId);

        getResult.IsSuccess.ShouldBeTrue();
        getResult.GetValueOrThrow().Name.ShouldBe(TestPackName);
    }
}
