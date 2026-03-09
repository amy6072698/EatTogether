using System;
using System.Collections.Generic;

namespace EatTogether.Models.EfModels;

public partial class Dish
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string DishName { get; set; } = null!;

    public decimal Price { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsTakeOut { get; set; }

    public bool IsLimited { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<SetMealItem> SetMealItems { get; set; } = new List<SetMealItem>();
}
