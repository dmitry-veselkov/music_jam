using System.ComponentModel.DataAnnotations;
using MusicJam.Studio.Models.Packs;

namespace MusicJam.Studio.Responses;

public class GetPackResponse
{
    [Required]
    public Guid PackId { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}

public class EditPackResponse
{
    [Required]
    public Guid PackId { get; set; }
    public string Name { get; set; }
    public List<RoundResponse> Rounds { get; set; }
}