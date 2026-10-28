using Microsoft.EntityFrameworkCore;
using MusicJam.Studio.Infrustructure.Data.Configurations;
using DbGame = MusicJam.Studio.Infrustructure.Entities.Game;
using Game = MusicJam.Studio.Models.Games.Game;
using Pack = MusicJam.Studio.Models.Packs.Pack;

namespace MusicJam.Studio.Storages;

public class GameStorage(AppDbContext db) : IGameStorage
{
    private IGameStorage _gameStorageImplementation;
    public async Task<List<Game>> GetAllUserGames(Guid userId)
    {
        var entities = await db.Games
            .AsNoTracking()
            .Include(x => x.Pack)
            .Where(x => x.UserId == userId)
            .ToListAsync();
        var result = entities
            .Select(ToModel)
            .ToList();
        Console.WriteLine(result.Count);
        return result;
    }

    public async Task<Game> SaveGame(Guid userId, Game game)
    {
        var entity = ToDbModel(game, userId);
        // Id у Entity с protected set — задаём через EF, чтобы сохранить id, пришедший с фронта
        db.Entry(entity).Property(x => x.Id).CurrentValue = game.Id;
        db.Games.Add(entity);
        await db.SaveChangesAsync();
        return ToModel(entity);
    }

    public async Task<bool?> Delete(Guid gameId)
    {
        var deleted =  await db.Games
            .Where(x => x.Id == gameId)
            .FirstOrDefaultAsync();
        if (deleted is null) return null;
        db.Games.Remove(deleted);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<Game?> GetGame(Guid gameId)
    {
        var game = await db.Games
            .Include(x => x.Pack)
            .FirstOrDefaultAsync(x => x.Id == gameId);
        return game is null ? null : ToModel(game);
    }

    public async Task<Game?> UpdateGame(Game game)
    {
        var entity = await db.Games
            .FirstOrDefaultAsync(x => x.Id == game.Id);
        if (entity is null)
            return null;
 
        entity.Title = game.Name;
        entity.Description = game.Description;
        entity.GameTime = ToUtc(game.PlannedStart);
        entity.Mode = game.Mode;
        entity.PackId = game.Pack.Id;
 
        await db.SaveChangesAsync();
        return ToModel(entity);
    }
    
    private static Game ToModel(DbGame e) => new()
    {
        Id = e.Id,
        Name = e.Title,
        Pack = new Pack { Id = e.PackId, Name = e.Pack?.Title ?? string.Empty },
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
