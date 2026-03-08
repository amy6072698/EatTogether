using System;
using System.Collections.Generic;

namespace EatTogether.Models.EfModels;

public partial class Article
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public int? EventId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? CoverImageUrl { get; set; }

    public DateTime PublishDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public bool IsPinned { get; set; }

    public int Status { get; set; }

    public virtual ArticleCategory Category { get; set; } = null!;

    public virtual Event? Event { get; set; }

    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
}
