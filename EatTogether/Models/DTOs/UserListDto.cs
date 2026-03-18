namespace EatTogether.Models.DTOs
{
	public class UserListDto
	{
		public int Id { get; set; }
		public string EmployeeNumber { get; set; } = "";
		public string Name { get; set; } = "";
		public string Account { get; set; } = "";
		public string Email { get; set; } = "";
		public string Phone { get; set; } = "";
		public DateOnly HireDate { get; set; }
		public DateTime CreatedAt { get; set; }
		public bool IsActive { get; set; }
		public bool IsDeleted { get; set; }
		public List<int> RoleIds { get; set; } = new();
		public List<string> RoleNames { get; set; } = new();

		// Service 層填入，供 Extension 轉換用
		public bool CanEdit { get; set; }
		public bool CanResign { get; set; }
		public bool CanReinstate { get; set; }
	}
}
