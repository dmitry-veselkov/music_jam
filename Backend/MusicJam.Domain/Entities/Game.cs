
using MusicJam.Domain.Common;

namespace MusicJam.Domain.Entities;

public class Game : Entity
{
    public Guid PackId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime GameTime { get; set; }
    public Mode Mode { get; set; }
    public State State { get; set; }
    
    public Pack Pack { get; set; }
    public User User { get; set; }
}