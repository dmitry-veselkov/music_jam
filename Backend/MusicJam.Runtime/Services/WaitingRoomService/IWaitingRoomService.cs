using MusicJam.Runtime.Models;
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
    
    Result<(Team Team, IReadOnlyList<Team> Teams), WaitingRoomError> Join(string connectionId, Guid gameId, string teamName);
    
    Result<WaitingRoomError> Leave(string connectionId, Guid gameId, Guid teamId);
    
    (Guid GameId, Guid TeamId)? Disconnect(string connectionId);
}
