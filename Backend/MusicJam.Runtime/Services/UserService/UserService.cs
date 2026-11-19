using MusicJam.Runtime.Models;
using MusicJam.Runtime.Responses;
using MusicJam.Runtime.Storages.GameStorage;
using Shared;

namespace MusicJam.Runtime.Services.UserService;

public class UserService(IGameStorage gameStorage) : IUserService
{
    public async Task<Result<CheckRoleResponse?, IUserService.UserError>> GetRole(Guid gameId, Guid sessionId)
    {
        var game = await gameStorage.Get(gameId);
        if (game is null)
        {
            return IUserService.UserError.GameNotFound;
        }

        if (game.FindViewerBySession(sessionId))
        {
            return new CheckRoleResponse { Role = Role.Viewer, TeamId = null, TeamName = null };
        }

        var team = game.FindTeamBySession(sessionId);
        return team is not null 
            ? new CheckRoleResponse { Role = Role.Player, TeamId = team.TeamId, TeamName = team.TeamName } 
            : Result<CheckRoleResponse?, IUserService.UserError>.Success(null);
    }

    public async Task<Result<bool, IUserService.UserError>> IsHost(Guid gameId, Guid? userId)
    {
        var game = await gameStorage.Get(gameId);
        if (game is null)
        {
            return IUserService.UserError.GameNotFound;
        }
        return userId is not null && userId == game.HostId;
    }
}
