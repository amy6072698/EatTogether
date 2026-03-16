using EatTogether.Models.DTOs;
using EatTogether.Models.Repositories;

namespace EatTogether.Models.Services
{
	public class ArticleCategoryService
	{
		private readonly IArticleCategoryRepository _repo;

		public ArticleCategoryService(IArticleCategoryRepository repo)
		{
			_repo = repo;
		}

		public async Task CreateAsync(ArticleCategoryCreateDto dto) {
		
				await _repo.CreateAsync(dto);

		}

	}
}
