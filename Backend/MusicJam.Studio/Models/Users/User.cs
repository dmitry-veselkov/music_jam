using MusicJam.Studio.Models.Games;
using MusicJam.Studio.Models.Packs;

namespace MusicJam.Studio.Models.Users;

public class User(string email, string passwordHash, string name)
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = name;
    public string Email { get; set; } = email;
    public string PasswordHash { get; set; } = passwordHash;
    public List<Pack> Packs { get; set; } = [];
    public List<Game> Games { get; set; } = [];
    public List<Question> Questions { get; set; } = [];
    public Statistic Statistic { get; set; } = new();
}
