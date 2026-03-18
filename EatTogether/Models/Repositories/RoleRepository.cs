using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Models.Repositories
{
	public interface IRoleRepository
	{
		Task<List<string>> GetRoleNamesByIdsAsync(List<int> roleIds);
		Task<IEnumerable<RoleListDto>> GetAllAsync();

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




	}
}
