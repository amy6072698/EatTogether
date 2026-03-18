using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Models.Repositories
{
	public interface IRoleRepository
	{
		Task CreateAsync(RoleCreateDto dto);
		Task<IEnumerable<UserForRoleDto>> GetActiveUsersAsync();
		Task<IEnumerable<RoleListDto>> GetAllAsync();
		Task<RoleOverviewDto> GetOverviewAsync();
		Task<List<string>> GetRoleNamesByIdsAsync(List<int> roleIds);
		Task<bool> IsNameDuplicateAsync(string roleName, int? excludeId = null);
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

		// 角色 Modal 員工清單（在職員工，不含離職）
		public async Task<IEnumerable<UserForRoleDto>> GetActiveUsersAsync()
		{
			return await _context.Users
				.AsNoTracking()
				.Where(u => !u.IsDeleted)           // 只顯示未離職員工
				.OrderBy(u => u.Name)
				.Select(u => new UserForRoleDto
				{
					Id = u.Id,
					Name = u.Name,
					EmployeeNumber = u.EmployeeNumber,
					Account = u.Account,
					IsActive = u.IsActive
				})
				.ToListAsync();
		}

		// 角色名稱唯一性檢查
		public async Task<bool> IsNameDuplicateAsync(string roleName, int? excludeId = null)
		{
			var query = _context.Roles.Where(r => r.RoleName == roleName);
			if (excludeId.HasValue)
				query = query.Where(r => r.Id != excludeId.Value);
			return await query.AnyAsync();
		}

		public async Task CreateAsync(RoleCreateDto dto)
		{
			using var transaction = await _context.Database.BeginTransactionAsync();

			try
			{
				var role = new Role
				{
					RoleName = dto.RoleName.Trim(),
					Description = dto.Description?.Trim(),
					CreatedAt = DateTime.Now
				};

				_context.Roles.Add(role);
				await _context.SaveChangesAsync(); // 先取得 role.Id

				// 批次寫入 RoleFunctions
				_context.RoleFunctions.AddRange(
					dto.FunctionIds.Select(fid => new RoleFunction
					{
						RoleId = role.Id,
						FunctionId = fid
					})
				);

				// 批次寫入 UserRoles
				_context.UserRoles.AddRange(
					dto.UserIds.Select(uid => new UserRole
					{
						RoleId = role.Id,
						UserId = uid
					})
				);

				await _context.SaveChangesAsync();
				await transaction.CommitAsync();
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}





	}
}
