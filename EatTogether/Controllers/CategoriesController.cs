using EatTogether.Models.Services;
using EatTogether.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
			
			// 準備下拉選單給 Modal 使用
			ViewBag.ParentCategoryOptions = await GetParentCategoryOptionsAsync();
			
			return View(vms);
		}

		// 用於 Modal 提交的新增
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CategoryViewModel vm)
		{
			if (!ModelState.IsValid)
			{
				var errors = ModelState.ToDictionary(
					kvp => kvp.Key,
					kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
				);
				return BadRequest(errors);
			}

			await _categoryService.CreateAsync(vm.ToDto());
			return Ok(new { message = "新增成功" });
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
	}
}
