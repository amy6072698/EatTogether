using EatTogether.Models.DTOs;

namespace EatTogether.Models.ViewModels
{
	public static class DishViewModelExtension
	{
		public static DishViewModel ToViewModel(this DishDto dto) // Dto => ViewModel
		{
			return new DishViewModel
			{
				Id = dto.Id,
				CategoryId = dto.CategoryId,
				CategoryName = dto.CategoryName,
				DishName = dto.DishName,
				Price = dto.Price,
				IsActive = dto.IsActive,
				Description = dto.Description,
				ImageUrl = dto.ImageUrl,
				IsTakeOut = dto.IsTakeOut,
				IsLimited = dto.IsLimited,
				StartDate = dto.StartDate,
				EndDate = dto.EndDate,
				CreatedAt = dto.CreatedAt,
				UpdatedAt = dto.UpdatedAt
			};
		}
		public static DishDto ToDto(this DishViewModel vm) // ViewModel => Dto
		{
			return new DishDto
			{
				Id = vm.Id,
				CategoryId = vm.CategoryId,
				DishName = vm.DishName,
				Price = vm.Price,
				IsActive = vm.IsActive,
				Description = vm.Description,
				ImageUrl = vm.ImageUrl,
				IsTakeOut = vm.IsTakeOut,
				IsLimited = vm.IsLimited,
				StartDate = vm.StartDate,
				EndDate = vm.EndDate
			};
		}
	}
}
