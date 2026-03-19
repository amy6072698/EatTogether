using Microsoft.AspNetCore.Authorization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EatTogether.Models.ViewModels
{
	public class ArticleCategoryCreateViewModel	
	{
		public int Id { get; set; }

		[Display(Name = "分類名稱")]
		[Required(ErrorMessage = "{0}必填")]
		[StringLength(50)]
		public string? Name { get; set; }

		[Display(Name = "排序順序")]
		[Required(ErrorMessage = "{0}必填")]
		public int? SortOrder { get; set; }


		[Display(Name = "是否啟用")]
		public bool IsEnabled { get; set; }

	}
}
