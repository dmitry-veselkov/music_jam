using MusicJam.Studio.Requests.Auth;
using MusicJam.Studio.Services.AuthService;
using Shouldly;
using Tests.FunctionalTests.Extensions;
using Tests.FunctionalTests.Fixtures;

namespace Tests.FunctionalTests.Tests.AuthServiceTests;

[Collection(TestCollection.Name)]
public class LoginTests(TestApplicationFixture fixture) : AuthServiceTestBase(fixture)
{
    private readonly LoginRequest _loginRequest = new()
    {
        Email = TestUserEmail,
        Password = TestUserPassword,
    };

    [Fact]
    public async Task Login_WhenUserDoesNotExist_ReturnsNotFound()
    {
        var result = await AuthService.Login(_loginRequest);

        result.IsSuccess.ShouldBeFalse();
        result.GetFaultOrThrow().ShouldBe(IAuthService.AuthError.UserNotFound);
    }

    [Fact]
    public async Task Login_WhenUserExists_ReturnsAccessToken()
    {
        await CreateUserAsync(TestUserEmail, TestUserPassword);

        var result = await AuthService.Login(_loginRequest);
        result.IsSuccess.ShouldBeTrue();

        var accessToken = result.GetValueOrDefault();
        accessToken.ShouldNotBeNullOrWhiteSpace();
        accessToken.ShouldBeValidAccessToken();
    }
}
