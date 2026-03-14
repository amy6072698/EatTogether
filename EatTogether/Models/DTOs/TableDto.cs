namespace EatTogether.Models.DTOs
{
    public class TableDto
    {
        public int Id { get; set; }
        public string TableName { get; set; } = null!;
        public int SeatCount { get; set; }
        public int Status { get; set; }
        public string? Remark { get; set; }

        // 計算屬性：狀態文字
        public string StatusText => Status switch
        {
            0 => "空桌",
            1 => "用餐中",
            2 => "保留",
            _ => "未知"
        };

        // 計算屬性：狀態 Badge CSS class
        public string StatusBadgeClass => Status switch
        {
            0 => "bg-success",
            1 => "bg-warning text-dark",
            2 => "bg-secondary",
            _ => "bg-light text-dark"
        };
    }
}
