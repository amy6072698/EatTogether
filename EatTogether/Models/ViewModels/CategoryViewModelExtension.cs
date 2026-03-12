using EatTogether.Models.DTOs;

namespace EatTogether.Models.ViewModels
{
	public static class CategoryViewModelExtension
	{
		public static CategoryViewModel ToViewModel(this CategoryDto dto) // Dto => ViewModel
		{
			return new CategoryViewModel
			{
				Id = dto.Id,
				CategoryName = dto.CategoryName,
				IsActive = dto.IsActive,
				ParentCategoryId = dto.ParentCategoryId,
				ParentCategoryName = dto.ParentCategoryName,
				DisplayOrder = dto.DisplayOrder,
				ImageUrl = dto.ImageUrl,
				CreatedAt = dto.CreatedAt,
				UpdatedAt = dto.UpdatedAt
			};
		}

		public static CategoryDto ToDto(this CategoryViewModel vm) // ViewModel => Dto
		{
			return new CategoryDto
			{
				Id = vm.Id,
				CategoryName = vm.CategoryName,
				IsActive = vm.IsActive,
				ParentCategoryId = vm.ParentCategoryId,
				DisplayOrder = vm.DisplayOrder,
				ImageUrl = vm.ImageUrl

			};
		}
	}
}
