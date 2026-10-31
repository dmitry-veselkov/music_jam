using Microsoft.EntityFrameworkCore;
using MusicJam.Studio.Infrustructure.Data.Configurations;
using DbGame = MusicJam.Studio.Infrustructure.Entities.Game;
using Game = MusicJam.Studio.Models.Games.Game;

namespace MusicJam.Studio.Storages;

public interface IGameStorage
{
    Task<List<Game>> GetAllUserGames(Guid userId, int? take = null, int skip = 0);

    Task<Game> SaveGame(Guid userId, Game game);
    
    Task<bool?> Delete(Guid gameId);

    Task<Game?> GetGame(Guid gameId);
    
    Task<Game?> UpdateGame(Game game);

}