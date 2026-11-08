using MusicJam.Runtime.Models;

namespace MusicJam.Runtime.Storages.ConnectionStorage;

public interface IConnectionStorage
{
    Task Bind(string connectionId, ConnectionBinding binding);

    Task<ConnectionBinding?> Get(string connectionId);

    Task<ConnectionBinding?> Unbind(string connectionId);
}
