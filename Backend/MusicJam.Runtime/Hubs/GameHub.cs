using Microsoft.AspNetCore.SignalR;
using MusicJam.Runtime.Services.GameService;
using MusicJam.Runtime.Services.WaitingRoomService;

namespace MusicJam.Runtime.Hubs;

public sealed partial class GameHub(
    IWaitingRoomService waitingRoomService,
    IGameService gameService) : Hub<IGameClient>
{
    public const string Path = "/api/runtime/hub";

    private static string GroupName(Guid gameId) => $"game:{gameId}";
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (await waitingRoomService.Disconnect(Context.ConnectionId) is { } disconnected)
        {
            await Clients.Group(GroupName(disconnected.GameId)).TeamUpdated(disconnected.Team);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
