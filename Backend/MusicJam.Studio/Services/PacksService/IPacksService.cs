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
        EmptyRequest,
        EmptyPackName,
        EmptyRounds,
        InvalidRound,
        EmptyRoundName,
        EmptyQuestions,
        InvalidTableFormat
    }

    Task<Result<PackCard[], PacksError>> GetPackCards(Guid userId, PacksWithPaginationRequest request);

    Task<Result<PackShort[], PacksError>> GetPacksShort(Guid userId, PacksWithPaginationRequest request);

    Task<Result<PackForTable, PacksError>> GetPackForTable(Guid userId, Guid packId);

    Task<Result<PacksError>> DeletePack(Guid userId, Guid packId);

    Task<Result<PacksError>> UpsertPack(Guid userId, Guid packId, LoadPackRequest request);
}
