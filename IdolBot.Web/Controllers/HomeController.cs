using IdolBot.Extension;
using Microsoft.AspNetCore.Mvc;

namespace IdolBot.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
