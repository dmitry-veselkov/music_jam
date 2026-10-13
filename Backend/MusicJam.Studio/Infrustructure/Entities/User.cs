
using MusicJam.Domain.Common;

namespace MusicJam.Domain.Entities;

public class User : Entity
{ 
    public String Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    
    public List<Game> Games { get; set; }
    public List<Pack> Packs { get; set; }
    public List<Question> Questions { get; set; }
}