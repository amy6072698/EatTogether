using System.ComponentModel.DataAnnotations;

namespace EatTogether.Models.DTOs
{
	/// <summary>
	/// 新增角色時前端送來的資料
	/// </summary>
	public class RoleCreateDto
	{
		[Required(ErrorMessage = "{0}為必填")]
		[StringLength(20, ErrorMessage = "{0}不可超過 {1} 字")]
		[Display(Name = "角色名稱")]
		public string RoleName { get; set; } = "";

		[StringLength(100, ErrorMessage = "{0}不可超過 {1} 字")]
		[Display(Name = "角色描述")]
		public string? Description { get; set; }

		/// <summary>勾選的 FunctionId 清單（後端會過濾掉 IsOwnerOnly=1 的兩項）</summary>
		public List<int> FunctionIds { get; set; } = new();

		/// <summary>要指派此角色的員工 Id 清單</summary>
		public List<int> UserIds { get; set; } = new();
	}
}
