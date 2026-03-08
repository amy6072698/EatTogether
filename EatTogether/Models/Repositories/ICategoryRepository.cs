using EatTogether.Models.DTOs;

namespace EatTogether.Models.Repositories
{
    // ICategoryRepository 規格書，不實作，實作由 CategoryRepository 實作
    public interface ICategoryRepository
        {
            IEnumerable<CategoryDto> GetAll();
            CategoryDto? GetById(int id);
            void Create(CategoryDto dto);
            void Update(CategoryDto dto);
            void SoftDelete(int id);
        }
    
}
