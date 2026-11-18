using MusicJam.Studio.Models.Packs;

namespace MusicJam.Studio.Models.Users;

public class User(string email, string password, string username)
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = username;
    public string Email { get; set; } = email;
    public string Password { get; set; } = password;
    public List<Pack> Packs { get; set; } = [];
    public List<Question> Questions { get; set; } = [];
}
