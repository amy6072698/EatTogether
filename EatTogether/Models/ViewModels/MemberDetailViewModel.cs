namespace EatTogether.Models.ViewModels
{
	/// <summary>
	/// 詳情 Modal 所需的 ViewModel（唯讀）。
	/// 以 JSON 回傳給前端 AJAX 呼叫。
	/// </summary>
	public class MemberDetailViewModel
	{
		public int Id { get; set; }
		public string Name { get; set; } = "";
		public string Account { get; set; } = "";
		public string Email { get; set; } = "";
		public string? Phone { get; set; }
		public string? BirthDate { get; set; }         // 格式化為 yyyy-MM-dd
		public string CreatedAt { get; set; } = "";    // 格式化為 yyyy-MM-dd HH:mm:ss
		public string? DeletedAt { get; set; }         // 已刪除會員才有值
		public string StatusText { get; set; } = "";
		public string? BlacklistReason { get; set; }   // 黑名單狀態才顯示；NULL → "（未填寫）"
	}
}
