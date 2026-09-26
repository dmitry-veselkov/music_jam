using MusicJam.Domain.Models.Auth;
using MusicJam.Domain.Models.Common;

namespace MusicJam.Domain.Services.AuthServices;

public interface IAuthService
{
    public enum AuthError
    {
        SameUserAlreadyExists,
    }

    Task<Result<AuthError>> Login(AuthCredentials credentials);
}