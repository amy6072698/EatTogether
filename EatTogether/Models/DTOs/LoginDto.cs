namespace EatTogether.Models.DTOs
{
	public class LoginDto
	{
		public int UserId { get; set; }
		public string Account { get; set; } = "";
		public string Name { get; set; } = "";
		public List<int> RoleIds { get; set; } = new();
		public List<string> RoleNames { get; set; } = new();
		public List<string> FunctionNames { get; set; } = new();
		public bool MustChangePassword { get; set; }
	}
}
