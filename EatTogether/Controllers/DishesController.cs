using EatTogether.Models.Services;
using EatTogether.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        // GET: /Dishes
        public async Task<IActionResult> Index()
        {
            var dtos = await _dishService.GetAllAsync();
            var vms = dtos.Select(d => d.ToViewModel()).ToList();
            return View(vms);
        }

        // GET: /Dishes/Create
        public async Task<IActionResult> Create()
        {
            var vm = new DishViewModel();
            vm.CategoryOptions = await GetCategoryOptionsAsync();
            return View(vm);
        }

        // POST: /Dishes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] DishViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.CategoryOptions = await GetCategoryOptionsAsync();
                return View(vm);
            }

            // 處理裁切後的 Base64 圖片
            if (!string.IsNullOrEmpty(vm.CroppedImageData))
            {
                vm.ImageUrl = await SaveBase64ImageAsync(vm.CroppedImageData);
            }

            await _dishService.CreateAsync(vm.ToDto());
            return RedirectToAction(nameof(Index));
        }

        // GET: /Dishes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _dishService.GetByIdAsync(id);
            if (dto == null) return NotFound();
            var vm = dto.ToViewModel();
            vm.CategoryOptions = await GetCategoryOptionsAsync();
            return View(vm);
        }

        // POST: /Dishes/Edit/5
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

            // 處理裁切後的 Base64 圖片
            if (!string.IsNullOrEmpty(vm.CroppedImageData))
            {
                vm.ImageUrl = await SaveBase64ImageAsync(vm.CroppedImageData);
            }

            await _dishService.UpdateAsync(vm.ToDto());
            return RedirectToAction(nameof(Index));
        }

        // POST: /Dishes/Disable/5
        [HttpPost]
        public async Task<IActionResult> Disable(int id)
        {
            await _dishService.DisableAsync(id);
            return Ok();
        }

        // GET: /Dishes/GetAllJson
        public async Task<IActionResult> GetAllJson()
        {
            var dtos = await _dishService.GetAllAsync();
            return Json(dtos.Select(d => new { id = d.Id, dishName = d.DishName, price = d.Price }));
        }

        // =============================================
        // 私有輔助方法
        // =============================================
        private async Task<List<SelectListItem>> GetCategoryOptionsAsync()
        {
            var categories = await _categoryService.GetAllAsync();
            return categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text  = c.CategoryName
                })
                .ToList();
        }

        private async Task<string> SaveBase64ImageAsync(string base64Data)
        {
            // 移除 data:image/jpeg;base64, 前綴
            var base64 = base64Data.Contains(",")
                ? base64Data.Split(',')[1]
                : base64Data;

            var bytes    = Convert.FromBase64String(base64);
            var fileName = $"{Guid.NewGuid()}.jpg";
            var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

            await System.IO.File.WriteAllBytesAsync(savePath, bytes);

            return "/images/" + fileName;
        }
    }
}
