namespace EatTogether.Models.DTOs
{
	/// <summary>
	/// 包含 MemberListDto 全部欄位 + AvatarFileName，
	/// 供詳情 Modal AJAX 使用。
	/// </summary>
	public class MemberDetailDto : MemberListDto
	{
		public string? AvatarFileName { get; set; }
	}
}
