using EatTogether.Models.DTOs;

namespace EatTogether.Models.Repositories
{
	public interface IArticleCategoryRepository
	{
		Task CreateAsync(ArticleCategoryCreateDto dto);

	}
}
