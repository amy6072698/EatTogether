using EatTogether.Models.DTOs;
using EatTogether.Models.Extensions;
using EatTogether.Models.Infra;
using EatTogether.Models.Services;
using EatTogether.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Controllers
{
	//[Authorize]
	[RequirePermission("Event_Manage")]
	public class EventsController : Controller
	{
		private readonly EventService _service;

		public EventsController(EventService service)
		{
			_service = service;
		}

		// GET: Event/Create
		[HttpGet]
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
			TempData["SuccessMessage"] = "活動新增完成！";
			return RedirectToAction("Index");

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

		// GET: Event/Edit/5
		[HttpGet]
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
		public async Task<IActionResult> Edit(EventEditViewModel vm)
		{
			if (!ModelState.IsValid)
			{
				return View(vm);
			}
			var dto = vm.ToEditDto();
			var result = await _service.EditAsync(dto);

			if (result.Success)
			{
				ViewData["SuccessMessage"] = "活動編輯完成！";
			}

			return View(vm);
		}


		// POST: Event/Deactivate/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Deactivate(int id)
		{
			await _service.DeactivateAsync(id);
			TempData["SuccessMessage"] = "活動已停用";
			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<IActionResult> CopyCreate(int id)
		{
			var vm = await _service.GetCopyCreateVm(id);
			if (vm == null) return NotFound();

			return View("Create", vm);
		}


	}
}
