using MusicJam.Runtime.Models;
using Shared;

namespace MusicJam.Runtime.Services.GameService;

public interface IGameService
{
    
    public enum GameError
    {
        GameNotFound,
    }
    
    public Task<Result<Game, GameError>> Create(Guid hostId, Guid packId);
    
    public Task<Result<Game, GameError>> Get(Guid gameId);
}
