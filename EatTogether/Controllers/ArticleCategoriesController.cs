using EatTogether.Models.Extensions;
using EatTogether.Models.Services;
using EatTogether.Models.ViewModels;
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

		public IActionResult Index()
		{
			return View();
		}

		// GET: ArticleCategory/Create
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		// POST: ArticleCategory/Create		
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(ArticleCategoryCreateViewModel vm)
		{
			if (!ModelState.IsValid)
			{
				return View(vm);
			}

			var dto = vm.ToCreateDto();
			await _service.CreateAsync(dto);
			TempData["SuccessMessage"] = "分類新增完成！";
			return RedirectToAction("Index");  //這會連到ArticleCategory/Index

		}


	}
}
