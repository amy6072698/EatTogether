using System.ComponentModel.DataAnnotations;

namespace EatTogether.Models.ViewModels
{
	public class EventEditViewModel
	{
		public int Id { get; set; }

		[Display(Name = "標題")]
		[Required(ErrorMessage = "{0}必填")]
		[StringLength(100)]
		public string Title { get; set; }

		[Display(Name = "摘要")]
		[Required(ErrorMessage = "{0}必填")]
		[StringLength(50)]
		public string Summary { get; set; }

		[Display(Name = "門檻")]
		[Required(ErrorMessage = "{0}必填")]
		[Range(0.0000001, int.MaxValue, ErrorMessage = "{0}必須大於零")]
		public int MinSpend { get; set; }

		[Display(Name = "開始日期")]
		[Required(ErrorMessage = "{0}必填")]
		[DataType(DataType.Date)]
		public DateTime StartDate { get; set; }

		private DateTime _endDate;
		[Display(Name = "結束日期")]
		[Required(ErrorMessage = "{0}必填")]
		[DataType(DataType.Date)]
		public DateTime EndDate
		{
			get => _endDate;
			set => _endDate = value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
		}


		[Display(Name = "贈品")]
		[StringLength(100)]
		public string? RewardItem { get; set; }

		[Display(Name = "折扣類別")]
		[StringLength(20)]
		public string? DiscountType { get; set; }

		[Display(Name = "折扣金額")]
		[Required(ErrorMessage = "{0}必填")]
		public decimal DiscountValue { get; set; }

		[Display(Name = "狀態")]
		public int Status { get; set; }


		// 後端驗證：開始日期不能是過去
		public IEnumerable<ValidationResult> Validate(ValidationContext context)
		{
			if (StartDate.Date < DateTime.Today)
				yield return new ValidationResult("開始日期不能早於今天", new[] { nameof(StartDate) });

			if (EndDate.Date < StartDate.Date)
				yield return new ValidationResult("結束日期不能早於開始日期", new[] { nameof(EndDate) });
		}
	}
}
