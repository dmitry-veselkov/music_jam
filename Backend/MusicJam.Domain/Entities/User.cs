
namespace MusicJam.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public String Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public List<Game> Games { get; set; }
    public List<Round> Rounds { get; set; }
    public List<Question> Questions { get; set; }
}