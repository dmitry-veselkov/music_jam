using MusicJam.Studio.Models.Games;
using MusicJam.Studio.Responses;

namespace MusicJam.Studio.Extensions;

public static class GameExtensions
{
    public static GameResponse ToResponse(this Game game)
    {
        return new GameResponse
        {
            GameId = game.Id,
            Name = game.Name,
            Description = game.Description,
            Date = game.PlannedStart,
            Mode = game.Mode,
            State = game.State,
            Pack = game.Pack.ToShortResponse()
        };
    }
}
