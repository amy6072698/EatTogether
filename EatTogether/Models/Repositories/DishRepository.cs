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
            var dish = new Dish
            {
                CategoryId = dto.CategoryId,
                DishName = dto.DishName,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                IsTakeOut = dto.IsTakeOut,
                IsLimited = dto.IsLimited,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Dishes.Add(dish);
            _context.SaveChanges();
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
            return _context.Dishes
                .Where(d => d.Id == id && d.IsActive)
                .Select(d => new DishDto
                {
                    Id = d.Id,
                    CategoryId = d.CategoryId,
                    CategoryName = d.Category != null ? d.Category.CategoryName : null,
                    DishName = d.DishName,
                    Price = d.Price,
                    IsActive = d.IsActive,
                    Description = d.Description,
                    ImageUrl = d.ImageUrl,
                    IsTakeOut = d.IsTakeOut,
                    IsLimited = d.IsLimited,
                    StartDate = d.StartDate,
                    EndDate = d.EndDate,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt
                })
                .FirstOrDefault();
        }

        public void SoftDelete(int id)
        {
            var dish = _context.Dishes.Find(id);
            if (dish == null) return;

            dish.IsActive = false;
            dish.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();
        }

        public void Update(DishDto dto)
        {
            var dish = _context.Dishes.Find(dto.Id);
            if (dish == null)return;
            {
                dish.CategoryId = dto.CategoryId;
                dish.DishName = dto.DishName;
                dish.Description = dto.Description;
                dish.Price = dto.Price;
                dish.ImageUrl = dto.ImageUrl;
                dish.IsTakeOut = dto.IsTakeOut;
                dish.IsLimited = dto.IsLimited;
                dish.StartDate = dto.StartDate;
                dish.EndDate = dto.EndDate;
                dish.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();
            }
        }
    }
}
