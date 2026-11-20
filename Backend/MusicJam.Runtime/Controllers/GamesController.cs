using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicJam.Runtime.Requests;
using MusicJam.Runtime.Responses;
using MusicJam.Runtime.Services.GameService;
using MusicJam.Studio.Constants;
using MusicJam.Studio.Extensions;

namespace MusicJam.Runtime.Controllers;

[ApiController]
[Route("/api/runtime/games")]
[Authorize(AuthenticationSchemes = AuthConstants.DefaultAuthorizationScheme)]
public class GamesController(IGameService gameService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreateGameResponse>> CreateGame([FromBody] CreateGameRequest request)
    {
        var hostId = User.GetUserId();
        var result = await gameService.Create(hostId, request.PackId);
        if (result.TryGetFault(out var error, out var game))
        {
            return ErrorResponse(error);
        }

        var response = new CreateGameResponse
        {
            GameId = game.GameId,
            InviteCode = game.InviteInfo.Code,
            InviteLink = game.InviteInfo.Link
        };

        return response;
    }

    private ActionResult ErrorResponse(IGameService.GameError error)
    {
        return error switch
        {
            IGameService.GameError.GameNotFound => NotFound(),
            _ => throw new ArgumentOutOfRangeException(error.ToString()),
        };
    }
}