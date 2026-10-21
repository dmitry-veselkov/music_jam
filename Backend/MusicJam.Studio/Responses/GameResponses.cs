using MusicJam.Studio.Models.Games;


namespace MusicJam.Studio.Responses;

public class GameResponses
{
    
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public Mode Mode { get; set; }
    public State State { get; set; }
}