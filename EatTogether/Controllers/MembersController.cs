using EatTogether.Models.DTOs;
using EatTogether.Models.Extensions;
using EatTogether.Models.Infra;
using EatTogether.Models.Services;
using EatTogether.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EatTogether.Controllers
{
	//[RequirePermission("Member_Manage")]
	public class MembersController : Controller
    {
		private readonly IMemberService _memberService;

		public MembersController(IMemberService memberService)
		{
			_memberService = memberService;
		}
		// GET /Members/Index
		[HttpGet]
		public async Task<IActionResult> Index(MemberSearchDto search)
		{
			var dtos = await _memberService.GetAllAsync(search);

			var vm = new MemberIndexViewModel
			{
				Rows = dtos.Select(d => d.ToRowVm()),
				Name = search.Name,
				Account = search.Account,
				Email = search.Email,
				Phone = search.Phone,
				Status = search.Status,
				SortBy = search.SortBy,
			};

			return View(vm);
		}

		// GET /Members/Detail/{id}  — JSON，供詳情 Modal AJAX 呼叫
		[HttpGet]
		public async Task<IActionResult> Detail(int id)
		{
			var dto = await _memberService.GetDetailAsync(id);
			if (dto is null)
				return NotFound(new { message = "找不到該會員。" });

			return Json(dto.ToDetailVm());
		}
		// PATCH /Member/Blacklist/{id}
		[HttpPatch]
		public async Task<IActionResult> Blacklist(int id, [FromBody] BlacklistRequest request)
		{
			var result = await _memberService.BlacklistAsync(id, request?.Reason);

			return result.IsSuccess
				? Ok(new { message = "已成功加入黑名單。" })
				: BadRequest(new { message = result.ErrorMessage });
		}

		// PATCH /Member/Unblacklist/{id}
		[HttpPatch]
		public async Task<IActionResult> Unblacklist(int id)
		{
			var result = await _memberService.UnblacklistAsync(id);

			return result.IsSuccess
				? Ok(new { message = "已成功解除黑名單。" })
				: BadRequest(new { message = result.ErrorMessage });
		}
	}

	public class BlacklistRequest
	{
		public string? Reason { get; set; }
	}
}
