using MusicJam.Runtime.Models;

namespace MusicJam.Runtime.Responses;

public sealed class JoinResponse
{
    public Role Role { get; set; }
    public Guid? TeamId { get; set; }
    public string? TeamName { get; set; }
}
