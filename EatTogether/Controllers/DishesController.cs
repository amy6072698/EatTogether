using EatTogether.Models.Services;
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
			_dishService = dishService;
			_categoryService = categoryService;
		}

		public async Task<IActionResult> Index()
		{
			var dtos = await _dishService.GetAllAsync();
			var vms = dtos.Select(d => d.ToViewModel()).ToList();
			return View(vms);
		}

		public async Task<IActionResult> Create()
		{
			var vm = new DishViewModel();
			vm.CategoryOptions = await GetCategoryOptionsAsync();
			return View(vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(DishViewModel vm)
		{
			if (!ModelState.IsValid)
			{
				vm.CategoryOptions = await GetCategoryOptionsAsync();
				return View(vm);
			}
			await _dishService.CreateAsync(vm.ToDto());
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Edit(int id)
		{
			var dto = await _dishService.GetByIdAsync(id);
			if (dto == null) return NotFound();

			var vm = dto.ToViewModel();
			vm.CategoryOptions = await GetCategoryOptionsAsync();
			return View(vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, DishViewModel vm)
		{
			if (id != vm.Id) return BadRequest();
			if (!ModelState.IsValid)
			{
				vm.CategoryOptions = await GetCategoryOptionsAsync();
				return View(vm);
			}
			await _dishService.UpdateAsync(vm.ToDto());
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		private async Task<List<SelectListItem>> GetCategoryOptionsAsync(int? selectedId = null)
		{
			var categories = await _categoryService.GetAllAsync();

			return categories
				.Select(c => new SelectListItem
				{
					Value = c.Id.ToString(),
					Text = c.CategoryName,
				})
				.ToList();
		}
	}
}
