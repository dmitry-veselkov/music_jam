using MusicJam.Runtime.Models;

namespace MusicJam.Runtime.Storages.TeamStorage;

public interface ITeamStorage
{
    Task<IReadOnlyList<Team>> GetTeams(Guid gameId);

    Task<bool> TryAdd(Guid gameId, string nameKey, Team team);

    Task<Team?> Remove(Guid gameId, Guid teamId);
}