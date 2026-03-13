namespace EatTogether.Models.ViewModels
{
    public class PaymentIndexViewModel
    {
        public List<TableStatusViewModel> Tables { get; set; } = new();
        public List<PaymentPreOrderSummaryViewModel> TakeoutOrders { get; set; } = new();
    }
}
