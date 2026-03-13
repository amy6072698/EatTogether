using Microsoft.AspNetCore.Mvc;

namespace EatTogether.Controllers
{
    public class MembersController : Controller
    {
        // GET /Members/Index
        [HttpGet]
        public IActionResult Index() => View();
    }
}
