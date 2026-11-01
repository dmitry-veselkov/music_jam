using MusicJam.Studio.Infrustructure.Entities;
using MusicJam.Studio.Services.PackService;
using Tests.FunctionalTests.Fixtures;

namespace Tests.FunctionalTests.Tests.PacksServiceTests;

public abstract class PacksServiceTestBase(TestApplicationFixture fixture) : FunctionalTestBase(fixture)
{
    protected IPacksService PacksService => GetService<IPacksService>();

    protected async Task<User> CreateUserAsync(IEnumerable<Pack> packs)
    {
        var user = new User
        {
            Packs = packs.ToList(),
        };

        await SeedAsync(user);

        return user;
    }

    protected async Task<Pack> CreatePackAsync(Guid userId, string title, IEnumerable<PackRound> rounds)
    {
        var now = DateTime.UtcNow;

        var pack = new Pack
        {
            Id = Guid.Empty,
            UserId = userId,
            Title = title,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
            Rounds = rounds.ToList()
        };

        await SeedAsync(pack);

        return pack;
    }

    protected async Task<Question> CreateQuestionAsync()
    {
        var question = new Question
        {
        };

        await SeedAsync(question);

        return question;
    }
}
