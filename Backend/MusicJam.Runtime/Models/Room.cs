namespace MusicJam.Runtime.Models;

public sealed class Room(Guid gameId)
{
    private readonly object lockObj = new();
    private readonly List<Team> _teams = [];

    public Guid GameId { get; } = gameId;
    public Guid PackId { get; set; }
    
    public IReadOnlyList<Team> Teams
    {
        get
        {
            lock (lockObj)
            {
                return _teams.ToArray();
            }
        }
    }
    
    public bool TryAddTeam(Team team)
    {
        lock (lockObj)
        {
            var nameTaken = _teams.Any(t =>
                string.Equals(t.TeamName, team.TeamName, StringComparison.OrdinalIgnoreCase));
            if (nameTaken)
            {
                return false;
            }

            _teams.Add(team);
            return true;
        }
    }
    
    public Team? RemoveTeam(Guid teamId)
    {
        lock (lockObj)
        {
            var team = _teams.FirstOrDefault(t => t.TeamId == teamId);
            if (team is not null)
            {
                _teams.Remove(team);
            }

            return team;
        }
    }
}
