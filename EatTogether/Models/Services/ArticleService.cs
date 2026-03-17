using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using EatTogether.Models.Repositories;

namespace EatTogether.Models.Services
{
	public class ArticleService
	{
		private readonly IArticleRepository _repo;
		private readonly EatTogetherDBContext _context;

		public ArticleService(IArticleRepository repo, EatTogetherDBContext context)
		{
			_repo = repo;
			_context = context;
		}

		public async Task CreateAsync(ArticleCreateDto dto)
		{

			await _repo.CreateAsync(dto);

		}
	}
}
