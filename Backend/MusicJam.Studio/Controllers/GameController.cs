using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicJam.Studio.Constants;
using MusicJam.Studio.Models.Games;
using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Requests;
using MusicJam.Studio.Services.GameService;
using MusicJam.Studio.Responses;

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
    public async Task<IActionResult> GetGames([FromQuery] GetQuestionsQuery query)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();
        var result = await gameService.GetGames(userId, new GetGameRequests()
        {
            Skip = query.Skip,
            Take = query.Take
        });
        Console.WriteLine(result.IsSuccess);
        if (result.TryGetFault(out var error, out var games))
            return NotFound(error);
        return Ok(games.Select(GameResponse.From));
    }

    [HttpPost]
    [ProducesResponseType(typeof(Game), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromForm] CreateGameRequests request)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();
        var result = await gameService.Create(userId, request);
        if (result.TryGetFault(out var error, out var game))
            return BadRequest(error);
        return CreatedAtAction(nameof(GetGame), new { gameId = game.Id }, GameResponse.From(game));
    }

    [HttpGet("{gameId:guid}")]
    [ProducesResponseType(typeof(Question), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetGame(Guid gameId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();
        var result = await gameService.Get(gameId);
        if (result.TryGetFault(out var error, out var game))
            return NotFound(error);
        return Ok(GameResponse.From(game));
    }

    
    [HttpPatch("{gameId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Upsert(Guid gameId, [FromBody] UpdateGameRequests request)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result = await gameService.Update(userId, gameId, request);
        if (result.TryGetFault(out var error, out var game))
            return BadRequest(error);
        return Ok(GameResponse.From(game));
    }

    [HttpDelete("{gameId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetQuestionTrack(Guid gameId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result = await gameService.Delete(gameId);
        if (result.TryGetFault(out var error, out var track))
            return NotFound(error);
        return NoContent();
    }

    private bool TryGetUserId(out Guid userId)
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(value, out userId);
    }
}