using EatTogether.Models.DTOs;

namespace EatTogether.Models.Repositories
{
	public interface IArticleRepository {
		Task CreateAsync(ArticleCreateDto dto);
		Task<List<ArticleDto>> GetAllAsync();
	}
}
