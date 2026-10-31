using MusicJam.Studio.Models.Packs;


namespace MusicJam.Studio.Storages;

public interface IPackStorage
{
    Task<List<Pack>> GetPacksByUser(Guid userId, int? take = null, int skip = 0);
    
    Task<Pack?> GetPackByUser(Guid userId, Guid packId);

    Task<Pack> SavePack(Guid userId, Pack pack);
    
    Task<bool> DeletePack(Guid userId, Guid packId);

    
    Task<Pack> Update(Guid userId, Pack pack);
}