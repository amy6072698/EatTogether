using EatTogether.Models.DTOs;

namespace EatTogether.Models.Repositories
{
    public interface ITableRepository
    {
        Task<IEnumerable<TableDto>> GetAllAsync();
        Task<TableDto?> GetByIdAsync(int id);
        Task CreateAsync(TableDto dto);
        Task UpdateAsync(TableDto dto);
        Task UpdateStatusAsync(int id, int newStatus);
        Task DeleteAsync(int id);
        Task<bool> IsNameExistsAsync(string tableName, int excludeId = 0);
    }
}
