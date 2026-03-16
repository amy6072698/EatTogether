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

		// GET: ArticleCategory/Create
		[HttpGet]
		public async Task<IActionResult> Create()
		{
			var dto = new ArticleCategoryDto { IsEnabled = true, SortOrder = 0 };

			// 如果是 AJAX 請求
			if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
			{
				return PartialView("_CreatePartial", dto);
			}

			return View(dto);
		}

		// POST: ArticleCategory/Create		
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(ArticleCategoryCreateViewModel vm)
		{
			var dto = vm.ToCreateDto();

			if (ModelState.IsValid)
			{
				await _service.CreateAsync(dto);
				TempData["SuccessMessage"] = "分類新增完成！";

				if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
				{
					//return Json(new { success = true });
					return Json(new { success = true, redirectUrl = Url.Action("Index") });
				}

				return RedirectToAction(nameof(Index));
			}

			// 驗證失敗：同樣判斷回傳 Partial 或 View
			if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
			{
				return PartialView("_CreatePartial", dto);
			}
			return View(dto);


		}


	}
}
