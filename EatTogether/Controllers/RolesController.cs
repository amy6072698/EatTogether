using Microsoft.AspNetCore.Mvc;

namespace EatTogether.Controllers
{
	public class RolesController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
