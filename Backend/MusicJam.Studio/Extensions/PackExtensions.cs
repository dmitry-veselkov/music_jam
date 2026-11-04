using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Responses;
using MusicJam.Studio.Responses.Packs;

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

    public static PackCardResponse ToCardResponse(this Pack pack)
    {
        return new PackCardResponse
        {
            PackId = pack.Id,
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
