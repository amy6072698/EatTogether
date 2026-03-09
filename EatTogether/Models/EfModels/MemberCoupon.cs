using System;
using System.Collections.Generic;

namespace EatTogether.Models.EfModels;

public partial class MemberCoupon
{
    public int Id { get; set; }

    public int MemberId { get; set; }

    public int CouponId { get; set; }

    public bool IsUsed { get; set; }

    public DateTime? UsedDate { get; set; }

    public virtual Coupon Coupon { get; set; } = null!;

    public virtual Member Member { get; set; } = null!;
}
