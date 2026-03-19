using EatTogether.Models.EfModels;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EatTogether.Models.ViewModels
{
	public class ArticleCreateViewModel : IValidatableObject
	{
		public int Id { get; set; }


		[Display(Name = "文章分類")]
		[Required(ErrorMessage ="{0}為必填")]
		public int? CategoryId { get; set; }

		[Display(Name = "關聯活動")]
		public int? EventId { get; set; }

		[Display(Name = "標題")]
		[Required(ErrorMessage = "{0}為必填")]
		[StringLength(200)]
		public string Title { get; set; }

		[Display(Name = "內文")]
		[Required(ErrorMessage = "{0}為必填")]
		[StringLength(4000, ErrorMessage = "內文字數不能超過4000個字")]
		public string Description { get; set; }

		[Display(Name = "上傳封面圖")]
		public IFormFile? CoverImageFile { get; set; } // 接收實體檔案

		// 用來存儲「存檔後的路徑」，寫入資料庫用
		public string? CoverImageUrl { get; set; }

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

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{			
			// 額外檢查：如果有點選下架日期，下架日期不能早於上架日期
			if (PublishDate.HasValue && ExpiryDate.HasValue && ExpiryDate < PublishDate)
			{
				yield return new ValidationResult("下架日期不能早於發佈日期", new[] { nameof(ExpiryDate) });
			}			
		}
	}
}
