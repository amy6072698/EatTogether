using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Models.Repositories
{
    public class DishRepository : IDishRepository
    {
        private readonly EatTogetherDBContext _context;

        public DishRepository(EatTogetherDBContext context) 
        {
            _context = context;
        }

		public async Task CreateAsync(DishDto dto)
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
				IsRecommended = dto.IsRecommended,
				IsPopular = dto.IsPopular,
				IsVegetarian = dto.IsVegetarian,
				SpicyLevel = dto.SpicyLevel,
				StartDate = dto.StartDate,
				EndDate = dto.EndDate,
				IsActive = true,
				CreatedAt = DateTime.Now
			};
			_context.Dishes.Add(dish);
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<DishDto>> GetAllAsync()
		{
			return await _context.Dishes
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
				   IsTakeOut = d.IsTakeOut,
				   IsLimited = d.IsLimited,
				   IsRecommended = d.IsRecommended,
				   IsPopular = d.IsPopular,
				   IsVegetarian = d.IsVegetarian,
				   SpicyLevel = d.SpicyLevel,
				   StartDate = d.StartDate,
				   EndDate = d.EndDate,
				   CreatedAt = d.CreatedAt,
				   UpdatedAt = d.UpdatedAt,
				   DisplayOrder = 0 // 資料庫無此欄位
			   })
               .OrderByDescending(d => d.CreatedAt)
			   .ToListAsync();
		}

		public async Task<IEnumerable<DishDto>> GetAllActiveAsync()
		{
			return await _context.Dishes
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
				   IsTakeOut = d.IsTakeOut,
				   IsLimited = d.IsLimited,
				   IsRecommended = d.IsRecommended,
				   IsPopular = d.IsPopular,
				   IsVegetarian = d.IsVegetarian,
				   SpicyLevel = d.SpicyLevel,
				   StartDate = d.StartDate,
				   EndDate = d.EndDate,
				   CreatedAt = d.CreatedAt,
				   UpdatedAt = d.UpdatedAt,
				   DisplayOrder = 0 // 資料庫無此欄位
			   })
               .OrderByDescending(d => d.CreatedAt)
			   .ToListAsync();
		}

		public async Task<DishDto?> GetByIdAsync(int id)
        {
            return await _context.Dishes
                .Where(d => d.Id == id)
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
                    UpdatedAt = d.UpdatedAt,
                    DisplayOrder = 0 // 資料庫無此欄位
                })
                .FirstOrDefaultAsync();
        }

		public async Task SoftDeleteAsync(int id)
		{
			var dish = await _context.Dishes.FindAsync(id);
			if (dish == null) return;

			dish.IsActive = false;
			dish.UpdatedAt = DateTime.UtcNow;

			await _context.SaveChangesAsync();
		}

		public async Task BatchSoftDeleteAsync(IEnumerable<int> ids)
		{
			var dishes = await _context.Dishes.Where(d => ids.Contains(d.Id)).ToListAsync();
			foreach (var d in dishes)
			{
				d.IsActive = false;
				d.UpdatedAt = DateTime.UtcNow;
			}
			await _context.SaveChangesAsync();
		}

		public async Task EnableAsync(int id)
		{
			var dish = await _context.Dishes.FindAsync(id);
			if (dish == null) return;

			dish.IsActive = true;
			dish.UpdatedAt = DateTime.UtcNow;

			await _context.SaveChangesAsync();
		}

		public async Task BatchEnableAsync(IEnumerable<int> ids)
		{
			var dishes = await _context.Dishes.Where(d => ids.Contains(d.Id)).ToListAsync();
			foreach (var d in dishes)
			{
				d.IsActive = true;
				d.UpdatedAt = DateTime.UtcNow;
			}
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var dish = await _context.Dishes.FindAsync(id);
			if (dish == null) return;

			_context.Dishes.Remove(dish);
			await _context.SaveChangesAsync();
		}

		public async Task BatchDeleteAsync(IEnumerable<int> ids)
		{
			var dishes = await _context.Dishes.Where(d => ids.Contains(d.Id)).ToListAsync();
			_context.Dishes.RemoveRange(dishes);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(DishDto dto)
		{
			var dish = await _context.Dishes.FindAsync(dto.Id);
			if (dish == null) return;
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
				dish.IsActive = dto.IsActive;
				dish.IsRecommended = dto.IsRecommended;
				dish.IsPopular = dto.IsPopular;
				dish.IsVegetarian = dto.IsVegetarian;
				dish.SpicyLevel = dto.SpicyLevel;
				dish.UpdatedAt = DateTime.UtcNow;

				await _context.SaveChangesAsync();
			}
		}

		public async Task UpdateOrderAsync(IEnumerable<int> orderedIds)
		{
			// 此功能暫不執行，因為資料庫 Dishes 表目前沒有 DisplayOrder 欄位
			await Task.CompletedTask;
		}
	}
}
