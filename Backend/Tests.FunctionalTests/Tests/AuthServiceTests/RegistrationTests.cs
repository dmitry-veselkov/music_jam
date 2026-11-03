using MusicJam.Studio.Requests.Auth;
using MusicJam.Studio.Services.AuthService;
using Shouldly;
using Tests.FunctionalTests.Extensions;
using Tests.FunctionalTests.Fixtures;

namespace Tests.FunctionalTests.Tests.AuthServiceTests;

[Collection(TestCollection.Name)]
public class RegistrationTests(TestApplicationFixture fixture) : AuthServiceTestBase(fixture)
{
    private readonly RegisterRequest _registerRequest = new()
    {
        Username = TestUserName,
        Email = TestUserEmail,
        Password = TestUserPassword,
    };

    [Fact]
    public async Task Register_WhenUserDoesNotExist_CreateUserAndReturnsAccessToken()
    {
        var result = await AuthService.Register(_registerRequest);

        result.IsSuccess.ShouldBeTrue();

        var accessToken = result.GetValueOrThrow();
        accessToken.ShouldNotBeNullOrWhiteSpace();
        accessToken.ShouldBeValidAccessToken();
    }

    [Fact]
    public async Task Register_WhenUserExists_ReturnsSameUserAlreadyExists()
    {
        await CreateUserAsync(TestUserEmail, TestUserPassword);

        var result = await AuthService.Register(_registerRequest);

        result.IsSuccess.ShouldBeFalse();
        result.GetFaultOrThrow().ShouldBe(IAuthService.AuthError.SameEmailAlreadyExists);
    }
}
