namespace EatTogether.Models.DTOs
{
	public class Setmealdto
	{
		// Id SetMealName DiscountType DiscountValue IsActive CreatedAt SetPrice Description ImageUrl UpdatedAt

		public int Id { get; set; }

		public string SetMealName { get; set; } = null!;
		public string DiscountType { get; set; } = null!;

		public decimal DiscountValue { get; set; }

		public bool IsActive { get; set; }

		public DateTime CreatedAt { get; set; }

		public decimal? SetPrice { get; set; }

		public string? Description { get; set; }

		public string? ImageUrl { get; set; }
		public DateOnly? StartDate { get; set; }
		public DateOnly? EndDate { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public int DisplayOrder { get; set; }

		public List<SetmealItemDto> Items { get; set; } = new ();

	}
}
