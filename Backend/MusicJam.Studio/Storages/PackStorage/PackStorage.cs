using MusicJam.Studio.Infrustructure.Data.Configurations;
using MusicJam.Studio.Models.Users;

using DbPack = MusicJam.Studio.Infrustructure.Entities.Pack;
using Pack = MusicJam.Studio.Models.Packs.Pack;

namespace MusicJam.Studio.Storages;

public class PackStorage(AppDbContext db, RoundStorage roundStorage) : IPackStorage
{
    public async Task<List<Pack>> TryGetPacksByUser(User user)
    {
        return db.Packs
            .Where(x => x.UserId == user.Id)
            .Select(x => ToModel(x))
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
    
    public static Pack ToModel(DbPack e) => new()
    {
        Id = e.Id, 
        Name = e.Title, 
        Rounds = roundStorage.ToModel(e.Rounds), 
        CreatedAtUtc = e.CreatedAtUtc, 
        UpdatedAtUtc = e.UpdatedAtUtc
    };
    
    public static DbPack ToDbModel(Pack e) => new()
    {
        Title = e.Name, 
        Rounds = e.Rounds, 
        CreatedAtUtc = e.CreatedAtUtc, 
        UpdatedAtUtc = e.UpdatedAtUtc
    };
}