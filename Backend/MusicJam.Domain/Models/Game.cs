namespace MusicJam.Domain.Models;

public class Game
{
    public Guid Id { get; set; }
    public Pack Pack;
    public String Description;
    public DateTime GameTime;
    public Modes Mode;
    public State State;
}