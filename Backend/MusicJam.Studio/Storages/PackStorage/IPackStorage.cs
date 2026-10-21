using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Models.Users;


namespace MusicJam.Studio.Storages;

public interface IPackStorage
{
    Task<List<Pack>> TryGetPacksByUser(Guid userId);
    
    Task<Pack?> TryGetPackByUser(Guid userId, Guid packId);

    Task<Pack> SavePack(Guid userId, Pack pack);
    
    Task<bool> DeletePack(Guid userId, Guid packId);

    
    Task<Pack> Update(Guid userId, Pack pack);
}