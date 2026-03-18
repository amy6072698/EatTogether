namespace EatTogether.Models.DTOs
{
	public class UserUpdateDto
	{
		public int Id { get; set; }
		public string Name { get; set; } = "";
		public string Account { get; set; } = "";
		public string? HashedPassword { get; set; }  // null = 不修改
		public string Email { get; set; } = "";
		public string Phone { get; set; } = "";
		public DateOnly HireDate { get; set; }
		public bool IsActive { get; set; }
		public bool? MustChangePassword { get; set; }  // null = 不修改
		public List<int> RoleIds { get; set; } = new();
	}
}
