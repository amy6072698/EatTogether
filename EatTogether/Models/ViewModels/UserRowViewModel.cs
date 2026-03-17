namespace EatTogether.Models.ViewModels
{
	public class UserRowViewModel
	{
		public int Id { get; set; }
		public string EmployeeNumber { get; set; } = "";
		public string Name { get; set; } = "";
		public string Account { get; set; } = "";
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public DateOnly? HireDate { get; set; }
		public DateTime CreatedAt { get; set; }
		public List<int> RoleIds { get; set; } = new();
		public List<string> RoleNames { get; set; } = new();
		public bool IsDeleted { get; set; }
		public bool IsActive { get; set; }

		// 前端顯示用
		public string StatusText => IsDeleted ? "離職" : IsActive ? "在職" : "請假";
		public string StatusColor => IsDeleted ? "gray" : IsActive ? "green" : "orange";
		public bool CanEdit { get; set; }
		public bool CanResign { get; set; }
		public bool CanReinstate { get; set; }
	}
}
