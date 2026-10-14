using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicJam.Studio.Constants;
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
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        var loginResult = await authService.Login(request);
        if (loginResult.TryGetFault(out var error, out var token))
        {
            return ErrorResponse(error);
        }

        Response.Cookies.Append(AuthConstants.AccessToken, token, GetTokenCookieOptions());

        return Ok();
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var registerResult = await authService.Register(request);
        if (registerResult.TryGetFault(out var error, out var token))
        {
            return ErrorResponse(error);
        }

        Response.Cookies.Append(AuthConstants.AccessToken, token, GetTokenCookieOptions());

        return Created();
    }

    [HttpGet("me")]
    [Authorize(AuthenticationSchemes = AuthConstants.DefaultAuthorizationScheme)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Me()
    {
        return Ok();
    }

    private IActionResult ErrorResponse(IAuthService.AuthError error)
    {
        return error switch
        {
            IAuthService.AuthError.UserNotFound => NotFound(),
            IAuthService.AuthError.InvalidCredentials => BadRequest(),
            IAuthService.AuthError.SameEmailAlreadyExists => Conflict(),
            _ => throw new ArgumentOutOfRangeException(error.ToString()),
        };
    }

    private static CookieOptions GetTokenCookieOptions() => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Expires = DateTimeOffset.UtcNow.AddDays(AuthConstants.AccessTokenLifetimeInDays)
    };
}
