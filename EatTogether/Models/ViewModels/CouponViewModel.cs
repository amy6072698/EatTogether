using System.ComponentModel.DataAnnotations;

namespace EatTogether.Models.ViewModels
{
    public class CouponCreateViewModel
    {
        [Required(ErrorMessage = "活動名稱為必填")]
        [StringLength(50, ErrorMessage = "活動名稱最多 50 個字元")]
        [Display(Name = "活動名稱")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "折扣碼為必填")]
        [StringLength(20, ErrorMessage = "折扣碼最多 20 個字元")]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "折扣碼只允許英文大寫與數字")]
        [Display(Name = "折扣碼")]
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = "折扣類型為必填")]
        [Display(Name = "折扣類型")]
        public int DiscountType { get; set; } = 0;   // 0=折金額, 1=打折%

        [Required(ErrorMessage = "折扣數值為必填")]
        [Range(1, 99999, ErrorMessage = "折扣數值必須大於 0")]
        [Display(Name = "折扣數值")]
        public int DiscountValue { get; set; }

        [Range(0, 999999, ErrorMessage = "最低消費不可為負數")]
        [Display(Name = "最低消費門檻")]
        public int MinSpend { get; set; } = 0;

        [Required(ErrorMessage = "活動開始日為必填")]
        [Display(Name = "活動開始日")]
        public DateTime StartDate { get; set; }

        [Display(Name = "活動結束日")]
        public DateTime? EndDate { get; set; }

        [Range(1, 999999, ErrorMessage = "限量張數必須大於 0")]
        [Display(Name = "限量張數")]
        public int? LimitCount { get; set; }
    }
    public class CouponEditViewModel
    {
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "活動名稱為必填")]
        [System.ComponentModel.DataAnnotations.StringLength(50)]
        [System.ComponentModel.DataAnnotations.Display(Name = "活動名稱")]
        public string Name { get; set; } = null!;

        // 唯讀顯示欄位
        public string Code { get; set; } = null!;
        public string DiscountDescription { get; set; } = null!;
        public int ReceivedCount { get; set; }
        public int? LimitCount { get; set; }
        public string StatusText { get; set; } = null!;
        public string StatusBadgeClass { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [System.ComponentModel.DataAnnotations.Range(1, 999999, ErrorMessage = "請輸入大於 0 的數字")]
        [System.ComponentModel.DataAnnotations.Display(Name = "增加限量張數")]
        public int? AddLimitCount { get; set; }
    }
}