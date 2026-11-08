using MusicJam.Runtime.Models;
using MusicJam.Runtime.Storage;
using Shared;
using WaitingRoomError = MusicJam.Runtime.Services.WaitingRoomService.IWaitingRoomService.WaitingRoomError;

namespace MusicJam.Runtime.Services.WaitingRoomService;

public sealed class WaitingRoomService(IRoomStore rooms) : IWaitingRoomService
{
    public Result<(Team Team, IReadOnlyList<Team> Teams), WaitingRoomError> Join(
        string connectionId,
        Guid gameId,
        string teamName)
    {
        var name = teamName?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            return WaitingRoomError.EmptyName;
        }

        if (name.Length > IWaitingRoomService.MaxTeamNameLength)
        {
            return WaitingRoomError.LongName;
        }

        if (rooms.GetBinding(connectionId) is not null)
        {
            return WaitingRoomError.AlreadyInRoom;
        }

        var room = rooms.GetOrCreate(gameId);

        var team = new Team(Guid.NewGuid(), name, TeamState.Ready);
        if (!room.TryAddTeam(team))
        {
            return WaitingRoomError.NameTaken;
        }

        rooms.BindConnection(connectionId, gameId, team.TeamId);

        return (team, room.Teams);
    }

    public Result<WaitingRoomError> Leave(string connectionId, Guid gameId, Guid teamId)
    {
        var binding = rooms.GetBinding(connectionId);
        if (binding is not { } b || b.GameId != gameId || b.TeamId != teamId)
        {
            return WaitingRoomError.NotYourTeam;
        }

        rooms.UnbindConnection(connectionId);
        rooms.Get(gameId)?.RemoveTeam(teamId);

        return Result<WaitingRoomError>.Success();
    }

    public (Guid GameId, Guid TeamId)? Disconnect(string connectionId)
    {
        if (rooms.UnbindConnection(connectionId) is not { } binding)
        {
            return null;
        }

        var removed = rooms.Get(binding.GameId)?.RemoveTeam(binding.TeamId);

        return removed is null ? null : binding;
    }
}
