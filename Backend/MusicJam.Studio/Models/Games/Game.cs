namespace MusicJam.Studio.Models.Games;

public class Game
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime PlannedStart { get; set; }
    public Mode Mode { get; set; }
    public State State { get; set; }
}
