using MusicJam.Runtime.Models;

namespace MusicJam.Runtime.Storage;

public interface IRoomStore
{
    Room GetOrCreate(Guid gameId);
    Room? Get(Guid gameId);
    void BindConnection(string connectionId, Guid gameId, Guid teamId);
    (Guid GameId, Guid TeamId)? GetBinding(string connectionId);
    (Guid GameId, Guid TeamId)? UnbindConnection(string connectionId);
}
