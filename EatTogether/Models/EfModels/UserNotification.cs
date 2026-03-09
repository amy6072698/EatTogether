using System;
using System.Collections.Generic;

namespace EatTogether.Models.EfModels;

public partial class UserNotification
{
    public int Id { get; set; }

    public int MemberId { get; set; }

    public int ArticleId { get; set; }

    public string Title { get; set; } = null!;

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Article Article { get; set; } = null!;

    public virtual Member Member { get; set; } = null!;
}
