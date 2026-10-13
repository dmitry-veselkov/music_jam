namespace MusicJam.Studio.Entities;

public class User(string email, string passwordHash, string? name)
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; } = name;
    public string Email { get; set; } = email;
    public string PasswordHash { get; set; } = passwordHash;
}
