using AIdol.Extension;
using Microsoft.AspNetCore.Mvc;

namespace AIdol.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
