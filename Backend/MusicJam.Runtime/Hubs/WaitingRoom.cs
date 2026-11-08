using Microsoft.AspNetCore.SignalR;
using MusicJam.Runtime.Middlewares;
using MusicJam.Runtime.Models;
using MusicJam.Runtime.Responses;
using MusicJam.Runtime.Services.WaitingRoomService;
using MusicJam.Studio.Extensions;
using WaitingRoomError = MusicJam.Runtime.Services.WaitingRoomService.IWaitingRoomService.WaitingRoomError;

namespace MusicJam.Runtime.Hubs;

public sealed partial class GameHub
{
    public async Task<JoinResponse> JoinGame(Guid gameId, Role role, string? teamName)
    {
        var sessionId = GetSessionId();
        var userId = Context.User?.Identity?.IsAuthenticated == true ? Context.User.GetUserId() : (Guid?)null;
        var result = await waitingRoomService.Join(Context.ConnectionId, sessionId, userId, gameId, role, teamName);
        if (result.TryGetFault(out var error, out var joined))
        {
            throw ToHubException(error);
        }

        var game = (await gameService.Get(gameId)).Value
                   ?? throw ToHubException(WaitingRoomError.GameNotFound);

        List<Team> teams;
        Team? team = null;
        lock (game.Lock)
        {
            teams = game.Teams.Values.ToList();
            if (joined.TeamId is { } teamId)
            {
                game.Teams.TryGetValue(teamId, out team);
            }
        }

        var group = GroupName(gameId);
        await Groups.AddToGroupAsync(Context.ConnectionId, group);
        await Clients.Caller.RoomState(teams);
        if (team is not null)
        {
            await Clients.OthersInGroup(group).TeamJoined(team);
        }

        return joined;
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
        Context.GetHttpContext()?.TryGetSessionId()
        ?? throw new HubException("Нет сессии, обновите страницу");

    private static HubException ToHubException(WaitingRoomError error) => error switch
    {
        WaitingRoomError.EmptyName => new("Название команды не может быть пустым"),
        WaitingRoomError.LongName => new($"Название команды должно быть не длиннее {IWaitingRoomService.MaxTeamNameLength} символов"),
        WaitingRoomError.AlreadyInRoom => new("Вы уже в комнате"),
        WaitingRoomError.NameTaken => new("Команда с таким названием уже есть"),
        WaitingRoomError.NotYourTeam => new("Эта команда вам не принадлежит"),
        WaitingRoomError.GameNotFound => new("Игра не найдена"),
        WaitingRoomError.GameAlreadyStarted => new("Игра уже началась"),
        WaitingRoomError.InvalidRole => new("Неизвестная роль"),
        WaitingRoomError.NotHost => new("Вы не являетесь хостом этой игры"),
        _ => throw new ArgumentOutOfRangeException(nameof(error), error, null),
    };
}
