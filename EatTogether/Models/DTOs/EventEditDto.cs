namespace EatTogether.Models.DTOs
{
	public class EventEditDto
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public string Summary { get; set; }

		public int MinSpend { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public string RewardItem { get; set; }

		public string DiscountType { get; set; }

		public decimal DiscountValue { get; set; }

		public int Status { get; set; }
	}
}
