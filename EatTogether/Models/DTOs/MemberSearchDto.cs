namespace EatTogether.Models.DTOs
{

	/// <summary>
	/// Status 可為：All / Normal / Unconfirmed / Blacklisted / Deleted
	/// SortBy  可為：CreatedAt_Desc / CreatedAt_Asc
	/// </summary>
	public class MemberSearchDto
	{
		public string? Name { get; set; }
		public string? Account { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public string Status { get; set; } = "All";
		public string SortBy { get; set; } = "CreatedAt_Desc";
	}

}
