using EatTogether.Models.DTOs;
using EatTogether.Models.Extensions;
using EatTogether.Models.Services;
using EatTogether.Models.ViewModels;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EatTogether.Controllers
{
	//[Authorize]
	public class ArticleCategoriesController : Controller
	{
		private readonly ArticleCategoryService _service;

		public ArticleCategoriesController(ArticleCategoryService service)
		{
			_service = service;
		}

		// GET: ArticleCategory/Index
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var data = await _service.GetAllForIndexAsync();

			// 判斷是否為 Ajax 請求 (或是檢查 Accept Header)
			if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
			{
				return Json(data); //回傳 JSON 格式
			}

			return View(data); // 

		}


		[HttpGet]
		public async Task<IActionResult> Create()
		{
			// 非 AJAX 請求直接導回列表
			if (Request.Headers["X-Requested-With"] != "XMLHttpRequest")
				return RedirectToAction(nameof(Index));

			var vm = new ArticleCategoryCreateViewModel
			{
				IsEnabled = true,
				//SortOrder = 0
			};

			if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
				return PartialView("_CreatePartial", vm);

			return View(vm);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(ArticleCategoryCreateViewModel vm)
		{
			if (ModelState.IsValid)
			{
				if (await _service.IsSortOrderDuplicateAsync(vm.SortOrder.Value, vm.Id))
				{
					ModelState.AddModelError("SortOrder", "此排序號碼已被使用");
					return PartialView("_CreatePartial", vm);
				}

				var dto = vm.ToCreateDto();
				await _service.CreateAsync(dto);
				TempData["SuccessMessage"] = "分類新增完成！";

				if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
					return Json(new { success = true, redirectUrl = Url.Action("Index") });

				return RedirectToAction(nameof(Index));
			}

			// 驗證失敗傳 vm，不是 dto
			if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
				return PartialView("_CreatePartial", vm);

			return View(vm);
		}


		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			//// 非 AJAX 請求直接導回列表
			//if (Request.Headers["X-Requested-With"] != "XMLHttpRequest")
			//	return RedirectToAction(nameof(Index));

			//var dto = await _service.GetEditByIdAsync(id);

			//if (dto == null)
			//{
			//	return NotFound();
			//}

			//var vm = dto.ToEditVm();	

			//if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
			//	return PartialView("_EditPartial", vm);

			//return View(vm);

			var dto = await _service.GetEditByIdAsync(id);
			if (dto == null) return NotFound();

			var vm = dto.ToEditVm();
			return PartialView("_EditPartial", vm);

		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(ArticleCategoryEditViewModel vm)
		{
			if (ModelState.IsValid)
			{
				if (await _service.IsSortOrderDuplicateAsync(vm.SortOrder, vm.Id)) {
					ModelState.AddModelError("SortOrder", "此排序號碼已被使用");
					return PartialView("_EditPartial", vm);
				}

				var dto = vm.ToEditDto();
				await _service.EditAsync(dto);
				TempData["SuccessMessage"] = "類別編輯完成！";

				if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
					return Json(new { success = true, redirectUrl = Url.Action("Index") });

				return RedirectToAction(nameof(Index));
			}

			// 驗證失敗傳 vm，不是 dto
			if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
				return PartialView("_EditPartial", vm);

			return View(vm);
		}


	}
}
