namespace EatTogether.Models.DTOs
{
    public class PreOrderDetailDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
