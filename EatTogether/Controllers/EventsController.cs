using EatTogether.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EatTogether.Controllers
{
	public class EventsController : Controller
	{
		private readonly EventService _service;

		public EventsController(EventService service)
		{
			_service = service;
		}

		//[Authorize]
		public IActionResult Create()
		{
			return View();
		}

		public IActionResult Index()
		{
			return View();
		}
	}
}
