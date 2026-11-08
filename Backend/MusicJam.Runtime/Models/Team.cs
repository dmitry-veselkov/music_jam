namespace MusicJam.Runtime.Models;

public enum TeamState
{
    Ready, 
    Disconnected
}

public sealed record Team(Guid TeamId, string TeamName, TeamState State);