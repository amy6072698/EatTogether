using EatTogether.Models.Extensions;
using EatTogether.Models.Infra;
using EatTogether.Models.Services;
using EatTogether.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EatTogether.Controllers
{
	//[RequirePermission("Staff_Manage")]
	public class RolesController : Controller
	{
		private readonly IRoleService _roleService;

		public RolesController(IRoleService roleService)
		{
			_roleService = roleService;
		}

		//GET /Roles/Index
		// 角色列表頁
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var dtos = await _roleService.GetAllAsync();

			var vm = new RoleIndexViewModel
			{
				Rows = dtos.Select(d => d.ToRowVm())
			};

			return View(vm);
		}

		// GET /Role/Overview
		// 權限總覽 Modal（AJAX，回傳 JSON）
		[HttpGet]
		public async Task<IActionResult> Overview()
		{
			var dto = await _roleService.GetOverviewAsync();
			return Ok(dto);
		}
	}
}
