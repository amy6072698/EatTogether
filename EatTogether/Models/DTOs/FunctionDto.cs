namespace EatTogether.Models.DTOs
{ 
	/// <summary>
    /// 功能權限 DTO（對應 Functions 資料表，供新增/編輯角色 Modal 的 Checkbox 卡片使用）
    /// </summary>
	public class FunctionDto
	{
		public int Id { get; set; }
		public string Category { get; set; } = "";
		public string FunctionName { get; set; } = "";
		public string DisplayName { get; set; } = "";
		public string? Description { get; set; }
		/// <summary>true = 僅限店長，新增/編輯角色 Modal 灰底不可勾選</summary>
		public bool IsOwnerOnly { get; set; }
	}
}
