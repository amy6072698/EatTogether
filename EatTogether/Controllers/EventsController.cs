using EatTogether.Models.DTOs;
using EatTogether.Models.Extensions;
using EatTogether.Models.Services;
using EatTogether.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Controllers
{
	//[Authorize]
	public class EventsController : Controller
	{
		private readonly EventService _service;

		public EventsController(EventService service)
		{
			_service = service;
		}

		// GET: Event/Create
		[HttpGet]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create()
		{
			return View();
		}

		// POST: Event/Create
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

		[HttpGet]
		// GET: Event/Index
		public async Task<IActionResult> Index()
		{
			var events = (await _service.GetAllForIndexAsync())
				.Select(x => x.ToEventVm())
				.ToList();
			return View(events);
		}

		[HttpGet]
		// GET: Event/Edit/5
		public async Task<IActionResult> Edit(int id)
		{
			var dto = await _service.GetEditByIdAsync(id);

			if (dto == null)
			{
				return NotFound();
			}

			var vm = dto.ToEditVm();

			return View(vm);
		}

		// POST: Event/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(EventEditDto dto)
		{
			var result = await _service.EditAsync(dto);
			if (result.Success)
			{
				// 使用 TempData 把 Service 裡的 "編輯完成！" 文字傳到下一頁
				TempData["SuccessMessage"] = result.Message;
				return RedirectToAction("Index");
			}

			ModelState.AddModelError("", result.Message);
			return View(dto);
		}

		// POST: Event/Deactivate/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeactivateAsync(int id)
		{
			await _service.DeactivateAsync(id);
			TempData["Success"] = "活動已停用";
			return RedirectToAction("Index");
		}

	}
}
