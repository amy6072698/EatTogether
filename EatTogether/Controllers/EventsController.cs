using EatTogether.Models.Extensions;
using EatTogether.Models.Services;
using EatTogether.Models.ViewModels;
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

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(EventCreateViewModel vm)
		{
			if (!ModelState.IsValid)
			{
				
				return View(vm);
			}

			var dto = vm.ToEditDto();
			_service.Create(dto);
			return View(vm);
		}

		public IActionResult Index()
		{
			var events = _service
				.GetAllForIndex()
				.Select(x => x.ToVm())
				.ToList();
			return View(events);
		}
	}
}
