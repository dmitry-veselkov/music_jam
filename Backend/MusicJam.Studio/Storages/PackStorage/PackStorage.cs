using MusicJam.Studio.Infrustructure.Data.Configurations;
using MusicJam.Studio.Models.Users;
using MusicJam.Studio.Storages.PackStorage;

using DbPack = MusicJam.Studio.Infrustructure.Entities.Pack;
using Pack = MusicJam.Studio.Models.Packs.Pack;

namespace MusicJam.Studio.Controllers;

public class PackStorage(AppDbContext db) : IPackStorage
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
    
    private static Pack ToModel(DbPack e) => new()
    {
        Id = e.Id, 
        Name = e.Title, 
        Rounds = e.Rounds, 
        CreatedAtUtc = e.CreatedAtUtc, 
        UpdatedAtUtc = e.UpdatedAtUtc
    };
}