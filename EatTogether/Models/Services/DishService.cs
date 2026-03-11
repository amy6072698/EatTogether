using EatTogether.Models.DTOs;
using EatTogether.Models.Repositories;

namespace EatTogether.Models.Services
{
	public class DishService
	{
		private readonly IDishRepository _repo;

		public DishService(IDishRepository repo)
		{
			_repo = repo;
		}
		
		public async Task<IEnumerable<DishDto>> GetAllAsync()
		{
			return await _repo.GetAllAsync();
		}

		public async Task<DishDto> GetByIdAsync(int id)
		{
			return await _repo.GetByIdAsync(id);
		}

		public async Task CreateAsync(DishDto dto)
		{
			await _repo.CreateAsync(dto);
		}

		public async Task UpdateAsync(DishDto dto)
		{
			await _repo.UpdateAsync(dto);
		}

		public async Task DisableAsync(int id)
		{
			await _repo.SoftDeleteAsync(id);
		}
	}
}
