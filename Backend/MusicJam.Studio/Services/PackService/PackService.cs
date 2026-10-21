using MusicJam.Studio.Models.Games;
using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Requests;
using MusicJam.Studio.Services.PackService;
using MusicJam.Studio.Storages;
using Shared;

namespace MusicJam.Studio.Services.GameService;

public class PackService(
    IPackStorage packStorage)
    : IPackService
{
    public async Task<Result<List<(Guid, string)>, IPackService.PackError>> GetShort(Guid userId, GetPackRequest query)
    {
        var packs = await packStorage.TryGetPacksByUser(userId);
        return packs.Select(x => (x.Id, x.Name))
            .Skip(query.Skip)
            .Take(query.Take)
            .ToList();
    }
    public async Task<Result<Pack, IPackService.PackError>> Create(Guid userId, CreatePackRequest request)
    {
        var pack = await packStorage.SavePack(userId, new Pack()
        {
            Rounds = request.Rounds,
            Name = request.Name,
        });
        return pack;
    }

    public async Task<Result<Pack, IPackService.PackError>> Update(Guid userId, Guid packId, UpdatePackRequest request)
    {
        var pack = await packStorage.Update(userId, new Pack()
        {
            Rounds = request.Rounds,
            Name = request.Name,
        });
        return pack;
    }

    public async Task<Result<bool, IPackService.PackError>> Delete(Guid userId, Guid packId)
    {
        var deleted = await packStorage.DeletePack(userId, packId);
        if (!deleted)
            return IPackService.PackError.NotFound;
        return true;
    }
}