using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using EatTogether.Models.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Models.Repositories
{
	public class ArticleRepository : IArticleRepository
	{
		private readonly EatTogetherDBContext _context;

		public ArticleRepository(EatTogetherDBContext context)
		{
			_context = context;
		}


		public async Task CreateAsync(ArticleCreateDto dto)
		{
			var article = dto.ToEntity();

			_context.Articles.Add(article);
			await _context.SaveChangesAsync();
		}


		public async Task<List<ArticleDto>> GetAllAsync()
		{
			var data = await _context.Articles
				.AsNoTracking()
				.Select(e => new ArticleDto
				{
					Id = e.Id,
					CategoryId = e.CategoryId,
					EventId = e.EventId,
					Title = e.Title,
					Description = e.Description,
					CoverImageUrl = e.CoverImageUrl,
					PublishDate = e.PublishDate,
					ExpiryDate = e.ExpiryDate,
					IsPinned = e.IsPinned,
					Status = e.Status,
					CategoryName = e.Category.Name,  // 直接存取導覽屬性
					EventName = e.Event != null ? e.Event.Title : null
				})
				.ToListAsync();

			return data;
		}

		public async Task<ArticleEditDto> GetEditByIdAsync(int id)
		{
			var entity = await _context.Articles.FindAsync(id);

			if (entity == null) return null;

			return new ArticleEditDto
			{
				Id = id,
				CategoryId = entity.CategoryId,
				EventId = entity.EventId,
				Title = entity.Title,
				Description = entity.Description,
				CoverImageUrl = entity.CoverImageUrl,
				PublishDate = entity.PublishDate,
				ExpiryDate = entity.ExpiryDate,
				IsPinned = entity.IsPinned,
				Status = entity.Status,
				CategoryName = entity.Category.Name,
				EventName = entity.Event.Title
			};

		}

		public async Task EditAsync(ArticleEditDto dto)
		{
			var entity = await _context.Articles.FindAsync(dto.Id);
			if (entity == null)
			{
				return;
			}

			entity.Id = dto.Id;
			entity.CategoryId = dto.CategoryId.GetValueOrDefault();
			entity.EventId = dto.EventId;
			entity.Title = dto.Title;
			entity.Description = dto.Description;
			entity.CoverImageUrl = dto.CoverImageUrl;
			entity.PublishDate = dto.PublishDate;


			await _context.SaveChangesAsync();
		}
	}
}
