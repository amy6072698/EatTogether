using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;

namespace EatTogether.Models.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly EatTogetherDBContext _context;

        public CategoryRepository(EatTogetherDBContext context) 
        {
            _context = context;
        }
        public void Create(CategoryDto dto)
        {
            var category = new Category
            {
                CategoryName = dto.CategoryName,
                IsActive = true,
                ParentCategoryId = dto.ParentCategoryId,
                DisplayOrder = dto.DisplayOrder,
                ImageUrl = dto.ImageUrl,
                CreatedAt = DateTime.Now
            };
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public IEnumerable<CategoryDto> GetAll()
        {
            return _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName,
                    IsActive = c.IsActive,
                    ParentCategoryId = c.ParentCategoryId,
                    ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.CategoryName : null,
                    DisplayOrder = c.DisplayOrder,
                    ImageUrl = c.ImageUrl,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToList();
        }

        public CategoryDto? GetById(int id)
        {
            var category = _context.Categories
                .Where(c => c.Id == id && c.IsActive)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName,
                    IsActive = c.IsActive,
                    ParentCategoryId = c.ParentCategoryId,
                    ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.CategoryName : null,
                    DisplayOrder = c.DisplayOrder,
                    ImageUrl = c.ImageUrl,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .FirstOrDefault(); 

                    return category;
        }

        public void SoftDelete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null) return;

            category.IsActive = false;
            category.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
        }

        public void Update(CategoryDto dto)
        {
            var category = _context.Categories.Find(dto.Id);
            if (category == null) return;
            
                category.CategoryName = dto.CategoryName;
                category.ParentCategoryId = dto.ParentCategoryId;
                category.DisplayOrder = dto.DisplayOrder;
                category.ImageUrl = dto.ImageUrl;
                category.UpdatedAt = DateTime.Now;

                _context.SaveChanges();
        }
    }
}
