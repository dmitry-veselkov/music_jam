using Microsoft.AspNetCore.Mvc;
using MusicJam.Runtime.Middlewares;
using MusicJam.Runtime.Services.GameService;
using MusicJam.Runtime.Services.UserService;
using MusicJam.Studio.Extensions;

namespace MusicJam.Runtime.Controllers;


[ApiController]
[Route("/api/runtime/games/{gameId:guid}/users")]

public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet("check-role")]
    public async Task<IActionResult> CheckRole(Guid gameId)
    {
        var session = HttpContext.GetSessionId();
        var result = await userService.GetRole(gameId, session);
        if (result.TryGetFault(out var error, out var role))
            return ErrorResponse(error);
        return Ok(role);
    }
    
    [HttpGet("check-host")]
    public async Task<IActionResult> IsHost(Guid gameId)
    {
        Guid? userId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : null;
        var result = await userService.IsHost(gameId, userId);
        if (result.TryGetFault(out var error, out var isHost))
            return ErrorResponse(error);
        return Ok(new { isHost });
    }
    
    private ActionResult ErrorResponse(IUserService.UserError error)
    {
        return error switch
        {
            IUserService.UserError.GameNotFound => NotFound(),
            _ => throw new ArgumentOutOfRangeException(error.ToString()),
        };
    }
}