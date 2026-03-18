using EatTogether.Models.DTOs;
using EatTogether.Models.ViewModels;

namespace EatTogether.Models.Extensions
{
	public static class RoleDtoExtension
	{
		// 預設 6 個角色名稱（不可刪除）
		private static readonly HashSet<string> _defaultRoleNames = new(StringComparer.OrdinalIgnoreCase)
		{
			"店長", "副店長", "收銀員", "外場服務生", "內場廚師", "工讀生"
		};

		/// <summary>RoleListDto → RoleRowViewModel（供列表頁渲染）</summary>
		public static RoleRowViewModel ToRowVm(this RoleListDto dto)
		{
			return new RoleRowViewModel
			{
				Id = dto.Id,
				RoleName = dto.RoleName,
				Description = dto.Description,
				FunctionDisplayNames = dto.FunctionDisplayNames,
				UserCount = dto.UserCount,
				CanDelete = !_defaultRoleNames.Contains(dto.RoleName)
			};
		}

		/// <summary>
		/// 組裝新增角色 ViewModel（帶入所有 Functions 與所有在職員工）
		/// </summary>
		public static RoleCreateViewModel ToCreateVm(
			IEnumerable<FunctionDto> allFunctions,
			IEnumerable<UserForRoleDto> allUsers)
		{
			return new RoleCreateViewModel
			{
				AllFunctions = allFunctions,
				AllUsers = allUsers
			};
		}

	}
}
