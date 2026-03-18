using EatTogether.Models.Extensions;
using EatTogether.Models.Services;
using EatTogether.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace EatTogether.Controllers
{
	//[Authorize]
	public class ArticlesController : Controller
	{
		private readonly ArticleService _service;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public ArticlesController(ArticleService service, IWebHostEnvironment webHostEnvironment)
		{
			_service = service;
			_webHostEnvironment = webHostEnvironment;
		}

		// GET: Articles/Index
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var data = await _service.GetAllForIndexAsync();
			var viewModels = data.Select(dto => dto.ToArticleVm());

			// 判斷是否為 Ajax 請求 (或是檢查 Accept Header)
			if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
			{
				return Json(viewModels); //回傳 JSON 格式
			}
			return View(viewModels); 
		}


		[HttpGet]
		public async Task<IActionResult> Create()
		{
			var vm = new ArticleCreateViewModel
			{
				// 注入真實資料到下拉選單
				CategorySelectList = await _service.GetCategorySelectListAsync(),
				EventSelectList = await _service.GetEventSelectListAsync()
			};

			return View(vm);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(ArticleCreateViewModel vm)
		{

			if (ModelState.IsValid)
			{
				// 先印出來確認值
				Console.WriteLine($"CategoryId = {vm.CategoryId}");

				// 檔案處理邏輯
				if (vm.CoverImageFile != null && vm.CoverImageFile.Length > 0)
				{
					// 檢查格式
					var supportedTypes = new[] { ".jpg", ".jpeg", ".png", ".webp" };
					var fileExt = Path.GetExtension(vm.CoverImageFile.FileName).ToLower();

					if (!supportedTypes.Contains(fileExt))
					{
						ModelState.AddModelError("CoverImageFile", "僅支援 JPG, PNG, WEBP 格式圖片");
						await PopulateSelectListsAsync(vm);
						return View(vm);
					}

					// 執行存檔 (建立唯一檔名防止覆蓋)
					string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "articles");
					if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

					string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(vm.CoverImageFile.FileName);
					string filePath = Path.Combine(uploadsFolder, uniqueFileName);

					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						await vm.CoverImageFile.CopyToAsync(fileStream);
					}

					// 將檔案路徑存入 VM (稍後轉給 DTO 存入資料庫)
					vm.CoverImageUrl = "/uploads/articles/" + uniqueFileName;

				}

				// 3. 呼叫 Service 存檔 (將 VM 轉為 DTO)
				try
				{
					var dto = vm.ToCreateDto(); 
					await _service.CreateAsync(dto);

					TempData["SuccessMessage"] = vm.Status == 1 ? "文章發佈成功！" : "草稿已儲存";
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					// 處理資料庫儲存失敗的情境
					ModelState.AddModelError("", "存檔失敗：" + ex.Message);
					await PopulateSelectListsAsync(vm);
					return View(vm);
				}

			}

			await PopulateSelectListsAsync(vm);
			return View(vm);
		}

		// 輔助方法：統一處理選單重載，避免程式碼重複
		private async Task PopulateSelectListsAsync(ArticleCreateViewModel vm)
		{
			vm.CategorySelectList = await _service.GetCategorySelectListAsync();
			vm.EventSelectList = await _service.GetEventSelectListAsync();
		}
	}
}
