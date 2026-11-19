using System.Collections.Concurrent;
using MusicJam.Runtime.Models;

namespace MusicJam.Runtime.Storages.GameStorage;

public sealed class GameStorage : IGameStorage
{
    private readonly ConcurrentDictionary<Guid, Game> _games = new();

    public Task<bool> Add(Game game) =>
        Task.FromResult(_games.TryAdd(game.GameId, game));

    public Task<Game?> Get(Guid gameId) =>
        Task.FromResult(_games.GetValueOrDefault(gameId));
}
