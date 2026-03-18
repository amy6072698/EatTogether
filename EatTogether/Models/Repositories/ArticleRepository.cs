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

	}
}
