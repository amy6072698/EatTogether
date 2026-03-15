using System.ComponentModel.DataAnnotations;

namespace EatTogether.Models.ViewModels
{
    public class TableCreateViewModel
    {
        [Required(ErrorMessage = "桌位名稱為必填")]
        [StringLength(20, ErrorMessage = "桌位名稱最多 20 個字元")]
        [Display(Name = "桌位名稱")]
        public string TableName { get; set; } = null!;

        [Required(ErrorMessage = "座位數為必填")]
        [Range(1, 99, ErrorMessage = "座位數必須大於 0")]
        [Display(Name = "座位數")]
        public int SeatCount { get; set; } = 2;
    }

    public class TableUpdateStatusViewModel
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public string? Remark { get; set; }
    }

    public class TableEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "桌位名稱為必填")]
        [StringLength(20, ErrorMessage = "桌位名稱最多 20 個字元")]
        [Display(Name = "桌位名稱")]
        public string TableName { get; set; } = null!;

        [Required(ErrorMessage = "座位數為必填")]
        [Range(1, 99, ErrorMessage = "座位數必須大於 0")]
        [Display(Name = "座位數")]
        public int SeatCount { get; set; }

        [System.ComponentModel.DataAnnotations.StringLength(200)]
        public string? Remark { get; set; }

        // 唯讀顯示用，不允許在 Edit 頁修改
        public int Status { get; set; }
        public string StatusText => Status switch
        {
            0 => "空桌",
            1 => "用餐中",
            2 => "保留",
            _ => "未知"
        };
    }
}
