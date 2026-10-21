using System.ComponentModel.DataAnnotations;
using MusicJam.Studio.Models.Packs;

namespace MusicJam.Studio.Requests;

public class GetPackRequest
{
    [Range(0, int.MaxValue)]
    public int Skip { get; set; }
    [Range(1, 100)]
    public int Take { get; set; } = 20;
}

public class CreatePackRequest
{
    [Required, StringLength(255, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Required] public List<Round> Rounds { get; set; } = [];
}

public class UpdatePackRequest : CreatePackRequest;


