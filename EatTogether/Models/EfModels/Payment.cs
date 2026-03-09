using System;
using System.Collections.Generic;

namespace EatTogether.Models.EfModels;

public partial class Payment
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public int PreOrderId { get; set; }

    public string Method { get; set; } = null!;

    public DateTime PaidAt { get; set; }

    public int DoneOrCancel { get; set; }

    public virtual Order? Order { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual PreOrder PreOrder { get; set; } = null!;

    public virtual ICollection<PreOrder> PreOrders { get; set; } = new List<PreOrder>();
}
