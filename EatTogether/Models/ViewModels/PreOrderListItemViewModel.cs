namespace EatTogether.Models.ViewModels
{
    public class PreOrderListItemViewModel
    {
        public int PreOrderId { get; set; }
        public string OrderNumber { get; set; }
        public string TableName { get; set; }    // 來自 Table.TableName
        public bool InOrOut { get; set; }
        public string? Note { get; set; }
        public int PendingCount { get; set; }
        public List<PreOrderDetailItemViewModel> Items { get; set; } = new();
        public DateTime OrderAt { get; set; }
        public int TotalAmount { get; set; }
        public int DoneOrCancel { get; set; }    // 0:待處理 1:完成 2:取消
    }
}
