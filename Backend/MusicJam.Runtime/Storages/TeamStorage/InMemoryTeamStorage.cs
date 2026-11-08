using System.Collections.Concurrent;
using MusicJam.Runtime.Models;

namespace MusicJam.Runtime.Storages.TeamStorage;

public sealed class InMemoryTeamStorage : ITeamStorage
{
    private readonly ConcurrentDictionary<Guid, GameTeams> _games = new();

    private sealed class GameTeams
    {
        public readonly object Lock = new();
        public readonly List<(string NameKey, Team Team)> Teams = [];
    }

    public Task<IReadOnlyList<Team>> GetTeams(Guid gameId)
    {
        if (!_games.TryGetValue(gameId, out var game))
        {
            return Task.FromResult<IReadOnlyList<Team>>([]);
        }

        lock (game.Lock)
        {
            return Task.FromResult<IReadOnlyList<Team>>(game.Teams.Select(x => x.Team).ToArray());
        }
    }

    public Task<bool> TryAdd(Guid gameId, string nameKey, Team team)
    {
        var game = _games.GetOrAdd(gameId, _ => new GameTeams());
        lock (game.Lock)
        {
            if (game.Teams.Any(x => x.NameKey == nameKey))
            {
                return Task.FromResult(false);
            }

            game.Teams.Add((nameKey, team));
            return Task.FromResult(true);
        }
    }

    public Task<Team?> Remove(Guid gameId, Guid teamId)
    {
        if (!_games.TryGetValue(gameId, out var game))
        {
            return Task.FromResult<Team?>(null);
        }

        lock (game.Lock)
        {
            var index = game.Teams.FindIndex(x => x.Team.TeamId == teamId);
            if (index < 0)
            {
                return Task.FromResult<Team?>(null);
            }

            var team = game.Teams[index].Team;
            game.Teams.RemoveAt(index);
            return Task.FromResult<Team?>(team);
        }
    }
}
