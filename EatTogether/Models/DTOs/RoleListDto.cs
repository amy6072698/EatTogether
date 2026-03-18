namespace EatTogether.Models.DTOs
{
	/// <summary>
	/// 角色列表頁用 DTO（含權限顯示名稱清單與員工數）
	/// </summary>
	public class RoleListDto
	{
		public int Id { get; set; }
		public string RoleName { get; set; } = "";
		public string? Description { get; set; }
		public List<int> FunctionIds { get; set; } = new();
		public List<string> FunctionDisplayNames { get; set; } = new();
		public int UserCount { get; set; }
	}
}
