using System;
using System.Collections.Generic;

namespace EatTogether.Models.EfModels;

public partial class Event
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Summary { get; set; }

    public int MinSpend { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string? RewardItem { get; set; }

    public string? DiscountType { get; set; }

    public decimal DiscountValue { get; set; }

    public int Status { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
