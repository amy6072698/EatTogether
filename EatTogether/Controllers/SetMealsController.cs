using EatTogether.Models.DTOs;
using EatTogether.Models.Infra;
using EatTogether.Models.Services;       
using EatTogether.Models.ViewModels;     
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EatTogether.Controllers        
{
	[RequirePermission("Menu_Manage")]
	public class SetMealsController : Controller
    {
        private readonly SetMealService _setMealService;
        private readonly DishService _dishService;
        private readonly CategoryService _categoryService;

        public SetMealsController(SetMealService setMealService, DishService dishService, CategoryService categoryService)
        {
            _setMealService = setMealService;
            _dishService    = dishService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var dtos = await _setMealService.GetAllAsync();
            var vms = dtos.Select(d =>
            {
                var vm = d.ToViewModel();
                if (string.IsNullOrEmpty(vm.ImageUrl))
                {
                    string safeName = vm.SetMealName;
                    foreach (char c in Path.GetInvalidFileNameChars())
                    {
                        safeName = safeName.Replace(c, '_');
                    }

                    var baseImagesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                    string jpgFileName = $"{safeName}.jpg";
                    string jpgPath = Path.Combine(baseImagesFolderPath, jpgFileName);
                    if (System.IO.File.Exists(jpgPath))
                    {
                        vm.ImageUrl = "/images/" + jpgFileName;
                    }
                    else
                    {
                        string pngFileName = $"{safeName}.png";
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
            var allSetMeals = await _setMealService.GetAllAsync();
            int nextOrder = allSetMeals.Any() ? allSetMeals.Max(s => s.DisplayOrder) + 1 : 1;
            
            // Prepare data for the new UI
            var vm = new SetMealViewModel { DisplayOrder = nextOrder };
            await PopulateCategoriesWithDishes(vm);
            
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] SetMealViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCategoriesWithDishes(vm); // Repopulate if validation fails
                return View(vm);
            }

            if (!string.IsNullOrEmpty(vm.CroppedImageData))
                vm.ImageUrl = await SaveBase64ImageAsync(vm.CroppedImageData, vm.SetMealName);

            // New logic to set DisplayOrder
            var allSetMeals = await _setMealService.GetAllAsync();
            vm.DisplayOrder = allSetMeals.Any() ? allSetMeals.Min(s => s.DisplayOrder) - 1 : 1;

            await _setMealService.CreateAsync(vm.ToDto());
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _setMealService.GetByIdAsync(id);
            if (dto == null) return NotFound();

            var vm = dto.ToViewModel();

            // Populate CategoriesWithDishes for the new UI
            await PopulateCategoriesWithDishes(vm);

			if (string.IsNullOrEmpty(vm.ImageUrl))
			{
				string safeName = vm.SetMealName;
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
        public async Task<IActionResult> Edit(int id, [FromForm] SetMealViewModel vm)
        {
            if (id != vm.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                await PopulateCategoriesWithDishes(vm); // Repopulate if validation fails
                return View(vm);
            }

            if (!string.IsNullOrEmpty(vm.CroppedImageData))
            {
                // 強制覆蓋原有檔案，並用餐點名稱命名
                vm.ImageUrl = await SaveBase64ImageAsync(vm.CroppedImageData, vm.SetMealName);
            }

            await _setMealService.UpdateAsync(vm.ToDto());
            return RedirectToAction(nameof(Index));
        }
        
        // Helper method to populate CategoriesWithDishes
        private async Task PopulateCategoriesWithDishes(SetMealViewModel vm)
        {
            var allCategories = await _categoryService.GetAllAsync();
            var allActiveDishes = await _dishService.GetAllAsync(); // This already filters for IsActive

            var categoriesWithDishes = new List<CategoryWithDishesViewModel>();

            foreach (var category in allCategories.OrderBy(c => c.DisplayOrder)) // Assuming categories have DisplayOrder
            {
                var categoryVm = new CategoryWithDishesViewModel
                {
                    CategoryId = category.Id,
                    CategoryName = category.CategoryName,
                    DishesInThisCategory = allActiveDishes
                        .Where(d => d.CategoryId == category.Id)
                        .Select(d => new SelectListItem
                        {
                            Value = d.Id.ToString(),
                            Text = $"{d.DishName} (${d.Price})",
                            Selected = vm.Items.Any(item => item.DishId == d.Id) // Pre-select in dropdown if already in set meal
                        }).ToList()
                };

                // Populate SelectedItemsForCategory for rendering existing items
                categoryVm.SelectedItemsForCategory = vm.Items
                    .Where(item => allActiveDishes.Any(d => d.Id == item.DishId && d.CategoryId == category.Id))
                    .ToList();
                
                // Set category-level optionality based on the first optional item in this category (if any)
                var firstOptionalItem = categoryVm.SelectedItemsForCategory.FirstOrDefault(i => i.IsOptional);
                if (firstOptionalItem != null)
                {
                    categoryVm.IsCategoryOptional = true;
                    categoryVm.OptionGroupNoForCategory = firstOptionalItem.OptionGroupNo;
                    categoryVm.PickLimitForCategory = firstOptionalItem.PickLimit;
                }

                categoriesWithDishes.Add(categoryVm);
            }
            vm.CategoriesWithDishes = categoriesWithDishes;
        }

        [HttpPost]
        public async Task<IActionResult> Disable(int id)
        {
            await _setMealService.DisableAsync(id);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> BatchDisable([FromBody] BatchRequestDto request)
        {
            if (request?.Ids == null || !request.Ids.Any()) return BadRequest("無項目可操作。");
            await _setMealService.BatchDisableAsync(request.Ids);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Enable(int id)
        {
            await _setMealService.EnableAsync(id);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> BatchEnable([FromBody] BatchRequestDto request)
        {
            if (request?.Ids == null || !request.Ids.Any()) return BadRequest("無項目可操作。");
            await _setMealService.BatchEnableAsync(request.Ids);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> BatchDelete([FromBody] BatchRequestDto request)
        {
            if (request?.Ids == null || !request.Ids.Any()) return BadRequest("無項目可操作。");
            await _setMealService.BatchDeleteAsync(request.Ids);
            return Ok();
        }

        private async Task<string> SaveBase64ImageAsync(string base64Data, string fileNamePrefix)
        {
            if (string.IsNullOrEmpty(base64Data)) return null;

            var base64 = base64Data.Contains(",") ? base64Data.Split(',')[1] : base64Data;
            var bytes = Convert.FromBase64String(base64);

            // 移除檔名中不合法的字元
            string sanitizedPrefix = fileNamePrefix;
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                sanitizedPrefix = sanitizedPrefix.Replace(c, '_');
            }

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // 存檔前先刪除同名的 .png 和 .jpeg 舊檔
            string pngToDelete = Path.Combine(folderPath, $"{sanitizedPrefix}.png");
            if (System.IO.File.Exists(pngToDelete))
            {
                System.IO.File.Delete(pngToDelete);
            }
            string jpegToDelete = Path.Combine(folderPath, $"{sanitizedPrefix}.jpeg");
            if (System.IO.File.Exists(jpegToDelete))
            {
                System.IO.File.Delete(jpegToDelete);
            }

            // 儲存新的 .jpg 檔案
            string newJpgFileName = $"{sanitizedPrefix}.jpg";
            var savePath = Path.Combine(folderPath, newJpgFileName);
            await System.IO.File.WriteAllBytesAsync(savePath, bytes);

            return "/images/" + newJpgFileName;
        }

        [HttpPost("SetMeals/UpdateItems/{setMealId}")]
        public async Task<IActionResult> UpdateItems(int setMealId, [FromBody] List<SetMealItemViewModel> items)
        {
            if (items == null) return BadRequest("無項目可更新。");

            try
            {
                var dtos = items.Select(i => i.ToItemDto());
                await _setMealService.UpdateItemsAsync(setMealId, dtos);
                return Ok(new { message = "套餐內容更新成功！" });
            }
            catch (Exception ex)
            {
                // Log the exception
                return BadRequest(new { message = "更新失敗：" + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrder([FromBody] OrderedIdsDto dto)
        {
            if (dto?.OrderedIds == null || !dto.OrderedIds.Any())
            {
                return BadRequest("No IDs provided for reordering.");
            }

            try
            {
                await _setMealService.UpdateOrderAsync(dto.OrderedIds);
                return Ok(new { message = "Order updated successfully." });
            }
            catch (Exception ex)
            {
                // In a real app, log this exception
                return StatusCode(500, "An error occurred while updating the order.");
            }
        }
    }
}
