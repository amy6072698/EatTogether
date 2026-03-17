using EatTogether.Models.Services;
using EatTogether.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EatTogether.Controllers
{
	//[Authorize]
	public class ArticlesController : Controller
	{
		private readonly ArticleService _service;

		public ArticlesController(ArticleService service)
		{
			_service = service;
		}

		public IActionResult Index()
		{
			return View();
		}

		public async Task<IActionResult> Create(ArticleCreateViewModel vm)
		{
			if (ModelState.IsValid)
			{
				if (vm.CoverImageFile != null)
				{
					// 檢查檔案格式
					var supportedTypes = new[] { ".jpg", ".jpeg", ".png", ".webp" };
					var fileExt = Path.GetExtension(vm.CoverImageFile.FileName).ToLower();

					if (!supportedTypes.Contains(fileExt))
					{
						ModelState.AddModelError("CoverImageFile", "僅支援 JPG, PNG, WEBP 格式圖片");
						// 重新載入下拉選單並回傳 View
						vm.CategorySelectList = /* ... */;
						return View(vm);
					}

					// ... 執行檔案上傳與存檔邏輯 ...
				}
				// ... 
			}

			return View(vm);
		}
	}
}
