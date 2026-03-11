using EatTogether.Models.DTOs;
using EatTogether.Models.Repositories;

namespace EatTogether.Models.Services
{
	public class CategoryService
	{
		private readonly ICategoryRepository _repo;

		public CategoryService(ICategoryRepository repo)
		{
			_repo = repo;
		}

		public async Task<IEnumerable<CategoryDto>> GetAllAsync()
		{
			return await _repo.GetAllAsync();
		}

		public async Task<CategoryDto> GetByIdAsync(int id)
		{
			return await _repo.GetByIdAsync(id);
		}

		public async Task CreateAsync(CategoryDto category)
		{
			await _repo.CreateAsync(category);
		}

		public async Task UpdateAsync(CategoryDto category)
		{
			await _repo.UpdateAsync(category);
		}

		public async Task DisableAsync(int id)
		{
			await _repo.SoftDeleteAsync(id);
		}
	}
}
