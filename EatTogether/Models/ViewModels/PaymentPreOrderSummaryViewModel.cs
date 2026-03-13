namespace EatTogether.Models.ViewModels
{
    // 左側桌號/外帶清單用
    public class PaymentPreOrderSummaryViewModel
    {
        public int PreOrderId { get; set; }
        public string OrderNumber { get; set; }
        public bool InOrOut { get; set; }
        public string TableName { get; set; }
        public DateTime OrderAt { get; set; }
        public int TotalAmount { get; set; }
        public bool HasUnserved { get; set; }
    }
}
