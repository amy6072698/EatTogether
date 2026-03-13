namespace EatTogether.Models.DTOs
{
    public class CreatePreOrderDto
    {
        public int TableId { get; set; }
        public bool InOrOut { get; set; }
        public string PayMethod { get; set; }
        public string? Note { get; set; }
        public int DiscountAmount { get; set; }
        public List<PreOrderDetailDto> Items { get; set; } = new();
    }
}
