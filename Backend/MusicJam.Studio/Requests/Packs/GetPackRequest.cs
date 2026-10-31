using System.ComponentModel.DataAnnotations;

namespace MusicJam.Studio.Requests.Packs;

public class GetPackRequest
{
    [Range(0, int.MaxValue)]
    public int Skip { get; set; }

    [Range(1, 100)]
    public int Take { get; set; } = 20;
}
