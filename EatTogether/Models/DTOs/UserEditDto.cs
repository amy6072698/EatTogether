namespace EatTogether.Models.DTOs
{
	public class UserEditDto
	{
		public int Id { get; set; }
		public string EmployeeNumber { get; set; } = "";  // 唯讀
		public DateTime CreatedAt { get; set; }           // 唯讀
		public string Name { get; set; } = "";
		public string Account { get; set; } = "";
		public string? Password { get; set; }             // 留空 = 不修改
		public string Email { get; set; } = "";
		public string Phone { get; set; } = "";
		public DateOnly HireDate { get; set; }
		public bool IsActive { get; set; }
		public List<int> RoleIds { get; set; } = new();
	}
}
