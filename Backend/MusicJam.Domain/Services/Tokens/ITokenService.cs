using MusicJam.Domain.Models;

namespace MusicJam.Domain.Services.Tokens;

public interface ITokenService
{
    string GenerateToken(User user);
}