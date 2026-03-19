using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EatTogether.Models.ViewModels
{
	public class ArticleEditViewModel
	{
		public int Id { get; set; }


		[Display(Name = "文章分類")]
		[Required(ErrorMessage = "{0}為必填")]
		public int? CategoryId { get; set; }

		[Display(Name = "關聯活動")]
		public int? EventId { get; set; }

		public string? CategoryName { get; set; }

		public string? EventName { get; set; }

		[Display(Name = "標題")]
		[Required(ErrorMessage = "{0}為必填")]
		[StringLength(200)]
		public string Title { get; set; }

		[Display(Name = "內文")]
		[Required(ErrorMessage = "{0}為必填")]
		[StringLength(4000, ErrorMessage = "內文字數不能超過4000個字")]
		public string Description { get; set; }

		[Display(Name = "上傳封面圖")]
		public IFormFile? CoverImageFile { get; set; } 
		public string? CoverImageUrl { get; set; }
		public string? ExistingCoverImageUrl { get; set; }  // 現有封面圖路徑

		[Display(Name = "上架日期")]
		[Required(ErrorMessage = "{0}為必填")]
		[DataType(DataType.Date)]
		public DateTime? PublishDate { get; set; }

		[Display(Name = "下架日期")]
		[DataType(DataType.Date)]
		public DateTime? ExpiryDate { get; set; }

		[Display(Name = "是否置頂")]
		public bool IsPinned { get; set; }

		public int Status { get; set; }

		[ValidateNever]
		public IEnumerable<SelectListItem> CategorySelectList { get; set; }

		[ValidateNever]
		public IEnumerable<SelectListItem> EventSelectList { get; set; }
	}
}
