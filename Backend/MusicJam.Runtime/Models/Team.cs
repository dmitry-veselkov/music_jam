namespace MusicJam.Runtime.Models;

public enum TeamState
{
    Ready,
    Disconnected
}

public sealed class Team
{
    public Guid TeamId { get; init; }
    public string TeamName { get; set; } = string.Empty;
    public TeamState State { get; set; }
}
