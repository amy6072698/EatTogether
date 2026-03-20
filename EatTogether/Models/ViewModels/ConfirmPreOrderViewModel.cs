using EatTogether.Models.DTOs;

namespace EatTogether.Models.ViewModels
{
    public class ConfirmPreOrderViewModel
    {
        public int TableId { get; set; }
        public bool InOrOut { get; set; }
        public int? PeopleNum { get; set; }
        public bool IsAddOrder { get; set; }
        public string? Note { get; set; }
        public string PayMethod { get; set; }
        public int? CouponId { get; set; }
        public string? CouponCode { get; set; }
        public int DiscountAmount { get; set; }
        public int? EventId { get; set; }
        public List<CreatePreOrderItemViewModel> Items { get; set; } = new();
        public List<EventApplicableDto> ApplicableEvents { get; set; } = new();
        public int OriginalAmount => Items
                                      .Where(i => !i.ParentIndex.HasValue)
                                      .Sum(i => i.Qty * i.UnitPrice);
        public int TotalAmount => OriginalAmount - DiscountAmount;
    }
}
