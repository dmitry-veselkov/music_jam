using Microsoft.AspNetCore.SignalR;
using MusicJam.Runtime.Middlewares;
using MusicJam.Runtime.Models;
using MusicJam.Runtime.Services.WaitingRoomService;
using WaitingRoomError = MusicJam.Runtime.Services.WaitingRoomService.IWaitingRoomService.WaitingRoomError;

namespace MusicJam.Runtime.Hubs;

public sealed partial class GameHub
{
    public async Task<Team?> JoinGame(Guid gameId, string? teamName, Role state)
    {
        var sessionId = GetSessionId();
        var result = await waitingRoomService.Join(Context.ConnectionId, sessionId, gameId, state, teamName);
        if (result.TryGetFault(out var error, out var joined))
        {
            throw ToHubException(error);
        }

        var group = GroupName(gameId);
        await Groups.AddToGroupAsync(Context.ConnectionId, group);
        await Clients.Caller.RoomState(joined.Teams);
        if (joined is { IsNewTeam: true, Team: { } team })
        {
            await Clients.OthersInGroup(group).TeamJoined(team);
        }

        return joined.Team;
    }
    
    public async Task LeaveGame(Guid gameId, Guid teamId)
    {
        var result = await waitingRoomService.Leave(Context.ConnectionId, gameId, teamId);
        if (result.TryGetFault(out var error))
        {
            throw ToHubException(error);
        }

        var group = GroupName(gameId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
        await Clients.Group(group).TeamLeft(teamId);
    }
    
    private Guid GetSessionId() =>
        Context.GetHttpContext()?.GetSessionId()
        ?? throw new HubException("Не удалось определить сессию");

    private static HubException ToHubException(WaitingRoomError error) => error switch
    {
        WaitingRoomError.EmptyName => new("Название команды не может быть пустым"),
        WaitingRoomError.LongName => new($"Название команды должно быть не длиннее {IWaitingRoomService.MaxTeamNameLength} символов"),
        WaitingRoomError.AlreadyInRoom => new("Вы уже в комнате"),
        WaitingRoomError.NameTaken => new("Команда с таким названием уже есть"),
        WaitingRoomError.NotYourTeam => new("Эта команда вам не принадлежит"),
        WaitingRoomError.GameNotFound => new("Игра не найдена"),
        WaitingRoomError.GameAlreadyStarted => new("Игра уже началась"),
        WaitingRoomError.InvalidRole => new("Войти можно только как player или viewer"),
        _ => throw new ArgumentOutOfRangeException(nameof(error), error, null),
    };
}
