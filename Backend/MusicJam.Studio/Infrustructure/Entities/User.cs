namespace MusicJam.Studio.Infrustructure.Entities;

public class User : Entity
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public List<Game> Games { get; set; }
    public List<Pack> Packs { get; set; }
    public List<Question> Questions { get; set; }
}
