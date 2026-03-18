namespace EatTogether.Models.ViewModels
{
    public class ConfirmPreOrderViewModel
    {
        public int TableId { get; set; }
        public bool InOrOut { get; set; }
        public string? Note { get; set; }
        public string PayMethod { get; set; }
        public int? CouponId { get; set; }
        public string? CouponCode { get; set; }
        public int DiscountAmount { get; set; }
        public List<CreatePreOrderItemViewModel> Items { get; set; } = new();
        public int OriginalAmount => Items
                                      .Where(i => !i.ParentIndex.HasValue)
                                      .Sum(i => i.Qty * i.UnitPrice);
        public int TotalAmount => OriginalAmount - DiscountAmount;
    }
}
