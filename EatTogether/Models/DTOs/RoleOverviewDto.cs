namespace EatTogether.Models.DTOs
{
	/// <summary>
	/// 權限總覽 Modal 用 DTO（矩陣：列 = 功能，欄 = 角色）
	/// </summary>
	public class RoleOverviewDto
	{
		public List<string> RoleNames { get; set; } = new();
		public List<string> FunctionDisplayNames { get; set; } = new();
		/// <summary>Matrix[functionIndex][roleIndex] = true 表示該角色擁有此功能</summary>
		public List<List<bool>> Matrix { get; set; } = new();
	}
}
