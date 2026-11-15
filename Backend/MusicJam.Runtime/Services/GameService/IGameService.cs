using Microsoft.AspNetCore.Mvc;
using MusicJam.Runtime.Models;

namespace MusicJam.Runtime.Services.GameService;

public interface IGameService
{
    public Task<ActionResult<Game>> Create(Guid hostId, Guid packId);
    
    public Task<ActionResult<Game>> Get(Guid gameId);
}
