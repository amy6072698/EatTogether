namespace EatTogether.Models.DTOs
{
	public class UserSearchDto
	{
		public string? EmployeeNumber { get; set; }
		public string? Name { get; set; }
		public string? Account { get; set; }
		public string? Email { get; set; }
		public bool HideResigned { get; set; }
		public string SortBy { get; set; } = "HireDate_Desc"; // HireDate_Desc / HireDate_Asc / CreatedAt_Desc
	}
}
