using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicJam.Web.Models.Constants;

namespace MusicJam.Web.Controllers;

[Route("/")]
public class HomeController : Controller
{
    // todo veselkov.d когда будет готов фронт, начать отдавать здесь index.html из dist
    [HttpGet]
    public IActionResult Index()
    {
        return Ok("Hello World!");
    }

    // страница создана лишь для демонстрации работы авторизации
    [HttpGet("private")]
    [Authorize(AuthenticationSchemes = AuthSchemes.AccountJwtTokenScheme)]
    public IActionResult TestPrivatePage()
    {
        return Ok("There is an authentication");
    }
}