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
	}
}
