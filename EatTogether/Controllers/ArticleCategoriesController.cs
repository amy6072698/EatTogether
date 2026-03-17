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


	}
}
