using Microsoft.EntityFrameworkCore;
using MusicJam.Studio.Infrustructure.Data.Configurations;
using DbGame = MusicJam.Studio.Infrustructure.Entities.Game;
using Game = MusicJam.Studio.Models.Games.Game;

namespace MusicJam.Studio.Storages;

public interface IGameStorage
{
    Task<List<Game>> GetAllUserGames(Guid userId);

    Task<Game> SaveGame(Guid userId, Game game);
    
    Task Delete(Guid userId, Guid gameId);

    Task<Game?> GetGame(Guid userId, Guid gameId);
    
    Task<Game> UpdateGame(Guid userId, Game game);

}