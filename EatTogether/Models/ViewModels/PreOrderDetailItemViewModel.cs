namespace EatTogether.Models.ViewModels
{
    public class PreOrderDetailItemViewModel
    {
        public int PreOrderId { get; set; }
        public string ProductName { get; set; }
        public int Qty { get; set; }
        public int UnitPrice { get; set; }
        public int Subtotal => Qty * UnitPrice;
        public int DetailId { get; set; }
        public int Status { get; set; }  // 0=待處理 1=完成 2=取消
    }
}