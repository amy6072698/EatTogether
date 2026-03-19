using EatTogether.Models.DTOs;

namespace EatTogether.Models.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);

        // 12
        Task<List<SetMealItemGroupDto>> GetSetMealItemsAsync(int setMealId);
        Task<int?> GetPriceByNameAsync(string name);
    }
}
