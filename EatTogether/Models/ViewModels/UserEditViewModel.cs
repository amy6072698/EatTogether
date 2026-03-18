using System.ComponentModel.DataAnnotations;

namespace EatTogether.Models.ViewModels
{
	public class UserEditViewModel
	{
		public int Id { get; set; }
		public string EmployeeNumber { get; set; } = "";
		public DateTime CreatedAt { get; set; }

		[MaxLength(50, ErrorMessage = "{0}不可超過 50 字")]
		[Display(Name = "姓名")]
		public string Name { get; set; } = "";

		[MaxLength(50, ErrorMessage = "{0}不可超過 50 字")]
		[Display(Name = "帳號")]
		public string Account { get; set; } = "";

		// 密碼選填，留空表示不修改；有填時才驗證複雜度（由 Service 層處理）
		[StringLength(60, MinimumLength = 6, ErrorMessage = "{0}長度必須介於{2}到{1}個字元")]
		[DataType(DataType.Password)]
		[Display(Name = "密碼")]
		public string? Password { get; set; }

		[MaxLength(100, ErrorMessage = "{0} 不可超過 100 字")]
		[EmailAddress(ErrorMessage = "{0} 格式不正確")]
		[Display(Name = "Email")]
		public string Email { get; set; } = "";

		[MaxLength(10, ErrorMessage = "{0}不可超過 10 碼")]
		[Display(Name = "手機號碼")]
		public string Phone { get; set; } = "";

		[Display(Name = "到職日期")]
		public DateOnly HireDate { get; set; }

		public bool IsActive { get; set; } = true;

		[MinLength(1, ErrorMessage = "請至少選擇一個{0}")]
		[Display(Name = "角色")]
		public List<int> RoleIds { get; set; } = new();
	}
}
