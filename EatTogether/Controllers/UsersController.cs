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
			// // �q JWT ���o�ثe�n�J�̸�T
			var currentUserId = int.Parse(User.FindFirstValue("UserId") ?? "0");

			// �ˬd�ثe�n�J�̬O�_�֦��u�޲z���u�v���v���аO (Claim)
			var canManage = User.HasClaim("Permission", "Staff_Manage");

			// �d�߱���
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
		// �w�����u�s���]�e�ݶ} Modal �ɩI�s�^
		//[RequirePermission("Staff_Manage")]
		[HttpGet]
		public async Task<IActionResult> NextEmployeeNumber()
		{
			var empNo = await _userService.GetEmployeeNumberPreviewAsync();
			return Ok(new { employeeNumber = empNo });
		}


		// POST /Users/Create
		// �s�W���u
		//[RequirePermission("Staff_Manage")]
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
		{
			if (!ModelState.IsValid)
			{
				var errors = ModelState.Values
					.SelectMany(v => v.Errors)
					.Select(e => e.ErrorMessage);
				return BadRequest(new { message = string.Join("�B", errors) });
			}

			var result = await _userService.CreateAsync(dto);

			if (result.IsSuccess) return Ok();

			return BadRequest(new { message = result.ErrorMessage });
		}

		// GET /Users/Edit/{id}
		// �e�ݶ}�s�� Modal �ɩI�s�A���o�w����
		//[RequirePermission("Staff_Manage")]
		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			var dto = await _userService.GetForEditAsync(id);
			if(dto == null)
			{
				return NotFound(new { message = "�䤣�즹���u" });
			}

			return Ok(dto.ToEditVm());
		}

		// PUT /Users/Edit/{id}
		// �x�s�s��
		//[RequirePermission("Staff_Manage")]
		[HttpPut]
		public async Task<IActionResult> Edit(int id, [FromBody] UserEditViewModel vm)
		{
			if (!ModelState.IsValid)
			{
				var errors = ModelState.Values
					.SelectMany(v => v.Errors)
					.Select(e => e.ErrorMessage);
				return BadRequest(new { message = string.Join("�B", errors) });
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
