using System.Collections.Concurrent;
using MusicJam.Runtime.Models;

namespace MusicJam.Runtime.Storage;

public sealed class InMemoryRoomStore : IRoomStore
{
    private readonly ConcurrentDictionary<Guid, Room> _rooms = new();
    private readonly ConcurrentDictionary<string, (Guid GameId, Guid TeamId)> _connections = new();


    public Room GetOrCreate(Guid gameId) => _rooms.GetOrAdd(gameId, id => new Room(id));

    public Room? Get(Guid gameId) => _rooms.GetValueOrDefault(gameId);

    public void BindConnection(string connectionId, Guid gameId, Guid teamId) => 
        _connections[connectionId] = (gameId, teamId);

    public (Guid GameId, Guid TeamId)? GetBinding(string connectionId) =>
        _connections.TryGetValue(connectionId, out var binding) ? binding : null;

    public (Guid GameId, Guid TeamId)? UnbindConnection(string connectionId)  =>
        _connections.TryRemove(connectionId, out var binding) ? binding : null;
}