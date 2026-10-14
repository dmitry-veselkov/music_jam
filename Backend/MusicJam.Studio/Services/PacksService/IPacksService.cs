using MusicJam.Studio.Models.Requests;
using MusicJam.Studio.Models.Responses;
using Shared;

namespace MusicJam.Studio.Services.PacksService;

public interface IPacksService
{
    public enum PacksError
    {
        PackNotFound,
        PackIsInUse,
    }

    Task<Result<PackCard[], PacksError>> GetPackCards(Guid userId, PacksWithPaginationRequest request);

    Task<Result<PackShort[], PacksError>> GetPacksShort(Guid userId, PacksWithPaginationRequest request);

    Task<Result<PackEdit, PacksError>> GetPackForEdit(Guid userId, Guid packId);

    Task<Result<PacksError>> DeletePack(Guid userId, Guid packId);
}
