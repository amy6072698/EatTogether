using EatTogether.Models.DTOs;

namespace EatTogether.Models.ViewModels
{
	/// <summary>
	/// 新增角色 Modal 的 ViewModel（含全部 Functions 與全部在職員工，供 Razor 渲染 Checkbox）
	/// </summary>
	public class RoleCreateViewModel
	{
		// --- 使用者填寫的欄位 ---
		public string RoleName { get; set; } = "";
		public string? Description { get; set; }
		public List<int> SelectedFunctionIds { get; set; } = new();
		public List<int> SelectedUserIds { get; set; } = new();

		// --- 供 Razor 渲染 Checkbox 卡片 ---
		public IEnumerable<FunctionDto> AllFunctions { get; set; } = new List<FunctionDto>();
		public IEnumerable<UserForRoleDto> AllUsers { get; set; } = new List<UserForRoleDto>();
	}

	/// <summary>
	/// 編輯角色 Modal 的 ViewModel（同 RoleCreateViewModel，多一個 Id）
	/// </summary>
	public class RoleEditViewModel
	{
		// --- 識別 ---
		public int Id { get; set; }

		// --- 使用者填寫的欄位 ---
		public string RoleName { get; set; } = "";
		public string? Description { get; set; }
		public List<int> SelectedFunctionIds { get; set; } = new();
		public List<int> SelectedUserIds { get; set; } = new();

		// --- 供 Razor 渲染 Checkbox 卡片 ---
		public IEnumerable<FunctionDto> AllFunctions { get; set; } = new List<FunctionDto>();
		public IEnumerable<UserForRoleDto> AllUsers { get; set; } = new List<UserForRoleDto>();
	}
}
