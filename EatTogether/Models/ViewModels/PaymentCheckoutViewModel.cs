namespace EatTogether.Models.ViewModels
{
    public class PaymentCheckoutViewModel
    {
        public int PreOrderId { get; set; }
        public string OrderNumber { get; set; }
        public bool InOrOut { get; set; }
        public string TableName { get; set; }
        public string PayMethod { get; set; }
        public int OriginalAmount { get; set; }
        public int DiscountAmount { get; set; }
        public int TotalAmount { get; set; }
        public bool HasUnserved { get; set; }
        public List<PaymentDetailItemViewModel> Items { get; set; } = new();
    }
}
