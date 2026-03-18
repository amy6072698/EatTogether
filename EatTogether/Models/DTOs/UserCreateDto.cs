using System.ComponentModel.DataAnnotations;

namespace EatTogether.Models.DTOs
{
	public class UserCreateDto
	{
		[Required(ErrorMessage = "{0}為必填")]
		[MaxLength(50, ErrorMessage = "{0}不可超過 50 字")]
		[Display(Name = "姓名")]
		public string Name { get; set; } = "";

		[Required(ErrorMessage = "{0}為必填")]
		[MaxLength(50, ErrorMessage = "{0}不可超過 50 字")]
		[Display(Name = "帳號")]
		public string Account { get; set; } = "";

		[Required(ErrorMessage = "{0}為必填")]
		[StringLength(60, MinimumLength = 6, ErrorMessage = "{0}長度必須介於{2}到{1}個字元")]
		[DataType(DataType.Password)]
		[Display(Name = "密碼")]
		public string Password { get; set; } = "";  // 明文，供 Service 驗證與 Hash

		[Required(ErrorMessage = "{0} 為必填")]
		[MaxLength(100, ErrorMessage = "{0} 不可超過 100 字")]
		[EmailAddress(ErrorMessage = "{0} 格式不正確")]
		[Display(Name = "Email")]
		public string Email { get; set; } = "";

		[Required(ErrorMessage = "{0}為必填")]
		[MaxLength(10, ErrorMessage = "{0}不可超過 10 碼")]
		[Display(Name = "手機號碼")]
		public string Phone { get; set; } = "";

		[Required(ErrorMessage = "{0}為必填")]
		[Display(Name = "到職日期")]
		public DateOnly HireDate { get; set; }

		public bool IsActive { get; set; } = true;

		[MinLength(1, ErrorMessage = "請至少選擇一個{0}")]
		[Display(Name = "角色")]
		public List<int> RoleIds { get; set; } = new();
	}
}
