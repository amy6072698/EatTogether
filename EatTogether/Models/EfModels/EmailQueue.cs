using System;
using System.Collections.Generic;

namespace EatTogether.Models.EfModels;

public partial class EmailQueue
{
    public int Id { get; set; }

    public string RecipientEmail { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string Body { get; set; } = null!;

    public int Status { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public DateTime CreatedAt { get; set; }
}
