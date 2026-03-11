using System.ComponentModel.DataAnnotations;

namespace EatTogether.Models.ViewModels
{
	public class CategoryViewModel
	{
		public int Id { get; set; }


		[Required(ErrorMessage = "分類名稱為必填")]
		[StringLength(50, ErrorMessage = "分類名稱最多50個字元")]
		[Display(Name = "分類名稱")]
		public string CategoryName { get; set; } = null!;


		[Display(Name = "是否啟用")]
		public bool IsActive { get; set; } = true;


		[Display(Name = "上層分類")]
		public int? ParentCategoryId { get; set; }


		[Display(Name = "上層分類名稱")]
		public string? ParentCategoryName { get; set; }


		[Required(ErrorMessage = "顯示順序為必填")]
		[Range(1, 999, ErrorMessage = "顯示順序請填1到999之間")]
		[Display(Name = "顯示順序")]
		public int DisplayOrder { get; set; }

		[StringLength(200, ErrorMessage = "圖片網址最多200個字元")]
		[Display(Name = "圖片網址")]
		public string? ImageUrl { get; set; }

		[Display(Name = "建立時間")]
		public DateTime CreatedAt { get; set; }

		[Display(Name = "更新時間")]
		public DateTime? UpdatedAt { get; set; }
	}
}
