using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;

namespace EatTogether.Models.Extensions
{
    public static class CategoryDtoExtensions
    {
        // EFModles轉換成DTO
        public static CategoryDto ToDto(this Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                IsActive = category.IsActive,
                ParentCategoryId = category.ParentCategoryId,
                ParentCategoryName = category.ParentCategory?.CategoryName,
                DisplayOrder = category.DisplayOrder,
                ImageUrl = category.ImageUrl,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
        }
    }
}
