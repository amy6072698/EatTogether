namespace EatTogether.Models.DTOs
{
	/// <summary>
	/// 角色 Modal「擁有此角色的員工」區塊，顯示在職員工的精簡資料
	/// </summary>
	public class UserForRoleDto
	{
		public int Id { get; set; }
		public string Name { get; set; } = "";
		public string EmployeeNumber { get; set; } = "";
		public string Account { get; set; } = "";
		/// <summary>true = 在職, false = 請假（IsDeleted=0 的員工才會出現在此清單）</summary>
		public bool IsActive { get; set; }
	}
}
