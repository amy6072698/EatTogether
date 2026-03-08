using System;
using System.Collections.Generic;

namespace EatTogether.Models.EfModels;

public partial class ArticleCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int SortOrder { get; set; }

    public bool IsEnabled { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
