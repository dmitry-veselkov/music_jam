using Microsoft.EntityFrameworkCore;
using MusicJam.Studio.Infrastructure.Data;
using MusicJam.Studio.Infrastructure.Entities;
using MusicJam.Studio.Storages.RoundStorage;
using DbPack = MusicJam.Studio.Infrastructure.Entities.Pack;
using Pack = MusicJam.Studio.Models.Packs.Pack;
using Round = MusicJam.Studio.Models.Packs.Round;

namespace MusicJam.Studio.Storages;

public class PackStorage(AppDbContext db, IRoundStorage roundStorage) : IPackStorage
{
    public async Task<List<Pack>> GetPacksByUser(Guid userId, int? take = null, int skip = 0)
    {
        var query = db.Packs
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.CreatedAtUtc)
            .Skip(skip);

        if (take.HasValue)
            query = query.Take(take.Value);

        var packsDb = await query.ToListAsync();
        var packs = new List<Pack>();
        foreach (var packDb in packsDb)
        {
            packs.Add(new Pack()
            {
                Id = packDb.Id,
                Name = packDb.Title,
                CreatedAtUtc = packDb.CreatedAtUtc,
                Rounds = await roundStorage.GetByPackId(packDb.Id),
                UpdatedAtUtc = packDb.UpdatedAtUtc,
            });
        }
        return packs;
    }
    
    public Task<List<PackShortView>> GetShortPacksByUser(Guid userId, int? take = null, int skip = 0)
    {
        var query = db.Packs
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.CreatedAtUtc)
            .Skip(skip);

        if (take.HasValue)
            query = query.Take(take.Value);

        return query
            .Select(x => new PackShortView(x.Id, x.Title))
            .ToListAsync();
    }
    
    public Task<List<PackCardView>> GetCardPacksByUser(Guid userId, int? take = null, int skip = 0)
    {
        var query = db.Packs
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.CreatedAtUtc)
            .Skip(skip);

        if (take.HasValue)
            query = query.Take(take.Value);

        return query
            .Select(x => new PackCardView(x.Id, x.Title, x.CreatedAtUtc, x.UpdatedAtUtc))
            .ToListAsync();
    }

    public async Task<Pack?> GetPackByUser(Guid userId, Guid packId)
    {
        var packsDb = db.Packs
            .FirstOrDefault(x => x.UserId == userId && x.Id == packId);
        if (packsDb is null)
            return null;
        var pack = new Pack()
        {
            Id = packsDb.Id,
            Name = packsDb.Title,
            CreatedAtUtc = packsDb.CreatedAtUtc,
            Rounds = await roundStorage.GetByPackId(packsDb.Id),
            UpdatedAtUtc = packsDb.UpdatedAtUtc,
        };
        return pack;
    }

    public async Task<Pack> SavePack(Guid userId, Pack pack)
    {
        var now = DateTime.UtcNow;
        var packDb = new DbPack
        {
            Id = pack.Id,
            UserId = userId,
            Title = pack.Name,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };
        
        await using var tx = await db.Database.BeginTransactionAsync();
        db.Packs.Add(packDb);
        await db.SaveChangesAsync();
        
        var rounds = new List<Round>();
        var order = 0;
        foreach (var round in pack.Rounds)
        {
            var (saved, _) = await roundStorage.Save(packDb.Id, order, round);
            rounds.Add(saved);
            order++;
        }
        
        await tx.CommitAsync();
        return ToModel(packDb, rounds);
    }

    public async Task<bool> DeletePack(Guid userId, Guid packId)
    {
        var exists = await db.Packs.AnyAsync(x => x.Id == packId && x.UserId == userId);
        if (!exists)
            return false;
        await using var tx = await db.Database.BeginTransactionAsync();
        
        var roundIds = await db.PackRounds
            .Where(x => x.PackId == packId)
            .Select(x => x.RoundId)
            .ToListAsync();
        foreach (var roundId in roundIds)
            await roundStorage.Delete(roundId);
        
        await db.Packs.Where(x => x.Id == packId).ExecuteDeleteAsync();
        await tx.CommitAsync();
        return true;
    }

    public async Task<Pack> Update(Guid userId, Pack pack)
    {
        var packDb = await db.Packs
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == pack.Id);
        if (packDb is null)
            return await SavePack(userId, pack);

        await using var tx = await db.Database.BeginTransactionAsync();
        packDb.Title = pack.Name;
        packDb.UpdatedAtUtc = DateTime.UtcNow;
        await db.SaveChangesAsync();

        var roundIds = await db.PackRounds
            .Where(x => x.PackId == packDb.Id)
            .Select(x => x.RoundId)
            .ToListAsync();
        foreach (var roundId in roundIds)
            await roundStorage.Delete(roundId);
 
        var rounds = new List<Round>();
        var order = 0;
        foreach (var round in pack.Rounds)
        {
            var (saved, _) = await roundStorage.Save(packDb.Id, order, round);
            rounds.Add(saved);
            order++;
        }
        await tx.CommitAsync();
        return ToModel(packDb, rounds);
    }
    
    public async Task<List<DateTime>> GetAllUsages(Guid packId)
    {
        return await db.HistoryPacks
            .Where(x => x.PackId == packId)
            .Select(x => x.Date)
            .ToListAsync();
    }

    public static Pack ToModel(DbPack e, List<Round> rounds) => new()
    {
        Id = e.Id,
        Name = e.Title,
        Rounds = rounds,
        CreatedAtUtc = e.CreatedAtUtc,
        UpdatedAtUtc = e.UpdatedAtUtc
    };
}