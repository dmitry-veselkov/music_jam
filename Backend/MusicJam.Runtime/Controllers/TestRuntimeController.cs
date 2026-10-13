using Microsoft.AspNetCore.Mvc;

namespace MusicJam.Runtime.Controllers;

[ApiController]
[Route("api/games")]
public class TestRuntimeController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok();
}
