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
        public string? MemberName { get; set; }
        public string? UserName { get; set; }      // null=客人自己點
        public string? CouponName { get; set; }    // null=無優惠券
        public string? CouponDesc { get; set; }    // 折扣內容描述
        public string? EventTitle { get; set; }    // null=無活動
        public int? PeopleNum { get; set; }        // null=未填寫
        public DateTime? CompletedAt { get; set; }   // 完成/取消時間
        public string? CompletedAtLabel { get; set; } // "付款時間" or "取消時間"
    }
}
