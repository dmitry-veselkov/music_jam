using MusicJam.Studio.Models.Users;

namespace MusicJam.Studio.Services.TokenService;

public interface ITokenService
{
    string GenerateToken(User user);
}
