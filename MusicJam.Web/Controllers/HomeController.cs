using Microsoft.AspNetCore.Mvc;

namespace MusicJam.Web.Controllers;

[Route("/")]
public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok("Hello World!");
    }
}