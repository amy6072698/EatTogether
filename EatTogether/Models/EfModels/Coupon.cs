using System;
using System.Collections.Generic;

namespace EatTogether.Models.EfModels;

public partial class Coupon
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public int DiscountType { get; set; }

    public int DiscountValue { get; set; }

    public int MinSpend { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? LimitCount { get; set; }

    public int? ReceivedCount { get; set; }

    public virtual ICollection<MemberCoupon> MemberCoupons { get; set; } = new List<MemberCoupon>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<PreOrder> PreOrders { get; set; } = new List<PreOrder>();
}
