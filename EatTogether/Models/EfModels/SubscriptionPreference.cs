using System;
using System.Collections.Generic;

namespace EatTogether.Models.EfModels;

public partial class SubscriptionPreference
{
    public int Id { get; set; }

    public int MemberId { get; set; }

    public bool IsEmailSubscribed { get; set; }

    public bool IsPushSubscribed { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Member Member { get; set; } = null!;
}
