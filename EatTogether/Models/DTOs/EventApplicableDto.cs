namespace EatTogether.Models.DTOs
{
    /// <summary>
    /// 符合條件（活動期間內、金額已達門檻）的活動 DTO
    /// </summary>
    public class EventApplicableDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string DiscountType { get; set; }   // FixedAmount | Percent | Gift
        public decimal DiscountValue { get; set; }
        public string? RewardItem { get; set; }
        public int MinSpend { get; set; }

        /// <summary>折抵金額（Gift 類型為 0）</summary>
        public int CalculatedDiscount { get; set; }

        /// <summary>給前端顯示的折扣描述，例如「折抵 NT$ 100」或「贈送：招牌甜點」</summary>
        public string DiscountDescription { get; set; }
    }
}
