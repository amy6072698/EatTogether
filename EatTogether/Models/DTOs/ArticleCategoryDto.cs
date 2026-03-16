using System.ComponentModel.DataAnnotations;

namespace EatTogether.Models.DTOs
{
	public class ArticleCategoryDto
	{
		//public int Id { get; set; }

		public string Name { get; set; }

		public int SortOrder { get; set; }

		public bool IsEnabled { get; set; }

	}
}
