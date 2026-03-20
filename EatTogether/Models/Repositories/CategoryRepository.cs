using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Models.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly EatTogetherDBContext _context;

        public CategoryRepository(EatTogetherDBContext context) 
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
					UpdatedAt = c.UpdatedAt,
					DishCount = c.Dishes.Count()
				})
				.ToListAsync();
		}

		public async Task<IEnumerable<CategoryDto>> GetAllActiveAsync()
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
					UpdatedAt = c.UpdatedAt,
					DishCount = c.Dishes.Count()
				})
				.ToListAsync();
		}


		public async Task<CategoryDto?> GetByIdAsync(int id)
		{
			return await _context.Categories
				.Where(c => c.Id == id)
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

		public async Task BatchSoftDeleteAsync(IEnumerable<int> ids)
		{
			var categories = await _context.Categories.Where(c => ids.Contains(c.Id)).ToListAsync();
			foreach (var c in categories)
			{
				c.IsActive = false;
				c.UpdatedAt = DateTime.Now;
			}
			await _context.SaveChangesAsync();
		}

		public async Task EnableAsync(int id)
		{
			var category = await _context.Categories.FindAsync(id);
			if (category == null) return;

			category.IsActive = true;
			category.UpdatedAt = DateTime.Now;

			await _context.SaveChangesAsync();
		}

		public async Task BatchEnableAsync(IEnumerable<int> ids)
		{
			var categories = await _context.Categories.Where(c => ids.Contains(c.Id)).ToListAsync();
			foreach (var c in categories)
			{
				c.IsActive = true;
				c.UpdatedAt = DateTime.Now;
			}
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var category = await _context.Categories.FindAsync(id);
			if (category == null) return;

			_context.Categories.Remove(category);
			await _context.SaveChangesAsync();
		}

		public async Task BatchDeleteAsync(IEnumerable<int> ids)
		{
			var categories = await _context.Categories.Where(c => ids.Contains(c.Id)).ToListAsync();
			_context.Categories.RemoveRange(categories);
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

		public async Task UpdateOrderAsync(IEnumerable<int> orderedIds)
		{
			var categories = await _context.Categories.ToListAsync();
			int order = 1;
			foreach (var id in orderedIds)
			{
				var category = categories.FirstOrDefault(c => c.Id == id);
				if (category != null)
				{
					category.DisplayOrder = order++;
					category.UpdatedAt = DateTime.Now;
				}
			}
			await _context.SaveChangesAsync();
		}
	}
}
