using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicJam.Runtime.Services.InviteService;

namespace MusicJam.Runtime.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/invites")]
public class InvitesController(IInviteService inviteService) : ControllerBase
{
    private const string JoinGameBaseUrl = "join-game";
    private const string JoinGameByCodeUrl = $"{JoinGameBaseUrl}/by-code";

    [HttpGet(JoinGameByCodeUrl)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> JoinGameByCode([FromQuery] string code)
    {
        var result = await inviteService.JoinGameByCode(code);
        return result.TryGetFault(out var error, out var waitingRoomUrl)
            ? ErrorResponse(error)
            : Redirect(waitingRoomUrl);
    }

    private ActionResult ErrorResponse(IInviteService.InviteError error)
    {
        return error switch
        {
            IInviteService.InviteError.InviteNotFound => NotFound(),
            IInviteService.InviteError.InvalidCodeFormat => BadRequest(),
            _ => throw new ArgumentOutOfRangeException(error.ToString()),
        };
    }
}