using MusicJam.Studio.Infrustructure.Data.Configurations;
using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Models.Users;

using DbPack = MusicJam.Studio.Infrustructure.Entities.Pack;
using Pack = MusicJam.Studio.Models.Packs.Pack;

namespace MusicJam.Studio.Storages;

public class PackStorage(AppDbContext db, RoundStorage roundStorage) : IPackStorage
{
    public async Task<List<Pack>> TryGetPacksByUser(User user)
    {
        var packs = db.Packs
            .Where(x => x.UserId == user.Id)
            .OrderBy(x => x.CreatedAtUtc)
            .ToList();
 
        var rounds = await roundStorage.TryGetByPackId(packs.Select(x => x.Id).ToList());
 
        return packs
            .Select(p => ToModel(p, rounds.TryGetValue(p.Id, out var r) ? r : new List<Round>()))
            .ToList();
    }

    public Task<Pack> SavePack(Pack user)
    {
        throw new NotImplementedException();
    }

    public Task<Pack> CreatePack(Pack user)
    {
        throw new NotImplementedException();
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