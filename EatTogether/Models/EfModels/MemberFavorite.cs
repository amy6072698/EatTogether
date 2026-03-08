using System;
using System.Collections.Generic;

namespace EatTogether.Models.EfModels;

public partial class MemberFavorite
{
    public int Id { get; set; }

    public int MemberId { get; set; }

    public int ProductId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Member Member { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
