using EatTogether.Models.EfModels;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Models.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
    }

    public class ProductRepository : IProductRepository
    {
        private readonly EatTogetherDBContext _context;
        public ProductRepository(EatTogetherDBContext db) => _context = db;

        public async Task<List<Product>> GetAllAsync() =>
            await _context.Products
                     .Include(p => p.Dish)  // 需要 Dish.DishName 和 Dish.Price
                     .ToListAsync();
    }
}
