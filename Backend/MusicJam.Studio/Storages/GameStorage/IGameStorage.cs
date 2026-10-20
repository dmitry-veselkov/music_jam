using Microsoft.EntityFrameworkCore;
using MusicJam.Studio.Infrustructure.Data.Configurations;
using DbGame = MusicJam.Studio.Infrustructure.Entities.Game;
using Game = MusicJam.Studio.Models.Games.Game;

namespace MusicJam.Studio.Storages;

public interface IGameStorage
{
    Task<List<Game>> GetAllUserGames(Guid userId);

    Task<Game> SaveGame(Guid userId, Game game);
    
    Task Delete(Guid gameId);

    Task GetGame(Guid userId, Guid gameId);
    
    Task UpdateGame(Guid userId, Game game);

}