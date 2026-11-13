using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Requests.Packs;
using MusicJam.Studio.Responses.Packs;
using Shared;

namespace MusicJam.Studio.Services.PacksService;

public interface IPacksService
{
    public enum PackError
    {
        NotFound,
        InvalidRequest,
        UnexpectedError,
        QuestionNotFound,
        DuplicateCell
    }

    Task<Result<ShortPackResponse[], PackError>> GetShortPacks(Guid userId, GetPackRequest query);
    Task<Result<PackCardResponse[], PackError>> GetPackCards(Guid userId, GetPackRequest request);
    Task<Result<EditPackResponse, PackError>> GetEditPack(Guid userId, Guid packId);
    Task<Result<Pack, PackError>> GetPack(Guid userId, Guid packId);
    Task<Result<Pack, PackError>> Create(Guid userId, CreatePackRequest request);
    Task<Result<Pack, PackError>> Update(Guid userId, Guid packId, UpdatePackRequest request);
    Task<Result<bool, PackError>> Delete(Guid userId, Guid packId);
}
