using MusicJam.Studio.Helpers;
using MusicJam.Studio.Infrustructure.Entities;
using MusicJam.Studio.Services.AuthService;
using Tests.FunctionalTests.Fixtures;

namespace Tests.FunctionalTests.Tests.AuthServiceTests;

public abstract class AuthServiceTestBase(TestApplicationFixture fixture) : FunctionalTestBase(fixture)
{
    protected IAuthService AuthService => GetService<IAuthService>();

    protected async Task CreateUserAsync(string email, string password)
    {
        var user = new User
        {
            Username = "John Doe",
            Email = email,
            PasswordHash = PasswordHelper.Hash(password),
            Games = [],
            Packs = [],
            Questions = []
        };

        await SeedAsync(user);
    }
}
