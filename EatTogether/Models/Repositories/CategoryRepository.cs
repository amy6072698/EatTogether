using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Models.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly EatTogetherContext _context;

        public CategoryRepository(EatTogetherContext context) 
        {
            _context = context;
        }
       

		public async Task CreateAsync(CategoryDto dto)
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
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<CategoryDto>> GetAllAsync()
		{
			return await _context.Categories
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
				.ToListAsync();
		}


		public async Task<CategoryDto?> GetByIdAsync(int id)
		{
			return await _context.Categories
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
				.FirstOrDefaultAsync();
		}

		
		public async Task SoftDeleteAsync(int id)
		{
			var category = await _context.Categories.FindAsync(id);
			if (category == null) return;

			category.IsActive = false;
			category.UpdatedAt = DateTime.Now;

			await _context.SaveChangesAsync();
		}


		public async Task UpdateAsync(CategoryDto dto)
		{
			var category = await _context.Categories.FindAsync(dto.Id);
			if (category == null) return;

			category.CategoryName = dto.CategoryName;
			category.ParentCategoryId = dto.ParentCategoryId;
			category.DisplayOrder = dto.DisplayOrder;
			category.ImageUrl = dto.ImageUrl;
			category.UpdatedAt = DateTime.Now;

			await _context.SaveChangesAsync();
		}
	}
}
