using System.ComponentModel.DataAnnotations;

namespace EatTogether.Models.DTOs
{
	/// <summary>
	/// 編輯角色時前後端共用的資料（GET 回傳預填值 / PUT 接收更新值）
	/// </summary>
	public class RoleEditDto
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "{0}為必填")]
		[StringLength(20, ErrorMessage = "{0}不可超過 {1} 字")]
		[Display(Name = "角色名稱")]
		public string RoleName { get; set; } = "";

		[StringLength(100, ErrorMessage = "{0}不可超過 {1} 字")]
		[Display(Name = "角色描述")]
		public string? Description { get; set; }
		public List<int> FunctionIds { get; set; } = new();
		public List<int> UserIds { get; set; } = new();
	}
}
