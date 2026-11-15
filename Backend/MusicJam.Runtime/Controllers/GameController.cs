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
        if (User.Identity?.IsAuthenticated == true && User.GetUserId() == game.HostId)
            return Ok(new CheckRoleResponse()
            {
                Role = Role.Host,
                TeamId = null
            });

        var sessionId = HttpContext.GetSessionId();
        var team = game.FindTeamBySession(sessionId); 
        return Ok(team is null
            ? new CheckRoleResponse()
            {
                Role = Role.Viewer,
                TeamId = null
            }
            : new CheckRoleResponse()
            {
                Role = Role.Player,
                TeamId = team.TeamId
            });
    }
    
}
