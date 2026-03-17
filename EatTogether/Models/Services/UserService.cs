using EatTogether.Models.DTOs;
using EatTogether.Models.Repositories;

namespace EatTogether.Models.Services
{
	public interface IUserService
	{
		Task<IEnumerable<UserListDto>> GetAllAsync(UserSearchDto dto, int currentUserId, bool canManage);
	}

	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepo;
		private readonly IRoleRepository _roleRepo;

		public UserService(IUserRepository userRepo, IRoleRepository roleRepo)
		{
			_userRepo = userRepo;
			_roleRepo = roleRepo;
		}
		public async Task<IEnumerable<UserListDto>> GetAllAsync(UserSearchDto dto, int currentUserId, bool canManage)
		{
			var userList = await _userRepo.GetAllAsync(dto);

			foreach (var item in userList)
			{
				// 只要有管理權限 (canManage)，就可以進入編輯頁面
				item.CanEdit = canManage;

				// 判斷是否可以執行「離職」處置
				item.CanResign = canManage && !item.IsDeleted && item.Id != currentUserId;

				// 判斷是否可以執行「復職」處置
				item.CanReinstate = canManage && item.IsDeleted;
			}

			return userList;
		}
	}
}
