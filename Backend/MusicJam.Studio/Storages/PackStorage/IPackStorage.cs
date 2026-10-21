using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Models.Users;


namespace MusicJam.Studio.Storages;

public interface IPackStorage
{
    Task<List<Pack>> TryGetPacksByUser(User user);

    Task<Pack> SavePack(Pack user);
    
    Task<Pack> CreatePack(Pack user);
    Task<Pack> DeletePack(Pack user);
}