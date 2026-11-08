using System.ComponentModel.DataAnnotations;

namespace MusicJam.Studio.Requests.Packs;

public class CreatePackRequest
{
    [Required]
    public Guid PackId { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<string> Categories { get; set; } = [];

    public List<int> Costs { get; set; } = [];

    [Required]
    public List<CreateRoundRequest> Rounds { get; set; } = [];
}
