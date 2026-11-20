using MusicJam.Runtime.Models;

namespace MusicJam.Runtime.Services.InviteService;

public interface IInviteService
{
    public GameInviteInfo CreateInviteInfo(Guid gameId);
}