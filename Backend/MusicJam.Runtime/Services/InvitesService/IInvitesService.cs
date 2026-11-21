using MusicJam.Runtime.Models;
using Shared;

namespace MusicJam.Runtime.Services.InvitesService;

public interface IInvitesService
{
    public enum InviteError
    {
        InviteNotFound,
        InvalidCode,
        InvalidLink,
    }

    public Task<GameInvite> CreateInvite(Guid gameId);

    public Task<Result<string, InviteError>> JoinGameByCode(string code);

    public Task<Result<string, InviteError>> JoinGameByLink(Guid inviteId);
}