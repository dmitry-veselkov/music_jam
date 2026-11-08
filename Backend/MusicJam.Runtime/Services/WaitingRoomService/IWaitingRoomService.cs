using MusicJam.Runtime.Models;
using MusicJam.Runtime.Responses;
using Shared;

namespace MusicJam.Runtime.Services.WaitingRoomService;

public interface IWaitingRoomService
{
    public const int MaxTeamNameLength = 50;

    enum WaitingRoomError
    {
        EmptyName,
        LongName,
        AlreadyInRoom,
        NameTaken,
        NotYourTeam
    }

    Task<Result<JoinResponse, WaitingRoomError>> Join(string connectionId, Guid gameId, string teamName);

    Task<Result<WaitingRoomError>> Leave(string connectionId, Guid gameId, Guid teamId);

    Task<ConnectionBinding?> Disconnect(string connectionId);
}
