using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicJam.Web.Models.Constants;

namespace MusicJam.Web.Controllers;

[Route("/")]
public class HomeController : Controller
{
    // страница создана лишь для демонстрации работы авторизации
    [HttpGet("private")]
    [Authorize(AuthenticationSchemes = AuthSchemes.AccountJwtTokenScheme)]
    public IActionResult TestPrivatePage()
    {
        return Ok("There is an authentication");
    }
}
