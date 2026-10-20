using MusicJam.Studio.Infrustructure.Data.Configurations;
using DbGame = MusicJam.Studio.Infrustructure.Entities.Game;
using Game = MusicJam.Studio.Models.Games.Game;

namespace MusicJam.Studio.Storages;

public class GameStorage(AppDbContext db) : IGameStorage
{
    private IGameStorage _gameStorageImplementation;
    public async Task<List<Game>> GetAllUserGames(Guid userId)
    {
        return db.Games
            .Where(x => x.UserId == userId)
            .Select(x => ToModel(x))
            .ToList();
    }

    public async Task<Game> SaveGame(Guid userId, Game game)
    {
        return await db.Games.Add(ToDbModel(game, userId));
    }

    public Task Delete(Guid gameId)
    {
        return _gameStorageImplementation.Delete(gameId);
    }

    public Task GetGame(Guid userId, Guid gameId)
    {
        return _gameStorageImplementation.GetGame(userId, gameId);
    }

    public Task UpdateGame(Guid userId, Game game)
    {
        return _gameStorageImplementation.UpdateGame(userId, game);
    }
    
    private static Game ToModel(DbGame e) => new()
    {
        Id = e.Id,
        Description = e.Description,
        PlannedStart = e.GameTime,
        Mode = e.Mode,
        State = e.State
    };
    
    private static DbGame ToDbModel(Game e, Guid userId) => new()
    {
        Description = e.Description,
        PackId = PackStorage.e.Pack,
        Mode = e.Mode,
        State = e.State,
        UserId = userId,
        Pack = e.Pack,
        Title = e.Name,
        GameTime = e.PlannedStart
    };
}