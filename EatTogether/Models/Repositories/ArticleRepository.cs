using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using EatTogether.Models.Extensions;

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
	}
}
