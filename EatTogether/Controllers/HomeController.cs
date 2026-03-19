using EatTogether.Models;
using EatTogether.Models.Infra;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EatTogether.Controllers
{
	[RequireLogin]
	public class HomeController : Controller
    {
		// GET /Home/Index
		[HttpGet]
        public IActionResult Index()
        {
            return View();
        }

    }
}
