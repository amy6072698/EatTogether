using System.ComponentModel.DataAnnotations;

namespace EatTogether.Models.ViewModels
{
	public class EventViewModel
	{
		public int Id { get; set; }

		[Display(Name = "標題")]
		public string Title { get; set; }

		[Display(Name = "摘要")]
		public string? Summary { get; set; }

		[Display(Name = "門檻")]
		public int MinSpend { get; set; }

		[Display(Name = "開始日期")]
		public DateTime StartDate { get; set; }

		[Display(Name = "結束日期")]
		public DateTime EndDate { get; set; }

		[Display(Name = "贈品")]
		public string? RewardItem { get; set; }

		[Display(Name = "折扣類別")]
		public string? DiscountType { get; set; }

		[Display(Name = "折扣金額")]
		public decimal DiscountValue { get; set; }

		[Display(Name = "狀態")]
		public int Status { get; set; }
	}
}
