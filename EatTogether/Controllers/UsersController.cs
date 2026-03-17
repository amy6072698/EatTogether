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
		[HttpGet]
		//[RequirePermission("Staff_View")]
		public async Task<IActionResult> Index(UserIndexViewModel vm)
		{
			// // 從 JWT 取得目前登入者資訊
			var currentUserId = int.Parse(User.FindFirstValue("UserId") ?? "0");

			// 檢查目前登入者是否擁有「管理員工」的權限標記 (Claim)
			var canMange = User.HasClaim("Premission", "Staff_Manage");

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

			var dtos = await _userService.GetAllAsync(searchDto, currentUserId, canMange);

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

	}
}
