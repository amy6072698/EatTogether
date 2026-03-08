using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;

namespace EatTogether.Models.Repositories
{
    public class DishRepository : IDishRepository
    {
        private readonly EatTogetherContext _context;

        public DishRepository(EatTogetherContext context) 
        {
            _context = context;
        }
        public void Create(DishDto dto)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DishDto> GetAll()
        {
            return _context.Dishes
                .Where(d => d.IsActive)
                .Select(d => new DishDto
                {
                    Id = d.Id,
                    DishName = d.DishName,
                    Description = d.Description,
                    Price = d.Price,
                    CategoryId = d.CategoryId,
                    CategoryName = d.Category != null ? d.Category.CategoryName : null,
                    ImageUrl = d.ImageUrl,
                    IsActive = d.IsActive,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt
                })
                .ToList();
        }

        public DishDto? GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void SoftDelete(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(DishDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
