using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Models.Repositories
{
	public interface IUserRepository
	{
		Task<IEnumerable<UserListDto>> GetAllAsync(UserSearchDto dto);
		Task<UserDto?> GetByAccountAsync(string account);
		Task<UserDto?> GetByEmailAsync(string email);
		Task<UserDto?> GetByIdAsync(int userId);
		Task SetMustChangePasswordAsync(int userId, bool value);
		Task UpdatePasswordAsync(int userId, string hashedPassword);
	}

	public class UserRepository : IUserRepository
	{
		private readonly EatTogetherDBContext _context;

		public UserRepository(EatTogetherDBContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<UserListDto>> GetAllAsync(UserSearchDto dto)
		{
			var query = _context.Users.AsQueryable(); // 尚未執行的查詢


			// 加上篩選條件
			if (!string.IsNullOrWhiteSpace(dto.EmployeeNumber))
			{
				query = query.Where(u => u.EmployeeNumber.Contains(dto.EmployeeNumber));
			}

			if (!string.IsNullOrWhiteSpace(dto.Name))
			{
				query = query.Where(u => u.Name.Contains(dto.Name));
			}

			if (!string.IsNullOrWhiteSpace(dto.Account))
			{
				query = query.Where(u => u.Account.Contains(dto.Account));
			}

			if (!string.IsNullOrWhiteSpace(dto.Email))
			{
				query = query.Where(u => u.Email.Contains(dto.Email));
			}

			if (dto.HideResigned)
			{
				query = query.Where(u => !u.IsDeleted);
			}


			// 排序
			query = dto.SortBy switch
			{
				"HireDate_Asc" => query.OrderBy(u => u.HireDate),
				"CreatedAt_Desc" => query.OrderByDescending(u => u.CreatedAt),
				_ => query.OrderByDescending(u => u.HireDate) // HireDate_Desc（預設）
			};

			var queryResult = await query
				.Select(u => new UserListDto
				{
					Id = u.Id,
					EmployeeNumber = u.EmployeeNumber,
					Name = u.Name,
					Account = u.Account,
					Email = u.Email,
					Phone = u.Phone,
					HireDate = u.HireDate,
					CreatedAt = u.CreatedAt,
					IsActive = u.IsActive,
					IsDeleted = u.IsDeleted,
					RoleIds = u.UserRoles.Select(ur => ur.RoleId).ToList(),
					RoleNames = u.UserRoles.Select(ur => ur.Role.RoleName).ToList()
				})
				.ToListAsync();

			return queryResult;
		}

		public async Task<UserDto?> GetByAccountAsync(string account)
		{
			var user = await _context.Users
				.AsNoTracking()
				.Where(u => u.Account == account)
				.Select(u => new UserDto
				{
					Id = u.Id,
					Account = u.Account,
					HashedPassword = u.HashedPassword,
					Name = u.Name,
					IsActive = u.IsActive,
					IsDeleted = u.IsDeleted,
					MustChangePassword = u.MustChangePassword,
					RoleIds = u.UserRoles.Select(ur => ur.RoleId).ToList()
				})
				.FirstOrDefaultAsync();

			return user;
		}

		public async Task<UserDto?> GetByIdAsync(int userId)
		{
			var user = await _context.Users
				.AsNoTracking()
				.Where(u => u.Id == userId)
				.Select(u => new UserDto
				{
					Id = u.Id,
					Account = u.Account,
					HashedPassword = u.HashedPassword,
					Name = u.Name,
					IsActive = u.IsActive,
					IsDeleted = u.IsDeleted,
					MustChangePassword = u.MustChangePassword,
					RoleIds = u.UserRoles.Select(ur => ur.RoleId).ToList()
				})
				.FirstOrDefaultAsync();

			return user;
		}

		public async Task<UserDto?> GetByEmailAsync(string email)
		{
			var user = await _context.Users
				.AsNoTracking()
				.Where(u => u.Email == email)
				.Select(u => new UserDto
				{
					Id = u.Id,
					Account = u.Account,
					HashedPassword = u.HashedPassword,
					Name = u.Name,
					IsActive = u.IsActive,
					IsDeleted = u.IsDeleted,
					MustChangePassword = u.MustChangePassword,
					RoleIds = u.UserRoles.Select(ur => ur.RoleId).ToList()
				})
				.FirstOrDefaultAsync();

			return user;
		}

		public async Task UpdatePasswordAsync(int userId, string hashedPassword)
		{
			var user = await _context.Users.FindAsync(userId);
			if (user == null) return;

			user.HashedPassword = hashedPassword;
			await _context.SaveChangesAsync();
		}

		public async Task SetMustChangePasswordAsync(int userId, bool value)
		{
			var user = await _context.Users.FindAsync(userId);
			if (user == null) return;

			user.MustChangePassword = value;
			await _context.SaveChangesAsync();
		}
	}
}
