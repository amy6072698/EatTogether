namespace EatTogether.Models.DTOs
{
	public class JwtPayloadDto
	{
		public int UserId { get; set; }
		public List<int> RoleIds { get; set; }
		public string Name { get; set; }
		public List<string> RoleNames { get; set; }
		public List<string> FunctionNames { get; set; } = new();
	}
}
