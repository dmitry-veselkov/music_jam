using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult TestPrivatePage()
    {
        return Ok("There is an authentication");
    }
}