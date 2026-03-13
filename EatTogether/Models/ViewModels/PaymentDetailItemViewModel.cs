namespace EatTogether.Models.ViewModels
{
    public class PaymentDetailItemViewModel
    {
        public int DetailId { get; set; }
        public string ProductName { get; set; }
        public int Qty { get; set; }
        public int UnitPrice { get; set; }
        public int SubTotal { get; set; }
        public int Status { get; set; }  // 0:未完成 1:完成 2:取消
    }
}
