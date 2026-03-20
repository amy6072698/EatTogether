using EatTogether.Models.DTOs;
using EatTogether.Models.DTOs;
using EatTogether.Models.Infra;
using EatTogether.Models.Services;
using EatTogether.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace EatTogether.Controllers
{
	[RequirePermission("Menu_Manage")]
	public class CategoriesController : Controller
	{
		private readonly CategoryService _categoryService;
		private readonly DishService _dishService;

		public CategoriesController(CategoryService categoryService, DishService dishService)
		{
			_categoryService = categoryService;
			_dishService = dishService;
		}

		// GET: Categories
		public async Task<IActionResult> Index()
		{
			var dtos = await _categoryService.GetAllAsync();
			var vms = dtos.Select(d => d.ToViewModel()).ToList();

			// 取得所有餐點以供詳情顯示
			var allDishes = await _dishService.GetAllAsync();

			ViewBag.CategoriesJson = System.Text.Json.JsonSerializer.Serialize(
				vms.Select(vm => new {
					id = vm.Id,
					categoryName = vm.CategoryName,
					imageUrl = vm.ImageUrl,
					parentCategoryName = vm.ParentCategoryName,
					dishCount = vm.DishCount,
					dishes = allDishes.Where(d => d.CategoryId == vm.Id).Select(d => new {
						dishName = d.DishName,
						price = d.Price,
						isActive = d.IsActive
					})
				})
			);

			// 準備下拉選單給 Modal 使用
			ViewBag.ParentCategoryOptions = await GetParentCategoryOptionsAsync();

			return View(vms);
		}

		// 用於 Modal 提交的新增
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CategoryViewModel vm)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(new { message = "資料格式不正確" });
				}

				var allCategories = await _categoryService.GetAllAsync();
                
                // 改用「目前最大值 + 1」作為預設排序，這是最穩定的做法
                if (vm.DisplayOrder <= 0)
                {
                    vm.DisplayOrder = allCategories.Any() ? allCategories.Max(c => c.DisplayOrder) + 1 : 1;
                }

				await _categoryService.CreateAsync(vm.ToDto());
				return Ok(new { message = "新增成功" });
			}
			catch (Exception ex)
			{
                // 捕捉具體的資料庫或邏輯錯誤並回傳
                var innerMsg = ex.InnerException != null ? " (" + ex.InnerException.Message + ")" : "";
				return StatusCode(500, new { message = "新增失敗: " + ex.Message + innerMsg });
			}
		}

		// 用於 Modal 提交的編輯 (如果需要)
		[HttpPost]
		public async Task<IActionResult> Edit(int id, [FromBody] CategoryViewModel vm)
		{
			if (id != vm.Id) return BadRequest(new { message = "ID 不符" });
			
			if (!ModelState.IsValid)
			{
				var errors = ModelState.ToDictionary(
					kvp => kvp.Key,
					kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
				);
				return BadRequest(errors);
			}

			await _categoryService.UpdateAsync(vm.ToDto());
			return Ok(new { message = "更新成功" });
		}

		// 停用分類
		[HttpPost]
		public async Task<IActionResult> Disable(int id)
		{
			await _categoryService.DisableAsync(id);
			return Ok(new { message = "已停用" });
		}

		[HttpPost]
		public async Task<IActionResult> BatchDisable([FromBody] BatchRequestDto request)
		{
			if (request?.Ids == null || !request.Ids.Any()) return BadRequest("無項目可操作。");
			await _categoryService.BatchDisableAsync(request.Ids);
			return Ok();
		}

		[HttpPost]
		public async Task<IActionResult> Enable(int id)
		{
			await _categoryService.EnableAsync(id);
			return Ok();
		}

		[HttpPost]
		public async Task<IActionResult> BatchEnable([FromBody] BatchRequestDto request)
		{
			if (request?.Ids == null || !request.Ids.Any()) return BadRequest("無項目可操作。");
			await _categoryService.BatchEnableAsync(request.Ids);
			return Ok();
		}

		[HttpPost]
		public async Task<IActionResult> BatchDelete([FromBody] BatchRequestDto request)
		{
			if (request?.Ids == null || !request.Ids.Any()) return BadRequest("無項目可操作。");
			await _categoryService.BatchDeleteAsync(request.Ids);
			return Ok();
		}

		[HttpPost]
		public async Task<IActionResult> UpdateOrder([FromBody] OrderedIdsDto request)
		{
			if (request?.OrderedIds == null || !request.OrderedIds.Any()) return BadRequest("無順序資料。");
			await _categoryService.UpdateOrderAsync(request.OrderedIds);
			return Ok();
		}

		private async Task<List<SelectListItem>> GetParentCategoryOptionsAsync(int excludeId = 0)
		{
			var allCategories = await _categoryService.GetAllAsync();

			var options = allCategories
				.Where(c => c.Id != excludeId)
				.Select(c => new SelectListItem
				{
					Value = c.Id.ToString(),
					Text = c.CategoryName
				})
				.ToList();

			options.Insert(0, new SelectListItem { Value = "", Text = "（無，設為頂層分類）" }); 

			return options;
		}
		// ── 圖片上傳 ──────────────────────────────────────
		// 接收前端裁切後的 base64，存到 wwwroot/images/categories/
		// 回傳可直接使用的相對路徑 /images/categories/xxx.jpg

		[HttpPost]
		public async Task<IActionResult> UploadImage([FromBody] CategoryImageUploadRequest request)
		{
			try
			{
				if (string.IsNullOrEmpty(request?.Base64Data))
					return BadRequest("未提供圖片資料");

				var imageUrl = await SaveCategoryImageAsync(request.Base64Data, request.CategoryName);
				return Ok(new { imageUrl });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "圖片處理失敗: " + ex.Message });
			}
		}

		// ── 私有存檔方法（根據使用者要求修改）──
		private async Task<string> SaveCategoryImageAsync(string base64Data, string fileNamePrefix)
		{
			if (string.IsNullOrEmpty(base64Data)) return null;
			var base64 = base64Data.Contains(",") ? base64Data.Split(',')[1] : base64Data;
			var bytes = Convert.FromBase64String(base64);
			string fileName = $"{fileNamePrefix}.jpg";
			foreach (char c in Path.GetInvalidFileNameChars())
				fileName = fileName.Replace(c, '_');
			var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "categories");
			if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

			// 刪除同名的舊檔（.png / .jpeg）
			var baseName = Path.GetFileNameWithoutExtension(fileName);
			foreach (var ext in new[] { ".png", ".jpeg" })
			{
				var oldFile = Path.Combine(folderPath, baseName + ext);
				if (System.IO.File.Exists(oldFile)) System.IO.File.Delete(oldFile);
			}

			await System.IO.File.WriteAllBytesAsync(Path.Combine(folderPath, fileName), bytes);
			return "/images/categories/" + fileName;
		}
	}
}
