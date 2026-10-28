using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicJam.Studio.Constants;
using MusicJam.Studio.Models.Games;
using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Requests;
using MusicJam.Studio.Services.GameService;

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
        if (result.TryGetFault(out var error, out var games))
            return NotFound(error);
        return Ok(games);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Game), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateQuestion([FromForm] CreateGameRequests request)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();
        var result = await gameService.Create(userId, request);
        if (result.TryGetFault(out var error, out var game))
            return BadRequest(error);
        return CreatedAtAction(nameof(GetGame), new { packId = game.Id }, game);
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
        return Ok(game);
    }

    
    [HttpPatch("{gameId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAnswerTrack(Guid gameId, UpdateGameRequests requests)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result = await gameService.Update(userId, gameId, requests);
        if (result.TryGetFault(out var error, out var track))
            return NotFound(error);
        return Ok(track);
    }

    [HttpDelete("{gameId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetQuestionTrack(Guid gameId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result = await gameService.Delete(userId, gameId);
        if (result.TryGetFault(out var error, out var track))
            return NotFound(error);
        return Ok(track);
    }

    private bool TryGetUserId(out Guid userId)
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(value, out userId);
    }
}