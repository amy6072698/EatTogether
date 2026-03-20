using EatTogether.Models.DTOs;

namespace EatTogether.Models.Repositories
{
    // ICategoryRepository 規格書，不實作，實作由 CategoryRepository 實作
    public interface ICategoryRepository
        {
            Task<IEnumerable<CategoryDto>> GetAllAsync();
            Task<IEnumerable<CategoryDto>> GetAllActiveAsync();
            Task<CategoryDto?> GetByIdAsync(int id);
            Task CreateAsync(CategoryDto dto);
            Task UpdateAsync(CategoryDto dto);
            Task SoftDeleteAsync(int id);
            Task BatchSoftDeleteAsync(IEnumerable<int> ids);
            Task EnableAsync(int id);
            Task BatchEnableAsync(IEnumerable<int> ids);
            Task DeleteAsync(int id);
            Task BatchDeleteAsync(IEnumerable<int> ids);
            Task UpdateOrderAsync(IEnumerable<int> orderedIds);
        }
    
}
