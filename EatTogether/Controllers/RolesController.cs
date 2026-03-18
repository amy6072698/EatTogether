using EatTogether.Models.DTOs;
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

		// GET /Role/Create
		// 新增角色 Modal 所需資料（AJAX，回傳 JSON）
		[HttpGet]
		public async Task<IActionResult> Create()
		{
			var data = await _roleService.GetCreateFormDataAsync();
			return Ok(new
			{
				allFunctions = data.AllFunctions,
				allUsers = data.AllUsers
			});
		}

		// POST /Role/Create
		// 新增角色
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] RoleCreateDto dto)
		{
			if (!ModelState.IsValid)
			{
				var error = ModelState.Values
					.SelectMany(v => v.Errors)
					.Select(e => e.ErrorMessage)
					.FirstOrDefault();
				return BadRequest(new { message = error });
			}

			var result = await _roleService.CreateAsync(dto);
			if (result.IsSuccess) return Ok();
			return BadRequest(new { message = result.ErrorMessage });
		}

	}
}
