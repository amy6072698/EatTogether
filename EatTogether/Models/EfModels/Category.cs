using System;
using System.Collections.Generic;

namespace EatTogether.Models.EfModels;

public partial class Category
{
    public int Id { get; set; }

    public string CategoryName { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? ParentCategoryId { get; set; }

    public int DisplayOrder { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Dish> Dishes { get; set; } = new List<Dish>();

    public virtual ICollection<Category> InverseParentCategory { get; set; } = new List<Category>();

    public virtual Category? ParentCategory { get; set; }
}
