using MusicJam.Runtime.Models;

namespace MusicJam.Runtime.Responses;

public sealed class JoinResponse
{
    public required Team Team { get; init; }
    public required IReadOnlyList<Team> Teams { get; init; }
}
