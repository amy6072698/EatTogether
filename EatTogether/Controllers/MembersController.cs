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
				Search = search,
			};

			return View(vm);
		}

		// GET /Members/Detail/{id}  �X JSON�A�ѸԱ� Modal AJAX �I�s
		[HttpGet]
		public async Task<IActionResult> Detail(int id)
		{
			var dto = await _memberService.GetDetailAsync(id);
			if (dto is null)
				return NotFound(new { message = "�䤣��ӷ|���C" });

			return Json(dto.ToDetailVm());
		}
		// PATCH /Member/Blacklist/{id}
		[HttpPatch]
		public async Task<IActionResult> Blacklist(int id, [FromBody] BlacklistRequest request)
		{
			var result = await _memberService.BlacklistAsync(id, request?.Reason);

			return result.IsSuccess
				? Ok(new { message = "�w���\�[�J�¦W��C" })
				: BadRequest(new { message = result.ErrorMessage });
		}

		// PATCH /Member/Unblacklist/{id}
		[HttpPatch]
		public async Task<IActionResult> Unblacklist(int id)
		{
			var result = await _memberService.UnblacklistAsync(id);

			return result.IsSuccess
				? Ok(new { message = "�w���\�Ѱ��¦W��C" })
				: BadRequest(new { message = result.ErrorMessage });
		}
	}

	public class BlacklistRequest
	{
		public string? Reason { get; set; }
	}
}
