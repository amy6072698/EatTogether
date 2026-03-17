using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using EatTogether.Models.ViewModels;

namespace EatTogether.Models.Extensions
{
	public static class ArticleMappingExtension
	{

		//新增文章 vm -> dto
		//	→ ToDto(this ArticleCreateViewModel vm)
		//	// Dto → Entity（Repository 寫入用）
		//	→Article ToEntity(this ArticleCreateDto dto)


		public static ArticleCreateDto ToCreateDto(this ArticleCreateViewModel vm)
		{
			return new ArticleCreateDto
			{
				Id = vm.Id,
				CategoryId = vm.CategoryId,
				EventId = vm.EventId,
				Title = vm.Title,
				Description	= vm.Description,
				CoverImageUrl = vm.CoverImageUrl,
				PublishDate = vm.PublishDate,
				ExpiryDate = vm.ExpiryDate,
				IsPinned = vm.IsPinned,
				Status	= vm.Status
			};
		}

		public static Article ToEntity(this ArticleCreateDto dto)
		{
			return new Article
			{
				Id = dto.Id,
				Title = dto.Title,
				Description = dto.Description,
				CoverImageUrl = dto.CoverImageUrl,
				PublishDate = dto.PublishDate,
				ExpiryDate = dto.ExpiryDate,
				IsPinned = dto.IsPinned,
				Status = dto.Status
			};
		}


	}
}
