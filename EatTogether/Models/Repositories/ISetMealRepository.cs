using EatTogether.Models.DTOs;

namespace EatTogether.Models.Repositories
{
	public interface ISetMealRepository
	{
		Task<IEnumerable<Setmealdto>> GetAllAsync();
		Task<Setmealdto?> GetByIdAsync(int id);
		Task CreateAsync(Setmealdto dto);
		Task UpdateAsync(Setmealdto dto);
		Task SoftDeleteAsync(int id);
		Task AddItemAsync(SetmealItemDto itemDto);
		Task RemoveItemAsync(int itemId);
	}
}
