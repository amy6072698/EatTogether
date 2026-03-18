using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Models.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly EatTogetherDBContext _context;

        public ProductRepository(EatTogetherDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Dish)
                    .ThenInclude(d => d != null ? d.Category : null)
                .Include(p => p.SetMeal)
                .Select(p => new ProductDto
                {
                    Id                = p.Id,
                    ProductType       = p.ProductType,
                    DishId            = p.DishId,
                    DishName          = p.Dish != null ? p.Dish.DishName : null,
                    DishPrice         = p.Dish != null ? p.Dish.Price : null,
                    DishImageUrl      = p.Dish != null ? p.Dish.ImageUrl : null,
                    DishCategoryName  = p.Dish != null && p.Dish.Category != null
                                        ? p.Dish.Category.CategoryName : null,
                    SetMealId         = p.SetMealId,
                    SetMealName       = p.SetMeal != null ? p.SetMeal.SetMealName : null,
                    SetMealPrice      = p.SetMeal != null ? p.SetMeal.SetPrice : null,
                    SetMealImageUrl   = p.SetMeal != null ? p.SetMeal.ImageUrl : null
                })
                .ToListAsync();
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Dish)
                    .ThenInclude(d => d != null ? d.Category : null)
                .Include(p => p.SetMeal)
                .Where(p => p.Id == id)
                .Select(p => new ProductDto
                {
                    Id                = p.Id,
                    ProductType       = p.ProductType,
                    DishId            = p.DishId,
                    DishName          = p.Dish != null ? p.Dish.DishName : null,
                    DishPrice         = p.Dish != null ? p.Dish.Price : null,
                    DishImageUrl      = p.Dish != null ? p.Dish.ImageUrl : null,
                    DishCategoryName  = p.Dish != null && p.Dish.Category != null
                                        ? p.Dish.Category.CategoryName : null,
                    SetMealId         = p.SetMealId,
                    SetMealName       = p.SetMeal != null ? p.SetMeal.SetMealName : null,
                    SetMealPrice      = p.SetMeal != null ? p.SetMeal.SetPrice : null,
                    SetMealImageUrl   = p.SetMeal != null ? p.SetMeal.ImageUrl : null
                })
                .FirstOrDefaultAsync();
        }

        // 12
        public async Task<List<SetMealItemGroupDto>> GetSetMealItemsAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product?.SetMealId == null) return new List<SetMealItemGroupDto>();

            var items = await _context.SetMealItems
                .Include(s => s.Dish)
                .Where(s => s.SetMealId == product.SetMealId)
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();

            // 固定項目（IsOptional=0）放 GroupNo=0
            var result = items
                .GroupBy(s => s.IsOptional ? s.OptionGroupNo ?? 0 : -1)
                .OrderBy(g => g.Key)
                .Select(g => new SetMealItemGroupDto
                {
                    GroupNo = g.Key,
                    PickLimit = g.First().PickLimit ?? g.Count(),
                    IsOptional = g.First().IsOptional,
                    Options = g.Select(s => new SetMealItemOptionDto
                    {
                        DishId = s.DishId,
                        DishName = s.Dish.DishName,
                        Qty = s.Quantity
                    }).ToList()
                })
                .ToList();

            return result;
        }
    }
}
