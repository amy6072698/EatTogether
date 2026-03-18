namespace EatTogether.Models.DTOs
{
	public class UserInsertDto
	{
		public string EmployeeNumber { get; set; } = "";
		public string Name { get; set; } = "";
		public string Account { get; set; } = "";
		public string HashedPassword { get; set; } = "";
		public string Email { get; set; } = "";
		public string Phone { get; set; } = "";
		public DateOnly HireDate { get; set; }
		public bool IsActive { get; set; }
		public bool MustChangePassword { get; set; }
		public List<int> RoleIds { get; set; } = new();
	}
}
