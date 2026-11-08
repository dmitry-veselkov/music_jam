using System.Collections.Concurrent;
using MusicJam.Runtime.Models;

namespace MusicJam.Runtime.Storages.ConnectionStorage;

public sealed class InMemoryConnectionStorage : IConnectionStorage
{
    private readonly ConcurrentDictionary<string, ConnectionBinding> _connections = new();

    public Task Bind(string connectionId, ConnectionBinding binding)
    {
        _connections[connectionId] = binding;
        return Task.CompletedTask;
    }

    public Task<ConnectionBinding?> Get(string connectionId) =>
        Task.FromResult(_connections.GetValueOrDefault(connectionId));

    public Task<ConnectionBinding?> Unbind(string connectionId) =>
        Task.FromResult(_connections.TryRemove(connectionId, out var binding) ? binding : null);
}
