namespace EatTogether.Models.ViewModels
{
    public class PreOrderListQueryViewModel
    {
        public int? Status { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? Keyword { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public List<PreOrderListItemViewModel> Orders { get; set; } = new();  // ← 用你現有的VM
    }
}
