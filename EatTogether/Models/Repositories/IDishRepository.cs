using EatTogether.Models.DTOs;

namespace EatTogether.Models.Repositories
{
    public interface IDishRepository
    {
        Task<IEnumerable<DishDto>> GetAllAsync();
        Task<DishDto?> GetByIdAsync(int id);
        Task CreateAsync(DishDto dto);
        Task UpdateAsync(DishDto dto);
        Task SoftDeleteAsync(int id);
    }
}
