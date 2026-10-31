using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicJam.Studio.Constants;
using MusicJam.Studio.Requests;
using MusicJam.Studio.Services.GameService;
using MusicJam.Studio.Responses;
using MusicJam.Studio.Extensions;

namespace MusicJam.Studio.Controllers;

[ApiController]
[Route("api/games")]
[Authorize(AuthenticationSchemes = AuthConstants.DefaultAuthorizationScheme)]
public class GameController(IGameService gameService) : ControllerBase
{
   
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GameResponse>>> GetGames([FromQuery] GetQuestionsRequest request)
    {
        var userId = User.GetUserId();
        var result = await gameService.GetGames(userId, new GetGameRequests()
        {
            Skip = request.Skip,
            Take = request.Take
        });
        if (result.TryGetFault(out var error, out var games))
            return ErrorResponse(error);
        return games
            .Select(x => x.ToResponse())
            .ToList();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GameResponse>> Create([FromForm] CreateGameRequests request)
    {
        var userId = User.GetUserId();
        var result = await gameService.Create(userId, request);
        if (result.TryGetFault(out var error, out var game))
            return ErrorResponse(error);
        return CreatedAtAction(nameof(GetGame), new { gameId = game.Id }, game.ToResponse());
    }

    [HttpGet("{gameId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GameResponse>> GetGame(Guid gameId)
    {
        var result = await gameService.Get(gameId);
        if (result.TryGetFault(out var error, out var game))
            return ErrorResponse(error);
        return game.ToResponse();
    }

    
    [HttpPatch("{gameId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GameResponse>> Upsert(Guid gameId, [FromBody] UpdateGameRequests request)
    {
        var userId = User.GetUserId();
        var result = await gameService.Update(userId, gameId, request);
        if (result.TryGetFault(out var error, out var game))
            return ErrorResponse(error);
        return game.ToResponse();
    }

    [HttpDelete("{gameId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GameResponse>> GetQuestionTrack(Guid gameId)
    {
        var result = await gameService.Delete(gameId);
        if (result.TryGetFault(out var error, out var track))
            return ErrorResponse(error);
        return NoContent();
    }
}