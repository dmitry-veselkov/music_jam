using MusicJam.Studio.Helpers;
using MusicJam.Studio.Models.Users;
using MusicJam.Studio.Requests;
using MusicJam.Studio.Services.TokenService;
using MusicJam.Studio.Storages.UsersStorage;
using Shared;

namespace MusicJam.Studio.Services.AuthService;

internal class AuthService(
    IUsersStorage usersStorage,
    ITokenService tokenService)
    : IAuthService
{
    public async Task<Result<string, IAuthService.AuthError>> Login(AuthRequest request)
    {
        var user = await usersStorage.TryGetUserByEmail(request.Email);
        if (user is null)
        {
            return IAuthService.AuthError.UserNotFound;
        }

        if (!PasswordHelper.Verify(request.Password, user.Password))
        {
            return IAuthService.AuthError.InvalidCredentials;
        }

        return tokenService.GenerateToken(user);
    }

    public async Task<Result<string, IAuthService.AuthError>> Register(RegisterRequest request)
    {
        var user = await usersStorage.TryGetUserByEmail(request.Email);
        if (user is not null)
        {
            return IAuthService.AuthError.SameEmailAlreadyExists;
        }

        var passwordHash = PasswordHelper.Hash(request.Password);

        user = new User(request.Email, passwordHash, request.Username);
        var saved = await usersStorage.SaveUser(user);

        return tokenService.GenerateToken(saved);
    }
}
