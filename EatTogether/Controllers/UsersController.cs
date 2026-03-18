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

		/* --------------------------------------------------------
           預產員工編號（前端開 Modal 時呼叫）
        -------------------------------------------------------- */
		// GET /Users/NextEmployeeNumber
		//[RequirePermission("Staff_Manage")]
		[HttpGet]
		public async Task<IActionResult> NextEmployeeNumber()
		{
			var empNo = await _userService.GetEmployeeNumberPreviewAsync();
			return Ok(new { employeeNumber = empNo });
		}

		/* --------------------------------------------------------
           新增員工
        -------------------------------------------------------- */
		// POST /Users/Create
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
	}
}
