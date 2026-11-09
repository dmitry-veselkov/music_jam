using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        var pack = result.Value;
        return Ok(new {gameId = pack?.GameId});
    }
}
