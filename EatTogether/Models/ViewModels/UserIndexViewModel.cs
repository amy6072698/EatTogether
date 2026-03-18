namespace EatTogether.Models.ViewModels
{
	public class UserIndexViewModel
	{
		public IEnumerable<UserRowViewModel> Rows { get; set; } = new List<UserRowViewModel>();
		public string? EmployeeNumber { get; set; }
		public string? Name { get; set; }
		public string? Account { get; set; }
		public string? Email { get; set; }
		public bool HideResigned { get; set; }
		public string SortBy { get; set; } = "HireDate_Desc";
	}
}
