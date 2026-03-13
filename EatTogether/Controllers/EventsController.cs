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
		public async Task<IActionResult> Create(EventCreateViewModel vm)
		{
			if (!ModelState.IsValid)
			{
				return View(vm);
			}

			var dto = vm.ToCreateDto();
			await _service.CreateAsync(dto);
			return View(vm);
		}

		public async Task<IActionResult> Index()
		{
			var events = (await _service.GetAllForIndexAsync())
				.Select(x => x.ToEventVm())
				.ToList();
			return View(events);
		}
	}
}
