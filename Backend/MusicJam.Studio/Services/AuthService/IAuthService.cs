using MusicJam.Studio.Entities;
using MusicJam.Studio.Requests;
using Shared;

namespace MusicJam.Studio.Services.AuthService;

public interface IAuthService
{
    public enum AuthError
    {
        UserNotFound,
        InvalidCredentials,
        SameEmailAlreadyExists,
    }

    Task<Result<string, AuthError>> Login(AuthRequest request);

    Task<Result<string, AuthError>> Register(RegisterRequest request);
}
