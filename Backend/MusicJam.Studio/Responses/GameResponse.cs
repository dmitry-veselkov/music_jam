using MusicJam.Studio.Models.Games;
using MusicJam.Studio.Responses.Packs;

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
}
