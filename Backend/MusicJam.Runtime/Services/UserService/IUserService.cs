using MusicJam.Runtime.Responses;
using Shared;

namespace MusicJam.Runtime.Services.UserService;

public interface IUserService
{
    public enum UserError
    {
        GameNotFound,
    }
    
    Task<Result<CheckRoleResponse?, UserError>> GetRole(Guid gameId, Guid sessionId);
    
    Task<Result<bool, UserError>> IsHost(Guid gameId, Guid? userId);
}
