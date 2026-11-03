using MusicJam.Studio.Requests.Auth;
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

    Task<Result<string, AuthError>> Login(LoginRequest request);

    Task<Result<string, AuthError>> Register(RegisterRequest request);
}
