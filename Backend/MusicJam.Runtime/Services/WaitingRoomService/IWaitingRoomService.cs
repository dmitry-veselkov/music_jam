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
        NotYourTeam,
        GameNotFound,
        GameAlreadyStarted,
        InvalidRole
    }

    Task<Result<JoinResponse, WaitingRoomError>> Join(
        string connectionId,
        Guid sessionId,
        Guid gameId,
        Role role,
        string? teamName);

    Task<Result<WaitingRoomError>> Leave(string connectionId, Guid gameId, Guid teamId);
    
    Task<RemovedTeam?> Disconnect(string connectionId);
}
