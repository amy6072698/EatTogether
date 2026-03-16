using System.ComponentModel.DataAnnotations;

namespace EatTogether.Models.ViewModels
{
	public class ArticleCategoryViewModel
	{
		public int Id { get; set; }


		[Display(Name = "分類名稱")]
		public string Name { get; set; }

		[Display(Name = "排序順序")]
		public int SortOrder { get; set; }


		[Display(Name = "是否啟用")]
		public bool IsEnabled { get; set; }
	}
}
