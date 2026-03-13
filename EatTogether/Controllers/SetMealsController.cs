using EatTogether.Models.DTOs;
using EatTogether.Models.Services;
using EatTogether.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EatTogether.Controllers
{
    public class SetMealsController : Controller
    {
        private readonly SetMealService _setMealService;
        private readonly DishService _dishService;

        public SetMealsController(SetMealService setMealService, DishService dishService)
        {
            _setMealService = setMealService;
            _dishService    = dishService;
        }

        // GET: /SetMeals
        public async Task<IActionResult> Index()
        {
            var dtos = await _setMealService.GetAllAsync();
            var vms = dtos.Select(d => d.ToViewModel()).ToList();
            return View(vms);
        }

        // GET: /SetMeals/Create
        public IActionResult Create()
        {
            return View(new SetMealViewModel());
        }

        // POST: /SetMeals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] SetMealViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            if (!string.IsNullOrEmpty(vm.CroppedImageData))
                vm.ImageUrl = await SaveBase64ImageAsync(vm.CroppedImageData);

            await _setMealService.CreateAsync(vm.ToDto());
            return RedirectToAction(nameof(Index));
        }

        // GET: /SetMeals/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _setMealService.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return View(dto.ToViewModel());
        }

        // POST: /SetMeals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] SetMealViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            if (!ModelState.IsValid)
                return View(vm);

            if (!string.IsNullOrEmpty(vm.CroppedImageData))
                vm.ImageUrl = await SaveBase64ImageAsync(vm.CroppedImageData);

            await _setMealService.UpdateAsync(vm.ToDto());
            return RedirectToAction(nameof(Index));
        }

        // POST: /SetMeals/Disable/5
        [HttpPost]
        public async Task<IActionResult> Disable(int id)
        {
            await _setMealService.DisableAsync(id);
            return Ok();
        }

        // POST: /SetMeals/AddItem
        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] SetMealItemViewModel vm)
        {
            try
            {
                await _setMealService.AddItemAsync(vm.ToItemDto());
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: /SetMeals/RemoveItem/5
        [HttpPost]
        public async Task<IActionResult> RemoveItem(int id)
        {
            await _setMealService.RemoveItemAsync(id);
            return Ok();
        }

        // =============================================
        // 私有輔助方法
        // =============================================
        private async Task<string> SaveBase64ImageAsync(string base64Data)
        {
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
