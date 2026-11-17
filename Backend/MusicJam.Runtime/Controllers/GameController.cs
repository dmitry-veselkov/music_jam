using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicJam.Runtime.Middlewares;
using MusicJam.Runtime.Models;
using MusicJam.Runtime.Responses;
using MusicJam.Runtime.Services.GameService;
using MusicJam.Studio.Constants;
using MusicJam.Studio.Extensions;

namespace MusicJam.Runtime.Controllers;


[ApiController]
[Route("/api/runtime")]
[Authorize(AuthenticationSchemes = AuthConstants.DefaultAuthorizationScheme)]
public class GameController(IGameService gameService) : ControllerBase
{
    [HttpPost("new-game")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateGame()
    {
        var body = await new StreamReader(Request.Body).ReadToEndAsync();
        var hostId = User.GetUserId();
        var result = await gameService.Create(hostId, Guid.Parse(body));
        if (result.Result is not null)
        {
            return result.Result;
        }

        var pack = result.Value;
        return Ok(new {gameId = pack?.GameId});
    }

    [HttpGet("user/check-role/{gameId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckRole(Guid gameId)
    {
        var result = await gameService.Get(gameId);
        var game = result.Value;
        if (game is null)
            return NotFound();
        var session = HttpContext.GetSessionId();
        var viewer = game.FindViewerBySession(session);
        var team = game.FindTeamBySession(session);
        return viewer switch
        {
            false when team is null => Ok(null),
            true => Ok(new CheckRoleResponse() { Role = Role.Viewer, TeamId = null, TeamName = null }),
            _ => Ok(new CheckRoleResponse() { Role = Role.Player, TeamId = team.TeamId, TeamName = team.TeamName })
        };
    }
    
    
    [HttpGet("user/check-host/{gameId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckHost(Guid gameId)
    {
        var result = await gameService.Get(gameId);
        var game = result.Value;
        if (game is null)
            return NotFound();
        return Ok(new { isHost = User.Identity?.IsAuthenticated == true && User.GetUserId() == game.HostId });
    }
}
