using MusicJam.Runtime.Models;

namespace MusicJam.Runtime.Storages.InvitesStorage;

public interface IInvitesStorage
{
    public Task<GameInvite> SaveInvite(GameInvite invite);

    public Task<GameInvite?> GetInviteById(Guid inviteId);

    public Task<GameInvite?> GetInviteByCode(string code);
}