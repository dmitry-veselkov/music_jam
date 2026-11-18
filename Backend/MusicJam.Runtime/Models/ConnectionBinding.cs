namespace MusicJam.Runtime.Models;

public enum Role
{
    Host,
    Player,
    Viewer
}

public sealed record ConnectionBinding(Guid GameId, Guid ParticipantId, Role Role);
