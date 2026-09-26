using Microsoft.AspNetCore.Mvc;

namespace MusicJam.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ILogger<AuthController> logger) : ControllerBase
{
    // эндпоинт создан лишь для демонстрации работы с апи
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        logger.LogInformation("GET query on {Controller}", nameof(AuthController));
        return Ok("Test auth api controller");
    }
}