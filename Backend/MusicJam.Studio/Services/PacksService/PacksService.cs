using MusicJam.Studio.Models.Requests;
using MusicJam.Studio.Models.Responses;
using MusicJam.Studio.Storages.PacksStorage;
using Shared;

namespace MusicJam.Studio.Services.PacksService;

internal class PacksService(IPacksStorage packsStorage) : IPacksService
{
    public async Task<Result<PackCard[], IPacksService.PacksError>> GetPackCards(
        Guid userId,
        PacksWithPaginationRequest request)
    {
        return await packsStorage.GetPackCards(userId, request.Skip, request.Take);
    }

    public async Task<Result<PackShort[], IPacksService.PacksError>> GetPacksShort(
        Guid userId,
        PacksWithPaginationRequest request)
    {
        return await packsStorage.GetPacksShort(userId, request.Skip, request.Take);
    }

    public async Task<Result<PackEdit, IPacksService.PacksError>> GetPackForEdit(Guid userId, Guid packId)
    {
        var pack = await packsStorage.GetPackEdit(userId, packId);
        if (pack is null)
        {
            return IPacksService.PacksError.PackNotFound;
        }

        return pack;
    }

    public async Task<Result<IPacksService.PacksError>> DeletePack(Guid userId, Guid packId)
    {
        var isPackInUse = await packsStorage.IsPackInUse(userId, packId);
        if (isPackInUse)
        {
            return IPacksService.PacksError.PackIsInUse;
        }

        await packsStorage.DeletePack(userId, packId);

        return Result<IPacksService.PacksError>.Success();
    }
}
