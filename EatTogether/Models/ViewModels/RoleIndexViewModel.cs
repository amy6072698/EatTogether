namespace EatTogether.Models.ViewModels
{
	public class RoleIndexViewModel
	{
		public IEnumerable<RoleRowViewModel> Rows { get; set; } = new List<RoleRowViewModel>();
	}
	public class RoleRowViewModel
	{
		public int Id { get; set; }
		public string RoleName { get; set; } = "";
		public string? Description { get; set; }
		/// <summary>綠色權限標籤清單（FunctionDisplayName）</summary>
		public List<string> FunctionDisplayNames { get; set; } = new();
		public int UserCount { get; set; }
		/// <summary>預設 6 個角色 = false，不可刪除</summary>
		public bool CanDelete { get; set; }
	}
}
