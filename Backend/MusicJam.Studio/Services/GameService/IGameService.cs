using MusicJam.Studio.Models.Games;
using MusicJam.Studio.Requests;
using Shared;

namespace MusicJam.Studio.Services.GameService;

public interface IGameService
{
    public enum GameError
    {
        NotFound,        
        InvalidRequest,
    }
    
    Task<Result<List<Game>, GameError>> GetGames(Guid userId, GetGameRequests query);

    Task<Result<Game, GameError>> Create(Guid userId, CreateGameRequests query);

    Task<Result<Game, GameError>> Get(Guid gameId);

    Task<Result<Guid, IGameService.GameError>> Delete(Guid gameId);

    Task<Result<Game, IGameService.GameError>> Update(Guid gameId, UpdateGameRequests request);
}