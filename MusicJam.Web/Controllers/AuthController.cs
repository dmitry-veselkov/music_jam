using Microsoft.AspNetCore.Mvc;
using MusicJam.Domain.Models.Auth;
using MusicJam.Domain.Services.AuthServices;
using MusicJam.Web.Models.Requests;

namespace MusicJam.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IAuthService authService,
    ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        var authCredentials = new AuthCredentials(request.Email, request.Password);
        var loginResult = await authService.Login(authCredentials);

        return loginResult.IsSuccess ? Ok() : ErrorResponse(loginResult.Fault);
    }

    private IActionResult ErrorResponse(IAuthService.AuthError error)
    {
        return error switch
        {
            IAuthService.AuthError.SameUserAlreadyExists => Conflict(),
            _ => throw new ArgumentOutOfRangeException(error.ToString()),
        };
    }
}