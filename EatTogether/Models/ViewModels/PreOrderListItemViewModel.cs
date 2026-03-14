namespace EatTogether.Models.ViewModels
{
    public class PreOrderListItemViewModel
    {
        public int PreOrderId { get; set; }
        public string OrderNumber { get; set; }
        public string TableName { get; set; }
        public bool InOrOut { get; set; }
        public string? Note { get; set; }
        public int PendingCount { get; set; }
        public List<PreOrderDetailItemViewModel> Items { get; set; } = new();
        public DateTime OrderAt { get; set; }
        public int OriginalAmount { get; set; }
        public int DiscountAmount { get; set; }
        public int TotalAmount { get; set; }
        public int DoneOrCancel { get; set; }
        public string PayMethod { get; set; }
        public string? MemberName { get; set; }  // null = 訪客
        public string? UserName { get; set; }      // ← 補上，null=客人自己點
        public string? CouponName { get; set; }    // ← 補上，null=無優惠券
        public string? CouponDesc { get; set; }    // ← 補上，折扣內容描述
    }
}
