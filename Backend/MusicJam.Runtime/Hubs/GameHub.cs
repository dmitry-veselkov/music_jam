using Microsoft.AspNetCore.SignalR;
using MusicJam.Runtime.Services.WaitingRoomService;

namespace MusicJam.Runtime.Hubs;

public sealed partial class GameHub(IWaitingRoomService waitingRoomService) : Hub<IGameClient>
{
    private static string GroupName(Guid gameId) => $"game:{gameId}";
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (await waitingRoomService.Disconnect(Context.ConnectionId) is { } removed)
        {
            await Clients.Group(GroupName(removed.GameId)).TeamLeft(removed.TeamId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
