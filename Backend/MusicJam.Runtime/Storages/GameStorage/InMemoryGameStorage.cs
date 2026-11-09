using System.Collections.Concurrent;
using MusicJam.Runtime.Models;

namespace MusicJam.Runtime.Storages.GameStorage;

public sealed class InMemoryGameStorage : IGameStorage
{
    private readonly ConcurrentDictionary<Guid, Game> _games = new();

    public Task<Game> GetOrCreate(Guid gameId, Guid packId)
    {
        return Task.FromResult(
            _games.GetOrAdd(gameId, id => new Game { GameId = id, PackId = packId }));
    }
        

    public Task<Game?> Get(Guid gameId) =>
        Task.FromResult(_games.GetValueOrDefault(gameId));
}
