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
    [Required]
    public Guid packId;
    [Required]

    public string Name;
    public string? Description;
    public DateTime? date;
    
    [Required]
    public Mode mode;
}

public class UpdateGameRequests
{
    [Required]
    public Guid packId;
    [Required]

    public string Name;
    public string? Description;
    public DateTime? date;
    
    [Required]
    public Mode mode;
}