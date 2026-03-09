using System;
using System.Collections.Generic;

namespace EatTogether.Models.EfModels;

public partial class Table
{
    public int Id { get; set; }

    public string TableName { get; set; } = null!;

    public int SeatCount { get; set; }

    public int Status { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<PreOrder> PreOrders { get; set; } = new List<PreOrder>();
}
