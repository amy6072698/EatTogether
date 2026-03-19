namespace EatTogether.Models.DTOs
{
	public class MemberListDto
	{
		public int Id { get; set; }
		public string Name { get; set; } = "";
		public string Account { get; set; } = "";
		public string Email { get; set; } = "";
		public string? Phone { get; set; }
		public DateOnly? BirthDate { get; set; }
		public DateTime CreatedAt { get; set; }
		public bool IsConfirmed { get; set; }
		public bool IsBlacklisted { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime? DeletedAt { get; set; }
		public string? BlacklistReason { get; set; }
	}
}
