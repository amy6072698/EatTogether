using System;
using System.Collections.Generic;

namespace EatTogether.Models.EfModels;

public partial class Order
{
    public int Id { get; set; }

    public int PreOrderId { get; set; }

    public string OrderNumber { get; set; } = null!;

    public int? MemberId { get; set; }

    public bool InOrOut { get; set; }

    public int? TableId { get; set; }

    public int? UserId { get; set; }

    public DateTime OrderAt { get; set; }

    public int? CouponId { get; set; }

    public int OriginalAmount { get; set; }

    public int DiscountAmount { get; set; }

    public int TotalAmount { get; set; }

    public string? Note { get; set; }

    public int PaymentId { get; set; }

    public string PayMethod { get; set; } = null!;

    public virtual Coupon? Coupon { get; set; }

    public virtual Member? Member { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Payment Payment { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual PreOrder PreOrder { get; set; } = null!;

    public virtual Table? Table { get; set; }

    public virtual User? User { get; set; }
}
