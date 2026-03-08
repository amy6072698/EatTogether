using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;

namespace EatTogether.Models.Extensions
{
    public static class DishDtoExtensions
    {
        // EFModels轉換成DTO
        public static DishDto ToDto(this Dish dish)
        {
            return new DishDto
            {
                Id = dish.Id,
                CategoryId = dish.CategoryId,
                CategoryName = dish.Category?.CategoryName,
                DishName = dish.DishName,
                Price = dish.Price,
                IsActive = dish.IsActive,
                Description = dish.Description,
                ImageUrl = dish.ImageUrl,
                IsTakeOut = dish.IsTakeOut,
                IsLimited = dish.IsLimited,
                StartDate = dish.StartDate,
                EndDate = dish.EndDate,
                CreatedAt = dish.CreatedAt,
                UpdatedAt = dish.UpdatedAt
            };
        }
    }
}