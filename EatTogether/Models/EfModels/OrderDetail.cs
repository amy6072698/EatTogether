using System;
using System.Collections.Generic;

namespace EatTogether.Models.EfModels;

public partial class OrderDetail
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public int UnitPrice { get; set; }

    public int Qty { get; set; }

    public int SubTotal { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
