using EatTogether.Models.DTOs;

namespace EatTogether.Models.Repositories
{

	public interface IArticleCategoryRepository
	{
		Task CreateAsync(ArticleCategoryCreateDto dto);
		Task<List<ArticleCategoryDto>> GetAllAsync();
		Task EditAsync(ArticleCategoryEditDto dto);
		Task<ArticleCategoryEditDto> GetEditByIdAsync(int id);

	}
}
