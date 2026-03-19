using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using EatTogether.Models.ViewModels;
using Humanizer;

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
				CategoryId = vm.CategoryId.GetValueOrDefault(),
				EventId = vm.EventId,
				Title = vm.Title,
				Description	= vm.Description,
				CoverImageUrl = vm.CoverImageUrl,
				PublishDate = vm.PublishDate.GetValueOrDefault(),
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
				CategoryId = dto.CategoryId,
				EventId = dto.EventId,
				Title = dto.Title,
				Description = dto.Description,
				CoverImageUrl = dto.CoverImageUrl,
				PublishDate = dto.PublishDate,
				ExpiryDate = dto.ExpiryDate,
				IsPinned = dto.IsPinned,
				Status = dto.Status
			};
		}



		//modify ArticleMappingExtension class
		//	文章列表 dto -> vm
		//	→ ToViewModel(this ArticleDto dto)
		//	// Entity → Dto（Repository 讀取用）
		//	ArticleDto ToDto(this Article entity)


		public static ArticleViewModel ToArticleVm(this ArticleDto dto)
		{
			return new ArticleViewModel
			{
				Id = dto.Id,
				CategoryId = dto.CategoryId,
				EventId = dto.EventId,
				CategoryName = dto.CategoryName,
				EventName = dto.EventName,
				Title = dto.Title,
				Description = dto.Description,
				CoverImageUrl = dto.CoverImageUrl,
				PublishDate = dto.PublishDate,
				ExpiryDate = dto.ExpiryDate,
				IsPinned = dto.IsPinned,
				Status = dto.Status

			};
		}

		public static ArticleDto ToArticleDto(this Article entity)
		{
			return new ArticleDto
			{
				Id = entity.Id,
				CategoryId = entity.CategoryId,
				EventId = entity.EventId,
				CategoryName = entity.Category?.Name,
				EventName = entity.Event?.Title,
				Title = entity.Title,
				Description = entity.Description,
				CoverImageUrl = entity.CoverImageUrl,
				PublishDate = entity.PublishDate,
				ExpiryDate = entity.ExpiryDate,
				IsPinned = entity.IsPinned,
				Status = entity.Status
			};
		}


		//modify ArticleMappingExtension class
		//	文章預覽 dto -> vm
		//	→ ToViewModel(this ArticleDetailsDto dto)
		//	// Entity → Dto（Repository 讀取用）
		//	ArticleDetailsDto ToDto(this Article entity)

		//編輯文章 vm<-> dto
 		//	→ ToDto(this ArticleEditViewModel vm)
		//	→ ToViewModel(this ArticleEditDto dto)
		//	// Dto → Entity（Repository 寫入用）
		//	→Article ToEntity(this ArticleEditDto dto)

		//	// Entity → Dto（Repository 讀取用）
		//	ArticleDto ToDto(this Article entity)

		public static ArticleEditViewModel ToArticleEditVm(this ArticleEditDto dto)
		{
			return new ArticleEditViewModel
			{
				Id = dto.Id,
				CategoryId = dto.CategoryId,
				EventId = dto.EventId,
				CategoryName = dto.CategoryName,
				EventName = dto.EventName,
				Title = dto.Title,
				Description = dto.Description,
				CoverImageUrl = dto.CoverImageUrl,
				PublishDate = dto.PublishDate,
				ExpiryDate = dto.ExpiryDate,
				IsPinned = dto.IsPinned,
				Status = dto.Status

			};
		}

		public static ArticleEditDto ToEditDto(this ArticleEditViewModel vm)
		{
			return new ArticleEditDto
			{
				Id = vm.Id,
				CategoryId = vm.CategoryId.GetValueOrDefault(),
				EventId = vm.EventId,
				CategoryName = vm.CategoryName,
				EventName = vm.EventName,
				Title = vm.Title,
				Description = vm.Description,
				CoverImageUrl = vm.CoverImageUrl,
				PublishDate = vm.PublishDate.GetValueOrDefault(),
				ExpiryDate = vm.ExpiryDate,
				IsPinned = vm.IsPinned,
				Status = vm.Status
			};
		}


		public static Article ToEntity(this ArticleEditDto dto)
		{
			return new Article
			{
				Id = dto.Id,
				CategoryId = dto.CategoryId.GetValueOrDefault(),
				EventId = dto.EventId,
				Title = dto.Title,
				Description = dto.Description,
				CoverImageUrl = dto.CoverImageUrl,
				PublishDate = dto.PublishDate,
				ExpiryDate = dto.ExpiryDate,
				IsPinned = dto.IsPinned,
				Status = dto.Status
			};
		}


		public static ArticleEditDto ToEditDto(this Article entity)
		{
			return new ArticleEditDto
			{
				Id = entity.Id,
				CategoryId = entity.CategoryId,
				EventId = entity.EventId,
				CategoryName = entity.Category?.Name,
				EventName = entity.Event?.Title,
				Title = entity.Title,
				Description = entity.Description,
				CoverImageUrl = entity.CoverImageUrl,
				PublishDate = entity.PublishDate,
				ExpiryDate = entity.ExpiryDate,
				IsPinned = entity.IsPinned,
				Status = entity.Status
			};
		}


	}
}
