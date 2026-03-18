using EatTogether.Models.DTOs;
using EatTogether.Models.Extensions;
using EatTogether.Models.Infra;
using EatTogether.Models.Services;
using EatTogether.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EatTogether.Controllers
{
    public class UsersController : Controller
    {
		private readonly IUserService _userService;

		public UsersController(IUserService userService)
		{
			_userService = userService;
		}

		// GET /Users/Index
		//[RequirePermission("Staff_View")]
		[HttpGet]
		public async Task<IActionResult> Index(UserIndexViewModel vm)
		{
			// // 從 JWT 取得目前登入者資訊
			var currentUserId = int.Parse(User.FindFirstValue("UserId") ?? "0");

			// 檢查目前登入者是否擁有「管理員工」的權限標記 (Claim)
			var canManage = User.HasClaim("Permission", "Staff_Manage");

			// 查詢條件
			var searchDto = new UserSearchDto
			{
				EmployeeNumber = vm.EmployeeNumber,
				Name = vm.Name,
				Account = vm.Account,
				Email = vm.Email,
				HideResigned = vm.HideResigned,
				SortBy = string.IsNullOrEmpty(vm.SortBy) ? "HireDate_Desc" : vm.SortBy
			};

			var dtos = await _userService.GetAllAsync(searchDto, currentUserId, canManage);

			var result = new UserIndexViewModel
			{
				Rows = dtos.Select(d => d.ToRowVm()),
				EmployeeNumber = vm.EmployeeNumber,
				Name = vm.Name,
				Account = vm.Account,
				Email = vm.Email,
				HideResigned = vm.HideResigned,
				SortBy = searchDto.SortBy
			};

			return View(result);
		}

		// GET /Users/NextEmployeeNumber
		// 預產員工編號（前端開 Modal 時呼叫）
		//[RequirePermission("Staff_Manage")]
		[HttpGet]
		public async Task<IActionResult> NextEmployeeNumber()
		{
			var empNo = await _userService.GetEmployeeNumberPreviewAsync();
			return Ok(new { employeeNumber = empNo });
		}


		// POST /Users/Create
		// 新增員工
		//[RequirePermission("Staff_Manage")]
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
		{
			if (!ModelState.IsValid)
			{
				var errors = ModelState.Values
					.SelectMany(v => v.Errors)
					.Select(e => e.ErrorMessage);
				return BadRequest(new { message = string.Join("、", errors) });
			}

			var result = await _userService.CreateAsync(dto);

			if (result.IsSuccess) return Ok();

			return BadRequest(new { message = result.ErrorMessage });
		}

		// GET /Users/Edit/{id}
		// 前端開編輯 Modal 時呼叫，取得預填資料
		//[RequirePermission("Staff_Manage")]
		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			var dto = await _userService.GetForEditAsync(id);
			if(dto == null)
			{
				return NotFound(new { message = "找不到此員工" });
			}

			return Ok(dto.ToEditVm());
		}

		// PUT /Users/Edit/{id}
		// 儲存編輯
		//[RequirePermission("Staff_Manage")]
		[HttpPut]
		public async Task<IActionResult> Edit(int id, [FromBody] UserEditViewModel vm)
		{
			if (!ModelState.IsValid)
			{
				var errors = ModelState.Values
					.SelectMany(v => v.Errors)
					.Select(e => e.ErrorMessage);
				return BadRequest(new { message = string.Join("、", errors) });
			}

			var result = await _userService.UpdateAsync(id, vm);
			if (result.IsSuccess) return Ok();
			return BadRequest(new { message = result.ErrorMessage });
		}

		// PATCH /Users/Resign/{id}
		//[RequirePermission("Staff_Manage")]
		[HttpPatch]
		public async Task<IActionResult> Resign(int id)
		{
			var operatorId = int.Parse(User.FindFirstValue("UserId") ?? "0");
			var result = await _userService.ResignAsync(id, operatorId);

			if (result.IsSuccess) return Ok();
			return BadRequest(new { message = result.ErrorMessage });
		}

		// PATCH /Users/Reinstate/{id}
		//[RequirePermission("Staff_Manage")]
		[HttpPatch]
		public async Task<IActionResult> Reinstate(int id)
		{
			var result = await _userService.ReinstateAsync(id);

			if (result.IsSuccess) return Ok();
			return BadRequest(new { message = result.ErrorMessage });
		}

	}
}
