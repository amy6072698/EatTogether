using System;
using System.Collections.Generic;

namespace EatTogether.Models.EfModels;

public partial class Product
{
    public int Id { get; set; }

    public string ProductType { get; set; } = null!;

    public int? DishId { get; set; }

    public int? SetMealId { get; set; }

    public virtual Dish? Dish { get; set; }

    public virtual ICollection<MemberFavorite> MemberFavorites { get; set; } = new List<MemberFavorite>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<PreOrderDetail> PreOrderDetails { get; set; } = new List<PreOrderDetail>();
}
