using Microsoft.AspNetCore.Mvc;

namespace EatTogether.Controllers
{
    public class UsersController : Controller
    {
        // GET /Users/Index
        [HttpGet]
        public IActionResult Index() => View();
    }
}
