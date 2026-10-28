using System.ComponentModel.DataAnnotations;
using MusicJam.Studio.Models.Games;

namespace MusicJam.Studio.Requests;

public class GetGameRequests
{
    [Range(0, int.MaxValue)]
    public int Skip { get; set; }
    [Range(1, 100)]
    public int Take { get; set; } = 20;
}

public class CreateGameRequests
{
    [Required] public Guid PackId { get; set; }
    [Required] public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? Date { get; set; }
    [Required] public Mode Mode { get; set; }
}

public class UpdateGameRequests : CreateGameRequests;
