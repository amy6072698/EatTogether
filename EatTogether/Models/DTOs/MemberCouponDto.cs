namespace EatTogether.Models.DTOs
{
    public class MemberCouponDto
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string MemberName { get; set; } = null!;
        public string MemberAccount { get; set; } = null!;
        public int CouponId { get; set; }
        public string CouponName { get; set; } = null!;
        public string Code { get; set; } = null!;
        public int DiscountType { get; set; }
        public int DiscountValue { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsUsed { get; set; }
        public DateTime? UsedDate { get; set; }

        public string DiscountDescription => DiscountType == 0
            ? $"折 ${DiscountValue}"
            : $"打 {100 - DiscountValue} 折";

        public bool IsExpired => EndDate.HasValue && EndDate.Value < DateTime.Now;

        public string StatusText
        {
            get
            {
                if (IsUsed) return "已使用";
                if (IsExpired) return "未使用（已過期）";
                return "未使用";
            }
        }

        public string StatusBadgeClass
        {
            get
            {
                if (IsUsed) return "bg-secondary";
                if (IsExpired) return "bg-warning text-dark";
                return "bg-success";
            }
        }
    }
}
