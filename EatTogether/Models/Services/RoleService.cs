using EatTogether.Models.DTOs;
using EatTogether.Models.Repositories;

namespace EatTogether.Models.Services
{
	public interface IRoleService
	{
		Task<IEnumerable<RoleListDto>> GetAllAsync();
		Task<RoleOverviewDto> GetOverviewAsync();
	}

	public class RoleService : IRoleService
	{
		// 預設 6 個角色名稱（不可刪除）
		private static readonly HashSet<string> _defaultRoleNames = new(StringComparer.OrdinalIgnoreCase)
		{
			"店長", "副店長", "收銀員", "外場服務生", "內場廚師", "工讀生"
		};

		private readonly IRoleRepository _roleRepo;
		private readonly IFunctionRepository _functionRepo;

		public RoleService(IRoleRepository roleRepo, IFunctionRepository functionRepo)
		{
			_roleRepo = roleRepo;
			_functionRepo = functionRepo;
		}

		public async Task<IEnumerable<RoleListDto>> GetAllAsync()
		{
			return await _roleRepo.GetAllAsync();
		}

		public async Task<RoleOverviewDto> GetOverviewAsync()
		{
			return await _roleRepo.GetOverviewAsync();
		}
	}
}
