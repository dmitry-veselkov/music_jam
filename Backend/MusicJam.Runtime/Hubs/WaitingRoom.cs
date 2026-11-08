using Microsoft.AspNetCore.SignalR;
using MusicJam.Runtime.Models;
using MusicJam.Runtime.Services.WaitingRoomService;
using WaitingRoomError = MusicJam.Runtime.Services.WaitingRoomService.IWaitingRoomService.WaitingRoomError;

namespace MusicJam.Runtime.Hubs;

public sealed partial class GameHub
{
    public async Task<Team> JoinGame(Guid gameId, string teamName)
    {
        var result = waitingRoom.Join(Context.ConnectionId, gameId, teamName);
        if (result.TryGetFault(out var error, out var joined))
        {
            throw ToHubException(error);
        }

        var group = GroupName(gameId);
        await Groups.AddToGroupAsync(Context.ConnectionId, group);
        await Clients.Caller.RoomState(joined.Teams);
        await Clients.OthersInGroup(group).TeamJoined(joined.Team);

        return joined.Team;
    }
    
    public async Task LeaveGame(Guid gameId, Guid teamId)
    {
        var result = waitingRoom.Leave(Context.ConnectionId, gameId, teamId);
        if (result.TryGetFault(out var error))
        {
            throw ToHubException(error);
        }

        var group = GroupName(gameId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
        await Clients.Group(group).TeamLeft(teamId);
    }
    
    private static HubException ToHubException(WaitingRoomError error) => error switch
    {
        WaitingRoomError.EmptyName => new("Название команды не может быть пустым"),
        WaitingRoomError.LongName => new($"Название команды должно быть не длиннее {IWaitingRoomService.MaxTeamNameLength} символов"),
        WaitingRoomError.AlreadyInRoom => new("Вы уже в комнате"),
        WaitingRoomError.NameTaken => new("Команда с таким названием уже есть"),
        WaitingRoomError.NotYourTeam => new("Эта команда вам не принадлежит"),
        _ => throw new ArgumentOutOfRangeException(nameof(error), error, null),
    };
}
