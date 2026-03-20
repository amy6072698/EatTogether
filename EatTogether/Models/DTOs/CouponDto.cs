namespace EatTogether.Models.DTOs
{
    public class CouponDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public int DiscountType { get; set; }      // 0=折金額, 1=打折%
        public int DiscountValue { get; set; }
        public int MinSpend { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? LimitCount { get; set; }
        public int ReceivedCount { get; set; }
        public bool IsDisabled { get; set; }

        /// <summary>此優惠券是否已套用在目前訂單中</summary>
        public bool IsInUse { get; set; }

        // 計算屬性
        public string DiscountTypeText => DiscountType == 0 ? "折金額" : "打折";

        public string DiscountDescription => DiscountType == 0
            ? $"折 ${DiscountValue}"
            : $"打 {100 - DiscountValue} 折";

        public bool IsExpired => EndDate.HasValue && EndDate.Value < DateTime.Now;
        public bool IsUpcoming => StartDate > DateTime.Now;
        public bool IsLimitHit => LimitCount.HasValue && ReceivedCount >= LimitCount.Value;

        public string StatusText
        {
            get
            {
                if (IsDisabled) return "已停用";
                if (IsUpcoming) return "尚未開始";
                if (IsExpired) return "已過期";
                if (IsLimitHit) return "已達限量";
                return "有效";
            }
        }

        public string StatusBadgeClass
        {
            get
            {
                if (IsDisabled) return "bg-dark";
                if (IsUpcoming) return "bg-secondary";
                if (IsExpired) return "bg-danger";
                if (IsLimitHit) return "bg-warning text-dark";
                return "bg-success";
            }
        }
    }
}
