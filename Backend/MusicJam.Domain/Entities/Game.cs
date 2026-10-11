
namespace MusicJam.Domain.Entities;

public class Game
{
    public Guid Id { get; set; }
    public Guid PackId { get; set; }
    public Guid UserId { get; set; }
    public String Title { get; set; }
    public String Description { get; set; }
    public DateTime GameTime { get; set; }
    public Mode Mode { get; set; }
    public State State { get; set; }
    
    public Pack Pack { get; set; }
    public User User { get; set; }
}