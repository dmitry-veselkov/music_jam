using System.ComponentModel.DataAnnotations;

namespace MusicJam.Studio.Requests;

public class GetPackResponse
{
    [Required]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}