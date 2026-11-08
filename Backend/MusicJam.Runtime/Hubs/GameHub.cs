using Microsoft.AspNetCore.SignalR;
using MusicJam.Runtime.Services.WaitingRoomService;

namespace MusicJam.Runtime.Hubs;

/// <summary>
/// Единый хаб игры. Здесь только транспорт: группы, рассылка событий, ошибки для клиента.
/// Правила живут в сервисах. Команды разных этапов разложены по partial-файлам:
/// WaitingRoom.cs - зал ожидания.
/// </summary>
public sealed partial class GameHub(IWaitingRoomService waitingRoom) : Hub<IGameClient>
{
    private static string GroupName(Guid gameId) => $"game:{gameId}";

    /// <summary>
    /// Вкладку закрыли или пропал интернет: фронт LeaveGame не пришлёт,
    /// поэтому убираем команду сами.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (waitingRoom.Disconnect(Context.ConnectionId) is { } removed)
        {
            await Clients.Group(GroupName(removed.GameId)).TeamLeft(removed.TeamId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
