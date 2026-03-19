namespace EatTogether.Models.ViewModels
{
	
	// 列表每一列所需的顯示欄位
	public class MemberRowViewModel
	{
		public int Id { get; set; }
		public string Name { get; set; } = "";
		public string Account { get; set; } = "";
		public string Email { get; set; } = "";
		public string? Phone { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? DeletedAt { get; set; }
		public string? BlacklistReason { get; set; }

		/// <summary>啟用中 / 未驗證 / 黑名單 / 已刪除</summary>
		public string StatusText { get; set; } = "";

		/// <summary>green / yellow / red / gray（對應 CSS class）</summary>
		public string StatusColor { get; set; } = "";

		/// <summary>blacklist / unblacklist / disabled</summary>
		public string ButtonType { get; set; } = "";
	}

	// 傳給 Index View 的完整 ViewModel
	public class MemberIndexViewModel
	{
		public IEnumerable<MemberRowViewModel> Rows { get; set; } = [];

		// 搜尋條件（回填搜尋列用）
		public string? Name { get; set; }
		public string? Account { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public string Status { get; set; } = "All";
		public string SortBy { get; set; } = "CreatedAt_Desc";
	}
}
