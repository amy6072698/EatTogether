using EatTogether.Models.DTOs;
using EatTogether.Models.Extensions;
using EatTogether.Models.Infra;
using EatTogether.Models.Services;
using EatTogether.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EatTogether.Controllers
{
	[RequirePermission("Member_Manage")]
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
	}
}
