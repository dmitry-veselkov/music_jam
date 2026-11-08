using Microsoft.AspNetCore.Mvc;
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
    
    public Task<ActionResult<Game>> Get(Guid gameId);
}
