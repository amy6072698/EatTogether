using EatTogether.Models.DTOs;
using System.Linq;

namespace EatTogether.Models.ViewModels
{
    public static class SetMealViewModelExtension
    {
        // Dto → ViewModel
        public static SetMealViewModel ToViewModel(this Setmealdto dto)
        {
            return new SetMealViewModel
            {
                Id            = dto.Id,
                SetMealName   = dto.SetMealName,
                DiscountType  = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                IsActive      = dto.IsActive,
                CreatedAt     = dto.CreatedAt,
                SetPrice      = dto.SetPrice,
                Description   = dto.Description,
                ImageUrl      = dto.ImageUrl,
                StartDate     = dto.StartDate,
                EndDate       = dto.EndDate,
                StartTime     = dto.StartTime,
                EndTime       = dto.EndTime,
                UpdatedAt     = dto.UpdatedAt,
                DisplayOrder  = dto.DisplayOrder,
                IsPopular     = dto.IsPopular,
                IsRecommended = dto.IsRecommended,
                Items         = dto.Items.Select(i => i.ToItemViewModel()).ToList()
            };
        }

        // ViewModel → Dto
        public static Setmealdto ToDto(this SetMealViewModel vm)
        {
            return new Setmealdto
            {
                Id            = vm.Id,
                SetMealName   = vm.SetMealName,
                DiscountType  = vm.DiscountType,
                DiscountValue = vm.DiscountValue,
                IsActive      = vm.IsActive,
                SetPrice      = vm.SetPrice,
                Description   = vm.Description,
                ImageUrl      = vm.ImageUrl,
                StartDate     = vm.StartDate,
                EndDate       = vm.EndDate,
                StartTime     = vm.StartTime,
                EndTime       = vm.EndTime,
                DisplayOrder  = vm.DisplayOrder,
                IsPopular     = vm.IsPopular,
                IsRecommended = vm.IsRecommended,
                Items         = vm.Items.Select(i => i.ToItemDto()).ToList()
            };
        }

        // ItemDto → ItemViewModel
        public static SetMealItemViewModel ToItemViewModel(this SetmealItemDto dto)
        {
            return new SetMealItemViewModel
            {
                Id            = dto.Id,
                SetMealId     = dto.SetMealId,
                DishId        = dto.DishId,
                DishName      = dto.DishName,
                DishPrice     = dto.DishPrice,
                CategoryName  = dto.CategoryName,
                Quantity      = dto.Quantity,
                IsOptional    = dto.IsOptional,
                OptionGroupNo = dto.OptionGroupNo,
                PickLimit     = dto.PickLimit,
                DisplayOrder  = dto.DisplayOrder
            };
        }

        // ItemViewModel → ItemDto
        public static SetmealItemDto ToItemDto(this SetMealItemViewModel vm)
        {
            return new SetmealItemDto
            {
                Id            = vm.Id,
                SetMealId     = vm.SetMealId,
                DishId        = vm.DishId,
                Quantity      = vm.Quantity,
                IsOptional    = vm.IsOptional,
                OptionGroupNo = vm.OptionGroupNo,
                PickLimit     = vm.PickLimit,
                DisplayOrder  = vm.DisplayOrder
            };
        }
    }
}
