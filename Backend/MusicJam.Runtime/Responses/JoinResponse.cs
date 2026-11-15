using MusicJam.Runtime.Models;

namespace MusicJam.Runtime.Responses;

public sealed class JoinResponse
{
    /// <summary>
    /// Команда игрока. У зрителя null.
    /// </summary>
    public Team? Team { get; init; }
    public required IReadOnlyList<Team> Teams { get; init; }
    public required bool IsNewTeam { get; init; }
}
