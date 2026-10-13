using MusicJam.Studio.Entities;

namespace MusicJam.Studio.Services.TokenService;

public interface ITokenService
{
    string GenerateToken(User user);
}
