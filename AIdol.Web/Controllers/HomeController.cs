using AIdol.Extension;
using AIdol.Model;
using Microsoft.AspNetCore.Mvc;

namespace AIdol.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet("index")]
        public ApiResult Index()
        {
            return Success("³É¹¦");
        }
    }
}
