namespace EatTogether.Models.ViewModels
{
    public class OrderDetailItemViewModel
    {
        public string ProductName { get; set; }
        public int Qty { get; set; }
        public int UnitPrice { get; set; }
        public int Subtotal => Qty * UnitPrice;
    }
}
