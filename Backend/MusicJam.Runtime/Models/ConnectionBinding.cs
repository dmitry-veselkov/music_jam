namespace MusicJam.Runtime.Models;

public enum Role
{
    Host,
    Player,
    Viewer
}

public sealed class Participant
{
    public Guid ParticipantId { get; set; }
    public Role Role { get; set; }
    public Guid? TeamId { get; set; }
}


public sealed record ConnectionBinding(Guid GameId, Guid ParticipantId, Role Role);
