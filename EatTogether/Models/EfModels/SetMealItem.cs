using System;
using System.Collections.Generic;

namespace EatTogether.Models.EfModels;

public partial class SetMealItem
{
    public int Id { get; set; }

    public int SetMealId { get; set; }

    public int DishId { get; set; }

    public int Quantity { get; set; }

    public bool IsOptional { get; set; }

    public int? OptionGroupNo { get; set; }

    public int? PickLimit { get; set; }

    public int DisplayOrder { get; set; }

    public virtual Dish Dish { get; set; } = null!;

    public virtual SetMeal SetMeal { get; set; } = null!;
}
