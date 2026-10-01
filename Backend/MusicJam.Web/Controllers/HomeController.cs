using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MusicJam.Web.Controllers;

[Route("/")]
public class HomeController : Controller
{
    // страница создана лишь для демонстрации работы авторизации
    [HttpGet("private")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult TestPrivatePage()
    {
        return Ok("There is an authentication");
    }
}
