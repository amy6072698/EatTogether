using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;

namespace EatTogether.Models.Extensions
{
	public static class SetmealdtoExtension
	{
		public static Setmealdto ToDo(this SetMeal setMeal)
		{
			return new Setmealdto
			{
				Id = setMeal.Id,
				SetMealName = setMeal.SetMealName,
				DiscountType = setMeal.DiscountType,
				DiscountValue = setMeal.DiscountValue,
				IsActive = setMeal.IsActive,
				CreatedAt = setMeal.CreatedAt,
				SetPrice = setMeal.SetPrice,
				Description = setMeal.Description,
				ImageUrl = setMeal.ImageUrl,
				UpdatedAt = setMeal.UpdatedAt,
				Items = setMeal.SetMealItems.Select(i => i.ToItemDto()).ToList()
			};
		}
			
			public static SetmealItemDto ToItemDto(this SetMealItem item)
		{
			return new SetmealItemDto
			{
				Id = item.Id,
				SetMealId = item.SetMealId,
				DishId = item.DishId,
				DishName = item.Dish?.DishName,
				DishPrice = item.Dish?.Price,
				Quantity = item.Quantity,
				IsOptional = item.IsOptional,
				OptionGroupNo = item.OptionGroupNo,
				PickLimit = item.PickLimit,
				DisplayOrder = item.DisplayOrder
			};
		}
	}
}		
