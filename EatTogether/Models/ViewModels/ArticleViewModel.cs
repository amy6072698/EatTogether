using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EatTogether.Models.ViewModels
{
	public class ArticleViewModel
	{

		public int Id { get; set; }


		[Display(Name = "文章分類")]
		public int? CategoryId { get; set; }

		[Display(Name = "關聯活動")]
		public int? EventId { get; set; }

		public string? CategoryName { get; set; }  

		public string? EventName { get; set; }   


		[Display(Name = "標題")]
		public string Title { get; set; }

		[Display(Name = "內文")]
		public string Description { get; set; }

		public string? CoverImageUrl { get; set; }

		[Display(Name = "上架日期")]
		[DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
		public DateTime? PublishDate { get; set; }

		[Display(Name = "下架日期")]
		[DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
		[DataType(DataType.Date)]
		public DateTime? ExpiryDate { get; set; }

		[Display(Name = "是否置頂")]
		public bool IsPinned { get; set; }

		public int Status { get; set; }

		public string StatusLabel => Status switch
		{
			0 => "草稿",
			1 => PublishDate.HasValue && PublishDate.Value.Date > DateTime.Today ? "待上架" : "已發佈",
			2 => "已下架",
			_ => "未知"
		};


		[ValidateNever]
		public IEnumerable<SelectListItem> CategorySelectList { get; set; }

		[ValidateNever]
		public IEnumerable<SelectListItem> EventSelectList { get; set; }
	}
}
