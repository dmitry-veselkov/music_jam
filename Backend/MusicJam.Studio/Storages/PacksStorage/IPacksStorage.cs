using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Models.Responses;

namespace MusicJam.Studio.Storages.PacksStorage;

public interface IPacksStorage
{
    Task<PackCard[]> GetPackCards(Guid userId, int skip, int take);

    Task<PackShort[]> GetPacksShort(Guid userId, int skip, int take);

    Task<PackEdit?> GetPackEdit(Guid userId, Guid packId);

    Task<bool> IsPackInUse(Guid userId, Guid packId);

    Task<Pack?> GetPackById(Guid userId, Guid packId);

    Task CreatePack(Guid userId, Pack pack);

    Task UpdatePack(Guid userId, Pack pack);

    Task DeletePack(Guid userId, Guid packId);
}
