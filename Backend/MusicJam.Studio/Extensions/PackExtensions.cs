using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Responses;

namespace MusicJam.Studio.Extensions;

public static class PackExtensions
{
    public static ShortPackResponse ToShortResponse(this Pack pack)
    {
        return new ShortPackResponse
        {
            PackId = pack.Id,
            Name = pack.Name
        };
    }

    public static GetPackResponse ToCardResponse(this Pack pack)
    {
        return new GetPackResponse
        {
            PackId = pack.Id.ToString(),
            Name = pack.Name,
            CreatedAtUtc = pack.CreatedAtUtc.ToString(),
            UpdatedAtUtc = pack.UpdatedAtUtc.ToString()
        };
    }

    public static EditPackResponse ToEditResponse(this Pack pack)
    {
        return new EditPackResponse
        {
            PackId = pack.Id,
            Name = pack.Name,
            Rounds = pack.Rounds.Select(x => x.ToResponse()).ToList()
        };
    }
}
