using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Requests;
using Shared;

namespace MusicJam.Studio.Services.PackService;

public interface IPackService
{
    public enum PackError
    {
        NotFound,
        InvalidRequest
    }
    
    Task<Result<List<(Guid, string)>, PackError>> GetShort(Guid userId, GetPackRequest query);
    Task<Result<Pack, PackError>> Create(Guid userId, CreatePackRequest request);
    Task<Result<Pack, PackError>> Update(Guid userId, Guid packId, UpdatePackRequest request);
    Task<Result<bool, IPackService.PackError>> Delete(Guid userId, Guid packId);
    
}