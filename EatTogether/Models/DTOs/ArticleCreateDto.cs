using EatTogether.Models.EfModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EatTogether.Models.DTOs
{
	public class ArticleCreateDto
	{
		public int Id { get; set; }

		public int CategoryId { get; set; }

		public int? EventId { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public string CoverImageUrl { get; set; }
				
		public DateTime PublishDate { get; set; }

		public DateTime? ExpiryDate { get; set; }

		public bool IsPinned { get; set; }

		public int Status { get; set; }

		public virtual ArticleCategory Category { get; set; }

		public virtual Event Event { get; set; }
	}
}
