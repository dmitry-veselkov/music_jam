using System.Collections.Concurrent;
using MusicJam.Runtime.Models;

namespace MusicJam.Runtime.Storages.InvitesStorage;

public class InMemoryInvitesStorage : IInvitesStorage
{
    private readonly ConcurrentDictionary<Guid, GameInvite> _invites = new();

    public async Task<GameInvite> SaveInvite(GameInvite invite)
    {
        await Task.CompletedTask;
        _invites.TryAdd(invite.InviteId, invite);
        return invite;
    }

    public async Task<GameInvite?> GetInviteById(Guid inviteId)
    {
        await Task.CompletedTask;
        return _invites.TryGetValue(inviteId, out var invite) ? invite : null;
    }

    public async Task<GameInvite?> GetInviteByCode(string code)
    {
        await Task.CompletedTask;
        foreach (var (_, invite) in _invites)
        {
            if (invite.Code == code)
            {
                return invite;
            }
        }

        return null;
    }
}
