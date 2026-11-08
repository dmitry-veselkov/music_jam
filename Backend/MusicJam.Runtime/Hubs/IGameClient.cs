using MusicJam.Runtime.Models;

namespace MusicJam.Runtime.Hubs;

public interface IGameClient
{
    Task RoomState(IReadOnlyList<Team> teams);
    Task TeamJoined(Team team);
    Task TeamLeft(Guid teamId);
    Task TeamUpdated(Team team);
}
