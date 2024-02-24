using IdolBot.Extension;
using Microsoft.AspNetCore.Mvc;

namespace IdolBot.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
