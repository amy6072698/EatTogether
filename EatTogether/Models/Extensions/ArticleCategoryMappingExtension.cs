using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using EatTogether.Models.ViewModels;
using Humanizer;
using NuGet.Protocol.Core.Types;

namespace EatTogether.Models.Extensions
{
	public static class ArticleCategoryMappingExtension {

		//文章分類新增
		//	vm -> dto
		//	→ ToDto(this ArticleCategoryCreateViewModel vm)
		//	Dto → Entity（Repository 寫入用）
		//	→ArticleCategory ToEntity(this ArticleCategoryCreateDto dto)

		public static ArticleCategoryCreateDto ToCreateDto(this ArticleCategoryCreateViewModel vm)
		{
			return new ArticleCategoryCreateDto
			{
				//Id = vm.Id,
				Name = vm.Name,
				SortOrder = vm.SortOrder.Value,
				IsEnabled = vm.IsEnabled
			};
		}

		public static ArticleCategory ToEntity(this ArticleCategoryCreateDto dto)
		{
			return new ArticleCategory
			{
				//Id = dto.Id,
				Name = dto.Name,
				SortOrder = dto.SortOrder,
				IsEnabled = dto.IsEnabled
			};
		}

		//文章分類列表 dto -> vm
		//	→ ToViewModel(this ArticleCategoryDto dto)
		//	// Entity → Dto（Repository 讀取用）
		//	ArticleCategoryDto ToDto(this ArticleCategory entity)

		public static ArticleCategoryViewModel ToEventVm(this ArticleCategoryDto dto)
		{
			return new ArticleCategoryViewModel
			{
				Id = dto.Id,
				Name = dto.Name,
				SortOrder = dto.SortOrder,
				IsEnabled = dto.IsEnabled
			};
		}

		public static ArticleCategoryDto ToEventDto(this ArticleCategory entity)
		{
			return new ArticleCategoryDto
			{
				Id = entity.Id,
				Name = entity.Name,
				SortOrder = entity.SortOrder,
				IsEnabled = entity.IsEnabled
			};
		}



		//	文章分類編輯
		//	vm<-> dto
		//→ ToDto(this ArticleCategoryEditViewModel vm)
		//→ ToViewModel(this ArticleCategoryEditDto dto)
		//Dto → Entity（Repository 寫入用）
		//	→Article ToEntity(this ArticleCategoryEditDto dto)

		//Entity → Dto（Repository 讀取用）
		//	ArticleCategoryDto ToDto(this ArticleCategory entity)

		public static ArticleCategoryEditDto ToEditDto(this ArticleCategoryEditViewModel vm)
		{
			return new ArticleCategoryEditDto
			{
				Id = vm.Id,
				Name = vm.Name,
				SortOrder = vm.SortOrder,
				IsEnabled = vm.IsEnabled
			};
		}

		public static ArticleCategoryEditViewModel ToEditVm(this ArticleCategoryEditDto dto)
		{
			return new ArticleCategoryEditViewModel
			{
				Id = dto.Id,
				Name = dto.Name,
				SortOrder = dto.SortOrder,
				IsEnabled = dto.IsEnabled
			};
		}

		public static ArticleCategory ToEntity(this ArticleCategoryEditDto dto)
		{
			return new ArticleCategory
			{
				Id = dto.Id,
				Name = dto.Name,
				SortOrder = dto.SortOrder,
				IsEnabled = dto.IsEnabled
			};
		}

		public static ArticleCategoryEditDto ToEditDto(this ArticleCategory entity)
		{
			return new ArticleCategoryEditDto
			{
				Id = entity.Id,
				Name = entity.Name,
				SortOrder = entity.SortOrder,
				IsEnabled = entity.IsEnabled
			};
		}



	}
}
