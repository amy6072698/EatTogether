using EatTogether.Models.DTOs;

namespace EatTogether.Models.Repositories
{
    public interface IDishRepository
    {
        IEnumerable<DishDto> GetAll();
        DishDto? GetById(int id);
        void Create(DishDto dto);
        void Update(DishDto dto);
        void SoftDelete(int id);
    }
}
