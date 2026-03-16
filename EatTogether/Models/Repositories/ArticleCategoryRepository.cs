using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using EatTogether.Models.Extensions;

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
	}
}
