namespace EatTogether.Models.ViewModels
{
    public class PaymentCheckoutViewModel
    {
        public List<int> PreOrderIds { get; set; } = new();
        public int PreOrderId { get; set; }
        public int DetailId { get; set; }
        public string OrderNumber { get; set; }
        public bool InOrOut { get; set; }
        public string TableName { get; set; }
        public string PayMethod { get; set; }
        public int OriginalAmount { get; set; }
        public int? CouponId { get; set; }
        public string? CouponName { get; set; }
        public int? EventId { get; set; }
        public string? EventTitle { get; set; }
        public int DiscountAmount { get; set; }
        public int TotalAmount { get; set; }
        public bool HasUnserved { get; set; }
        public List<PaymentDetailItemViewModel> Items { get; set; } = new();
        public List<SubOrderSummary> SubOrders { get; set; } = new();
    }

    public class SubOrderSummary
    {
        public int PreOrderId { get; set; }
        public string OrderNumber { get; set; }
        public int Amount { get; set; }      // 未結帳小計
        public bool HasUnserved { get; set; }
    }
}
