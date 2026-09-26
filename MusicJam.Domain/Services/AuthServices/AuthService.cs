using MusicJam.Domain.Models.Auth;
using MusicJam.Domain.Models.Common;

namespace MusicJam.Domain.Services.AuthServices;

internal class AuthService : IAuthService
{
    public async Task<Result<IAuthService.AuthError>> Login(AuthCredentials credentials)
    {
        await Task.CompletedTask;

        return IAuthService.AuthError.SameUserAlreadyExists;
    }
}