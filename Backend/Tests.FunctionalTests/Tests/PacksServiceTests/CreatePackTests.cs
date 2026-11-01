using MusicJam.Studio.Requests.Packs;
using Shouldly;
using Tests.FunctionalTests.Fixtures;

namespace Tests.FunctionalTests.Tests.PacksServiceTests;

[Collection(TestCollection.Name)]
public sealed class CreatePackTests(TestApplicationFixture fixture) : PacksServiceTestBase(fixture)
{
    [Fact]
    public async Task Create_WhenRequestIsValid_CreatesPack()
    {
        var user = await CreateUserAsync();
        var request = BuildCreatePackRequest();

        var result = await PacksService.Create(user.Id, request);

        result.IsSuccess.ShouldBeTrue();

        var createdPack = result.GetValueOrThrow();
        createdPack.Id.ShouldBe(request.PackId);
        createdPack.Name.ShouldBe(request.Name);
        createdPack.Rounds.ShouldHaveSingleItem();
        createdPack.Rounds[0].Name.ShouldBe(request.Rounds[0].Name);

        var getResult = await PacksService.GetPack(user.Id, request.PackId);

        getResult.IsSuccess.ShouldBeTrue();

        var savedPack = getResult.GetValueOrThrow();
        savedPack.Id.ShouldBe(request.PackId);
        savedPack.Name.ShouldBe(request.Name);
        savedPack.Rounds.ShouldHaveSingleItem();
    }

    [Fact]
    public async Task Create_WhenUserAlreadyHasPacks_CreatesAnotherPack()
    {
        var user = await CreateUserAsync();
        await CreatePacksAsync(user.Id, 2);

        var request = BuildCreatePackRequest();

        var result = await PacksService.Create(user.Id, request);

        result.IsSuccess.ShouldBeTrue();
        result.GetValueOrThrow().Id.ShouldBe(request.PackId);

        var packsResult = await PacksService.GetShortPacks(user.Id, new GetPackRequest());

        packsResult.IsSuccess.ShouldBeTrue();
        packsResult.GetValueOrThrow().Length.ShouldBe(3);
    }

    [Fact]
    public async Task Create_WhenRequestContainsMultipleRounds_CreatesAllRounds()
    {
        var user = await CreateUserAsync();

        var request = BuildCreatePackRequest(
            rounds:
            [
                BuildCreateRoundRequest("Round 1"),
                BuildCreateRoundRequest("Round 2"),
                BuildCreateRoundRequest("Round 3")
            ]);

        var result = await PacksService.Create(user.Id, request);

        result.IsSuccess.ShouldBeTrue();

        var pack = result.GetValueOrThrow();
        pack.Rounds.Count.ShouldBe(3);
        pack.Rounds
            .Select(x => x.Name)
            .ShouldBeEquivalentTo(new[] { "Round 1", "Round 2", "Round 3" });
    }

    [Fact]
    public async Task Create_WhenRoundContainsMultipleCells_CreatesRoundWithAllCells()
    {
        var user = await CreateUserAsync();

        var request = BuildCreatePackRequest(
            rounds:
            [
                BuildCreateRoundRequest(
                    questions:
                    [
                        [
                            BuildRoundQuestionRequest("Category 1", 100),
                            BuildRoundQuestionRequest("Category 1", 200)
                        ],
                        [
                            BuildRoundQuestionRequest("Category 2", 100),
                            BuildRoundQuestionRequest("Category 2", 200)
                        ]
                    ])
            ]);

        var result = await PacksService.Create(user.Id, request);

        result.IsSuccess.ShouldBeTrue();

        var pack = result.GetValueOrThrow();
        pack.Rounds.ShouldHaveSingleItem();

        var round = pack.Rounds[0];
        round.Categories.Count.ShouldBe(2);
        round.Categories.ShouldContain("Category 1");
        round.Categories.ShouldContain("Category 2");
    }
}
