using Microsoft.AspNetCore.Mvc;
using MusicJam.Domain.Models.Auth;
using MusicJam.Domain.Services.AuthServices;
using MusicJam.Web.Models.Requests;

namespace MusicJam.Web.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        // todo veselkov.d добавить проверку, что пользак уже авторизован

        var authCredentials = new AuthCredentials(request.Email, request.Password);
        var loginResult = await authService.Login(authCredentials);

        if (loginResult.IsFailure)
        {
            return ErrorResponse(loginResult.Fault);
        }

        var token = loginResult.GetValueOrThrow();
        // todo veselkov.d access_token нужно вынести в константу
        Response.Cookies.Append("access_token", token, GetTokenCookieOptions());

        return Ok();
    }

    private IActionResult ErrorResponse(IAuthService.AuthError error)
    {
        return error switch
        {
            IAuthService.AuthError.UserNotFound => NotFound(),
            IAuthService.AuthError.InvalidCredentials => BadRequest(),
            _ => throw new ArgumentOutOfRangeException(error.ToString()),
        };
    }

    private CookieOptions GetTokenCookieOptions() => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        // todo veselkov.d мне не нравится, что время жизни куки и жизнь самого токена никак не связаны и  захардкожены
        Expires = DateTimeOffset.UtcNow.AddDays(3)
    };
}