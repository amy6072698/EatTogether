using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Models.Repositories
{
	public interface IRoleRepository
	{
		Task<List<string>> GetRoleNamesByIdsAsync(List<int> roleIds);
		Task<IEnumerable<RoleListDto>> GetAllAsync();
		Task<RoleOverviewDto> GetOverviewAsync();

	}

	public class RoleRepository : IRoleRepository
	{
		private readonly EatTogetherDBContext _context;

		public RoleRepository(EatTogetherDBContext context)
		{
			_context = context;
		}

		public async Task<List<string>> GetRoleNamesByIdsAsync(List<int> roleIds)
		{
			var roleNames = await _context.Roles
				.AsNoTracking()
				.Where(r => roleIds.Contains(r.Id))
				.Select(r => r.RoleName)
				.ToListAsync();

			return roleNames;
		}

		public async Task<IEnumerable<RoleListDto>> GetAllAsync()
		{
			var roles = await _context.Roles
				.AsNoTracking()
				.OrderBy(r => r.Id)
				.Select(r => new RoleListDto
				{
					Id = r.Id,
					RoleName = r.RoleName,
					Description = r.Description,
					FunctionIds = r.RoleFunctions
						.Select(rf => rf.FunctionId)
						.ToList(),
					FunctionDisplayNames = r.RoleFunctions
						.Select(rf => rf.Function.DisplayName)
						.ToList(),
					UserCount = r.UserRoles.Count
				})
				.ToListAsync();

			return roles;
		}

		public async Task<RoleOverviewDto> GetOverviewAsync()
		{
			// 取得所有角色（依 Id 排序，確保矩陣欄位順序固定）
			var roles = await _context.Roles
				.AsNoTracking()
				.OrderBy(r => r.Id)
				.Select(r => new
				{
					r.Id,
					r.RoleName,
					FunctionIds =
						r.RoleFunctions
						.Select(rf => rf.FunctionId)
						.ToList()
				})
				.ToListAsync();

			// 取得所有功能（依 Id 排序，確保矩陣列位順序固定）
			var functions = await _context.Functions
				.AsNoTracking()
				.OrderBy(f => f.Id)
				.Select(f => new
				{
					f.Id,
					f.DisplayName
				})
				.ToListAsync();

			var martrix = functions
				.Select(f =>
				roles.Select(r => r.FunctionIds.Contains(f.Id)).ToList())
				.ToList();

			return new RoleOverviewDto
			{
				RoleNames = roles.Select(r => r.RoleName).ToList(),
				FunctionDisplayNames = functions.Select(f => f.DisplayName).ToList(),
				Matrix = martrix
			};
		}



	}
}
