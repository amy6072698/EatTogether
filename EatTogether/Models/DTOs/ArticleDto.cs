using System.ComponentModel.DataAnnotations;

namespace EatTogether.Models.DTOs
{
	public class ArticleDto
	{

		public int Id { get; set; }

		public int? CategoryId { get; set; }

		public int? EventId { get; set; }

		public string? CategoryName { get; set; }

		public string? EventName { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public string? CoverImageUrl { get; set; }

		public DateTime? PublishDate { get; set; }

		public DateTime? ExpiryDate { get; set; }

		public bool IsPinned { get; set; }

		public int Status { get; set; }
	}
}
