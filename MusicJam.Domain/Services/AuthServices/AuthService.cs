using MusicJam.Domain.Helpers;
using MusicJam.Domain.Models.Auth;
using MusicJam.Domain.Models.Common;
using MusicJam.Domain.Services.Tokens;
using MusicJam.Domain.Storages;

namespace MusicJam.Domain.Services.AuthServices;

internal class AuthService(
    IUsersStorage usersStorage,
    ITokenService tokenService)
    : IAuthService
{
    public async Task<Result<string, IAuthService.AuthError>> Login(AuthCredentials credentials)
    {
        var user = await usersStorage.TryGetUserByEmail(credentials.Email);
        if (user is null)
        {
            return IAuthService.AuthError.UserNotFound;
        }

        if (!PasswordHelper.Verify(credentials.Password, user.PasswordHash))
        {
            return IAuthService.AuthError.InvalidCredentials;
        }

        return tokenService.GenerateToken(user);
    }
}