using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EatTogether.Models.ViewModels
{
	public class ArticleDetailsViewModel
	{
		public int Id { get; set; }


		[Display(Name = "文章分類")]
		public int? CategoryId { get; set; }

		[Display(Name = "關聯活動")]
		public int? EventId { get; set; }

		public string? CategoryName { get; set; }

		public string? EventName { get; set; }

		[Display(Name = "標題")]
		[StringLength(200)]
		public string Title { get; set; }

		[Display(Name = "內文")]
		public string Description { get; set; }

		public string? CoverImageUrl { get; set; }

		[Display(Name = "上架日期")]
		[DataType(DataType.Date)]
		public DateTime? PublishDate { get; set; }

		[Display(Name = "下架日期")]
		[DataType(DataType.Date)]
		public DateTime? ExpiryDate { get; set; }

		[Display(Name = "是否置頂")]
		public bool IsPinned { get; set; }

		public int Status { get; set; }
	}
}
