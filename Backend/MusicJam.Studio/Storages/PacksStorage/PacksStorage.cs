using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Models.Responses;

namespace MusicJam.Studio.Storages.PacksStorage;

internal class PacksStorage : IPacksStorage
{
    public async Task<PackCard[]> GetPackCards(Guid userId, int skip, int take)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }

    public async Task<PackShort[]> GetPacksShort(Guid userId, int skip, int take)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }

    public async Task<PackEdit?> GetPackEdit(Guid userId, Guid packId)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }

    // Пока сделал так для простоты. По идее, нужно получать инфу о том, где именно используется пак
    public async Task<bool> IsPackInUse(Guid userId, Guid packId)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }

    public async Task<Pack?> GetPackById(Guid userId, Guid packId)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }

    public async Task CreatePack(Guid userId, Pack pack)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }

    public async Task UpdatePack(Guid userId, Pack pack)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }

    public async Task DeletePack(Guid userId, Guid packId)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
}
