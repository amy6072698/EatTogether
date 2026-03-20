namespace EatTogether.Models.ViewModels
{
    public class PaymentDetailItemViewModel
    {
        public int DetailId { get; set; }
        public int PreOrderId { get; set; }
        public string ProductName { get; set; }
        public int Qty { get; set; }
        public int UnitPrice { get; set; }
        public int SubTotal { get; set; }
        public int Status { get; set; }  // 0:未完成 1:完成 2:取消
        public bool IsBilled { get; set; }
        public bool IsSetMeal { get; set; }
        public int? ParentDetailId { get; set; }
        public bool IsInvalidGift { get; set; }  // 贈品已出餐但活動門檻不再符合
    }
}
