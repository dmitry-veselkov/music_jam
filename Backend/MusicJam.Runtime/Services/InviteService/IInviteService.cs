using MusicJam.Runtime.Models;
using Shared;

namespace MusicJam.Runtime.Services.InviteService;

public interface IInviteService
{
    public enum InviteError
    {
        InviteNotFound,
        InvalidCodeFormat
    }

    public GameInviteInfo CreateInviteInfo(Guid gameId);

    public Task<Result<string, InviteError>> JoinGameByCode(string code);
}