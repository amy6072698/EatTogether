using EatTogether.Models.Services;
using EatTogether.Models.ViewModels;
using EatTogether.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EatTogether.Controllers
{
    public class DishesController : Controller
    {
        private readonly DishService _dishService;
        private readonly CategoryService _categoryService;

        public DishesController(DishService dishService, CategoryService categoryService)
        {
            _dishService     = dishService;
            _categoryService = categoryService;
        }

		public async Task<IActionResult> Index(bool newDish = false)
		{
            var dtos = await _dishService.GetAllAsync();
            var vms = dtos.Select(d => {
                var vm = d.ToViewModel();
                if (string.IsNullOrEmpty(vm.ImageUrl))
                {
                    // Sanitize dish name for filename comparison
                    string safeDishName = vm.DishName;
                    foreach (char c in Path.GetInvalidFileNameChars())
                    {
                        safeDishName = safeDishName.Replace(c, '_');
                    }

                    // Determine the base path for wwwroot/images
                    var baseImagesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                    // Check for .jpg file
                    string jpgFileName = $"{safeDishName}.jpg";
                    string jpgPath = Path.Combine(baseImagesFolderPath, jpgFileName);
                    if (System.IO.File.Exists(jpgPath))
                    {
                        vm.ImageUrl = "/images/" + jpgFileName;
                    }
                    else
                    {
                        // Check for .png file if .jpg is not found
                        string pngFileName = $"{safeDishName}.png";
                        string pngPath = Path.Combine(baseImagesFolderPath, pngFileName);
                        if (System.IO.File.Exists(pngPath))
                        {
                            vm.ImageUrl = "/images/" + pngFileName;
                        }
                    }
                }
                return vm;
            }).ToList();
            return View(vms);
        }

        public async Task<IActionResult> Create()
        {
            var allDishes = await _dishService.GetAllAsync();
            int nextOrder = allDishes.Any() ? allDishes.Max(d => d.DisplayOrder) + 1 : 1;
            var vm = new DishViewModel { DisplayOrder = nextOrder };
            vm.CategoryOptions = await GetCategoryOptionsAsync();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] DishViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.CategoryOptions = await GetCategoryOptionsAsync();
                return View(vm);
            }

            if (!string.IsNullOrEmpty(vm.CroppedImageData))
            {
                // 新增時，直接用餐點名稱命名
                vm.ImageUrl = await SaveBase64ImageAsync(vm.CroppedImageData, vm.DishName);
            }

            await _dishService.CreateAsync(vm.ToDto());
			return RedirectToAction(nameof(Index), new { newDish = true });
		}

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _dishService.GetByIdAsync(id);
            if (dto == null) return NotFound();
            var vm = dto.ToViewModel();
            vm.CategoryOptions = await GetCategoryOptionsAsync();

			// 若資料庫沒有圖片路徑，自動用餐點名稱去找本地檔案
			if (string.IsNullOrEmpty(vm.ImageUrl))
			{
				string safeName = vm.DishName;
				foreach (char c in Path.GetInvalidFileNameChars())
					safeName = safeName.Replace(c, '_');
				var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
				if (System.IO.File.Exists(Path.Combine(folder, safeName + ".jpg")))
					vm.ImageUrl = "/images/" + safeName + ".jpg";
				else if (System.IO.File.Exists(Path.Combine(folder, safeName + ".png")))
					vm.ImageUrl = "/images/" + safeName + ".png";
			}
			return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] DishViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                vm.CategoryOptions = await GetCategoryOptionsAsync();
                return View(vm);
            }

            if (!string.IsNullOrEmpty(vm.CroppedImageData))
            {
                // 【核心修改】：編輯時，直接拿「目前的餐點名稱」去覆蓋檔案
                vm.ImageUrl = await SaveBase64ImageAsync(vm.CroppedImageData, vm.DishName);
            }

            await _dishService.UpdateAsync(vm.ToDto());
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Disable(int id)
        {
            await _dishService.DisableAsync(id);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> BatchDisable([FromBody] BatchRequestDto request)
        {
            if (request?.Ids == null || !request.Ids.Any()) return BadRequest("無項目可操作。");
            await _dishService.BatchDisableAsync(request.Ids);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Enable(int id)
        {
            await _dishService.EnableAsync(id);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> BatchEnable([FromBody] BatchRequestDto request)
        {
            if (request?.Ids == null || !request.Ids.Any()) return BadRequest("無項目可操作。");
            await _dishService.BatchEnableAsync(request.Ids);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> BatchDelete([FromBody] BatchRequestDto request)
        {
            if (request?.Ids == null || !request.Ids.Any()) return BadRequest("無項目可操作。");
            await _dishService.BatchDeleteAsync(request.Ids);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrder([FromBody] OrderedIdsDto request)
        {
            if (request?.OrderedIds == null || !request.OrderedIds.Any()) return BadRequest("無順序可更新。");
            await _dishService.UpdateOrderAsync(request.OrderedIds);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var dish = await _dishService.GetByIdAsync(id);
            if (dish == null) return NotFound();
            dish.IsActive = !dish.IsActive;
            await _dishService.UpdateAsync(dish);
            return Ok(new { isActive = dish.IsActive });
        }

        public async Task<IActionResult> GetAllJson()
        {
            var dtos = await _dishService.GetAllAsync();
            return Json(dtos.Select(d => new { id = d.Id, dishName = d.DishName, price = d.Price }));     
        }

        private async Task<List<SelectListItem>> GetCategoryOptionsAsync()
        {
            var categories = await _categoryService.GetAllAsync();
            return categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.CategoryName }).ToList();
        }

        /// <summary>
        /// 儲存裁切後的圖片。存檔前會先刪除同名的 .png 和 .jpeg 檔案，確保只保留最新的 .jpg。
        /// </summary>
        private async Task<string> SaveBase64ImageAsync(string base64Data, string dishName)
        {
            if (string.IsNullOrEmpty(base64Data)) return null;

            var base64 = base64Data.Contains(",") ? base64Data.Split(',')[1] : base64Data;
            var bytes = Convert.FromBase64String(base64);

            // 移除檔名中不合法的字元
            string fileNamePrefix = dishName;
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileNamePrefix = fileNamePrefix.Replace(c, '_');
            }

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // 刪除舊的 .png 和 .jpeg 檔案
            string pngToDelete = Path.Combine(folderPath, $"{fileNamePrefix}.png");
            if (System.IO.File.Exists(pngToDelete))
            {
                System.IO.File.Delete(pngToDelete);
            }
            string jpegToDelete = Path.Combine(folderPath, $"{fileNamePrefix}.jpeg");
            if (System.IO.File.Exists(jpegToDelete))
            {
                System.IO.File.Delete(jpegToDelete);
            }

            // 儲存新的 .jpg 檔案
            string newJpgFileName = $"{fileNamePrefix}.jpg";
            var savePath = Path.Combine(folderPath, newJpgFileName);
            await System.IO.File.WriteAllBytesAsync(savePath, bytes);

            return "/images/" + newJpgFileName;
        }
    }
}
