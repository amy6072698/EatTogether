using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using EatTogether.Models.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Models.Repositories
{
	public class ArticleCategoryRepository : IArticleCategoryRepository
	{
		private readonly EatTogetherDBContext _context;

		public ArticleCategoryRepository(EatTogetherDBContext context)
		{
			_context = context;
		}

		public async Task CreateAsync(ArticleCategoryCreateDto dto)
		{
			var category = dto.ToEntity();

			_context.ArticleCategories.Add(category);
			await _context.SaveChangesAsync();
		}

		public async Task<List<ArticleCategoryDto>> GetAllAsync()
		{
			var data = await _context.ArticleCategories
				.AsNoTracking()
				.Select(e => new ArticleCategoryDto
				{
					//Id = e.Id,
					Name = e.Name,
					SortOrder = e.SortOrder,
					IsEnabled = e.IsEnabled					
				})
				.ToListAsync();

			return data;
		}
	}
}
