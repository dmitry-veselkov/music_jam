using MusicJam.Studio.Models.Games;

namespace MusicJam.Studio.Responses;

// Формат GameDto на фронте
public class GameResponse
{
    public Guid GameId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public Mode Mode { get; set; }
    public State State { get; set; }
    public ShortPackResponse Pack { get; set; }

    public static GameResponse From(Game game) => new()
    {
        GameId = game.Id,
        Name = game.Name,
        Description = game.Description,
        Date = game.PlannedStart,
        Mode = game.Mode,
        State = game.State,
        Pack = new ShortPackResponse { PackId = game.Pack.Id, Name = game.Pack.Name }
    };
}
