using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicJam.Studio.Constants;
using MusicJam.Studio.Extensions;
using MusicJam.Studio.Requests;
using MusicJam.Studio.Services.AuthService;

namespace MusicJam.Studio.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Login([FromBody] AuthRequest request)
    {
        var loginResult = await authService.Login(request);
        if (loginResult.TryGetFault(out var error, out var token))
        {
            return ErrorResponse(error);
        }

        Response.SetAccessTokenCookie(token);

        return Ok();
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Register([FromBody] RegisterRequest request)
    {
        var registerResult = await authService.Register(request);
        if (registerResult.TryGetFault(out var error, out var token))
        {
            return ErrorResponse(error);
        }

        Response.SetAccessTokenCookie(token);

        return Created();
    }

    [HttpGet("me")]
    [Authorize(AuthenticationSchemes = AuthConstants.DefaultAuthorizationScheme)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult Me()
    {
        return Ok();
    }

    private ActionResult ErrorResponse(IAuthService.AuthError error)
    {
        return error switch
        {
            IAuthService.AuthError.UserNotFound => NotFound(),
            IAuthService.AuthError.InvalidCredentials => BadRequest(),
            IAuthService.AuthError.SameEmailAlreadyExists => Conflict(),
            _ => throw new ArgumentOutOfRangeException(error.ToString()),
        };
    }
}
