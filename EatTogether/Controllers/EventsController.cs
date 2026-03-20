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
			var vm = new EventCreateViewModel();
			vm.DishOptions = await _service.GetDishOptionsAsync();
			return View(vm);
		}

		// POST: Event/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(EventCreateViewModel vm)
		{
			if (!ModelState.IsValid)
			{
				vm.DishOptions = await _service.GetDishOptionsAsync();
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
			vm.DishOptions = await _service.GetDishOptionsAsync();  // 補這行
			return View(vm);

		}

		// POST: Event/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(EventEditViewModel vm)
		{
			if (!ModelState.IsValid)
			{
				vm.DishOptions = await _service.GetDishOptionsAsync();
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
			vm.DishOptions = await _service.GetDishOptionsAsync();
			return View("Create", vm);
		}

		/// <summary>AJAX：依消費金額回傳符合條件的進行中活動（供點餐確認頁自動套用）</summary>
		[HttpGet]
		public async Task<IActionResult> GetApplicableEvents(int amount)
		{
			var events = await _service.GetApplicableEventsAsync(amount);
			return Json(events);
		}

		/// <summary>診斷用：回傳所有活動的狀態（Debug 用，上線前可移除）</summary>
		[HttpGet]
		public async Task<IActionResult> DiagEvents(int amount = 0)
		{
			var all = await _service.GetAllForIndexAsync();
			var today = DateTime.Today;
			return Json(new
			{
				today = today.ToString("yyyy-MM-dd"),
				amount,
				events = all.Select(e => new
				{
					e.Id,
					e.Title,
					e.Status,
					startDate = e.StartDate.ToString("yyyy-MM-dd"),
					endDate = e.EndDate.ToString("yyyy-MM-dd"),
					e.MinSpend,
					e.DiscountType,
					e.DiscountValue,
					isDateOk = e.StartDate.Date <= today && e.EndDate.Date >= today,
					isAmountOk = amount > 0 && e.MinSpend <= amount,
					isStatusOk = e.Status == 1
				})
			});
		}
	}
}
