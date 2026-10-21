using Microsoft.EntityFrameworkCore;
using MusicJam.Studio.Infrustructure.Data.Configurations;
using MusicJam.Studio.Models.Packs;

using DbPack = MusicJam.Studio.Infrustructure.Entities.Pack;
using Pack = MusicJam.Studio.Models.Packs.Pack;

namespace MusicJam.Studio.Storages;

public class PackStorage(AppDbContext db, IRoundStorage roundStorage) : IPackStorage
{
    public async Task<List<Pack>> TryGetPacksByUser(Guid userId)
    {
        var packsDb = await db.Packs
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.CreatedAtUtc)
            .ToListAsync();
        var packs = new List<Pack>();
        foreach (var packDb in packsDb)
        {
            packs.Add(new Pack()
            {
                Id = packDb.Id,
                Name = packDb.Title,
                CreatedAtUtc = packDb.CreatedAtUtc,
                Rounds = await roundStorage.TryGetByPackId(packDb.Id),
                UpdatedAtUtc = packDb.UpdatedAtUtc,
            });
        }
        return packs;
    }

    public async Task<Pack?> TryGetPackByUser(Guid userId, Guid packId)
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
            Rounds = await roundStorage.TryGetByPackId(packsDb.Id),
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
        var packDb = await TryGetPackByUser(userId, pack.Id);
        if (packDb is null)
            return await SavePack(userId, pack);
        packDb.Name = pack.Name;
        packDb.UpdatedAtUtc = DateTime.UtcNow;
        await db.SaveChangesAsync(); 
        await using var tx = await db.Database.BeginTransactionAsync();
            
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
        packDb.Rounds = rounds;
        await tx.CommitAsync();
        return packDb;
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