namespace EatTogether.Models.ViewModels
{
    public class TableStatusViewModel
    {
        public int TableId { get; set; }
        public string TableName { get; set; }
        public bool HasOrder { get; set; }
        public bool HasUnserved { get; set; }
        public int? PreOrderId { get; set; }
        public int TotalAmount { get; set; }
    }
}
