using MusicJam.Studio.Models.Packs;

namespace MusicJam.Runtime.Models;

public enum GameState
{
    Pending,
    InProgress,
    Finished
}

public sealed class Game
{
    public Guid GameId { get; init; }
    public Guid HostId { get; set; }
    
    public Pack Pack { get; set; }
    public GameState State { get; set; } = GameState.Pending;

    public object Lock { get; } = new();

    public Dictionary<Guid, Team> Teams { get; } = new();

    public List<Guid> Viewers { get; } = new();

    public Team? FindTeamBySession(Guid sessionId)
    {
        lock (Lock)
        {
            return Teams
                .Values
                .FirstOrDefault(team => team.SessionId == sessionId);
        }
    }
    
    public bool FindViewerBySession(Guid sessionId)
    {
        lock (Lock)
        {
            return Viewers.Contains(sessionId);
        }
    }
}

