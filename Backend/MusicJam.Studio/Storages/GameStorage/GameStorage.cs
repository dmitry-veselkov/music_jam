using Microsoft.EntityFrameworkCore;
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
        var entity = ToDbModel(game, userId);
        db.Games.Add(entity);
        await db.SaveChangesAsync();
        return ToModel(entity);
    }

    public async Task Delete(Guid userId, Guid gameId)
    {
        var deleted =  await db.Games
            .Where(x => x.UserId == userId && x.Id == gameId)
            .FirstOrDefaultAsync();
        db.Games.Remove(deleted);
        await db.SaveChangesAsync();
    }

    public async Task<Game?> GetGame(Guid userId, Guid gameId)
    {
        var game = db.Games
            .FirstOrDefault(x => x.Id == gameId && x.UserId == userId);
        return game is null ? null : ToModel(game);
    }

    public async Task<Game> UpdateGame(Guid userId, Game game)
    {
        var entity = await db.Games
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == game.Id);
        if (entity is null)
            return null;
 
        entity.Title = game.Name;
        entity.Description = game.Description;
        entity.GameTime = ToUtc(game.PlannedStart);
        entity.Mode = game.Mode;
        entity.State = game.State;
        entity.PackId = game.Pack.Id;
 
        await db.SaveChangesAsync();
        return ToModel(entity);
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
        UserId = userId,
        PackId = e.Pack.Id,
        Title = e.Name,
        Description = e.Description,
        GameTime = ToUtc(e.PlannedStart),
        Mode = e.Mode,
        State = e.State
    };
    
    private static DateTime ToUtc(DateTime d) => d.Kind switch
    {
        DateTimeKind.Utc => d,
        DateTimeKind.Local => d.ToUniversalTime(),
        _ => DateTime.SpecifyKind(d, DateTimeKind.Utc)
    };
}
