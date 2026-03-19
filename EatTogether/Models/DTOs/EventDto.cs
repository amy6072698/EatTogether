namespace EatTogether.Models.DTOs
{
	public class EventDto
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public string Summary { get; set; }

		public int MinSpend { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		//public string RewardItem { get; set; }
		public int? RewardDishId { get; set; }
		public string RewardDishName { get; set; }  // 顯示用，從 Dishes 帶過來

		public string DiscountType { get; set; }

		public decimal DiscountValue { get; set; }

		public int Status { get; set; }
	}
}
