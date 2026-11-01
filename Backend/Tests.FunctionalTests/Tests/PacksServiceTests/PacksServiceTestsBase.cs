using MusicJam.Studio.Infrustructure.Entities;
using MusicJam.Studio.Requests;
using MusicJam.Studio.Requests.Packs;
using MusicJam.Studio.Services.PackService;
using Tests.FunctionalTests.Fixtures;
using DbPack = MusicJam.Studio.Infrustructure.Entities.Pack;
using DomainPack = MusicJam.Studio.Models.Packs.Pack;
using Round = MusicJam.Studio.Models.Packs.Round;

namespace Tests.FunctionalTests.Tests.PacksServiceTests;

public abstract class PacksServiceTestBase(TestApplicationFixture fixture) : FunctionalTestBase(fixture)
{
    protected const string TestPackName = "Test pack";
    protected readonly Guid TestPackId = Guid.Empty;

    protected IPacksService PacksService => GetService<IPacksService>();

    protected async Task<User> CreateUserAsync(
        IEnumerable<DbPack>? packs = null)
    {
        var user = new User
        {
            Username = "John Doe",
            Email = "example.com",
            PasswordHash = "hash",
            Packs = packs?.ToList() ?? []
        };

        await SeedAsync(user);

        return user;
    }

    protected async Task<DbPack> CreatePackAsync(Guid userId, string title, DateTime? createdAt = null)
    {
        var now = DateTime.UtcNow;

        var pack = new DbPack
        {
            Id = TestPackId,
            UserId = userId,
            Title = title,
            CreatedAtUtc = createdAt ?? now,
            UpdatedAtUtc = now,
            Rounds = []
        };

        await SeedAsync(pack);

        return pack;
    }

    protected async Task<List<DbPack>> CreatePacksAsync(
        Guid userId,
        int count)
    {
        var packs = Enumerable.Range(1, count)
            .Select(i =>
            {
                var createdAt = DateTime.UtcNow;

                return new DbPack
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Title = $"Pack {i}",
                    CreatedAtUtc = createdAt,
                    UpdatedAtUtc = createdAt,
                    Rounds = []
                };
            })
            .ToList();

        foreach (var pack in packs)
        {
            await SeedAsync(pack);
        }

        return packs;
    }

    protected async Task<Question> CreateQuestionAsync(
        Guid userId, string requested, string correctAnswer)
    {
        var question = new Question
        {
            UserId = userId,
            Requested = requested,
            CorrectAnswer = correctAnswer,
        };

        await SeedAsync(question);

        return question;
    }

    protected static DomainPack CreateDomainPack(
        string name = "Test pack",
        List<Round>? rounds = null,
        Guid? id = null)
    {
        var now = DateTime.UtcNow;

        return new DomainPack
        {
            Id = id ?? Guid.NewGuid(),
            Name = name,
            Rounds = rounds ?? [],
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };
    }

    protected static CreatePackRequest BuildCreatePackRequest(
        string name = TestPackName,
        Guid? packId = null,
        List<CreateRoundRequest>? rounds = null)
    {
        return new CreatePackRequest
        {
            PackId = packId ?? Guid.NewGuid(),
            Name = name,
            Rounds = rounds ?? [BuildCreateRoundRequest()]
        };
    }

    protected static CreateRoundRequest BuildCreateRoundRequest(
        string name = "Test round",
        List<List<RoundQuestionRequest>>? questions = null)
    {
        return new CreateRoundRequest
        {
            Name = name,
            Questions = questions ??
            [
                [
                    BuildRoundQuestionRequest()
                ]
            ]
        };
    }

    protected static RoundQuestionRequest BuildRoundQuestionRequest(
        string category = "Test category",
        int cost = 100,
        Guid? questionId = null)
    {
        return new RoundQuestionRequest
        {
            Category = category,
            Cost = cost,
            QuestionId = questionId
        };
    }
}
