using EatTogether.Models.Services;
using EatTogether.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EatTogether.Controllers
{
	public class CategoriesController : Controller
	{
		private readonly CategoryService _categoryService;

		public CategoriesController(CategoryService categoryService)
		{
			_categoryService = categoryService;
		}

		// GET: Categories
		public async Task<IActionResult> Index()
		{
			var dtos = await _categoryService.GetAllAsync();
			var vms = dtos.Select(d => d.ToViewModel()).ToList();
			return View(vms);
		}

		public async Task<IActionResult> Create()
		{
			var vm = new CategoryViewModel();
			vm.ParentCategoryOptions = await GetParentCategoryOptionsAsync();
			return View(vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task <IActionResult> Create(CategoryViewModel vm)
		{
			if (!ModelState.IsValid)
			{
				vm.ParentCategoryOptions = await GetParentCategoryOptionsAsync();
				return View(vm);
			}
			await _categoryService.CreateAsync(vm.ToDto());
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Edit(int id)
		{
			var dto = await _categoryService.GetByIdAsync(id);
			if (dto == null) return NotFound();

			var vm = dto.ToViewModel();
			vm.ParentCategoryOptions = await GetParentCategoryOptionsAsync(id);
			return View(vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, CategoryViewModel vm)
		{
			if (id != vm.Id) return BadRequest();
			if (!ModelState.IsValid)
			{
				vm.ParentCategoryOptions = await GetParentCategoryOptionsAsync(id);
				return View(vm);
			}

			await _categoryService.UpdateAsync(vm.ToDto());
			return RedirectToAction(nameof(Index));
		}

		private async Task<List<SelectListItem>> GetParentCategoryOptionsAsync(int excludeId = 0)
		{
			var allCategories = await _categoryService.GetAllAsync();

			var options = allCategories
				.Where(c => c.Id != excludeId) // 排除自己
				.Select(c => new SelectListItem
				{
					Value = c.Id.ToString(),
					Text = c.CategoryName
				})
				.ToList();

			options.Insert(0, new SelectListItem { Value = "", Text = "（無，設為頂層分類）" }); 

			return options;
		}
	}
}
