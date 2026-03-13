using EatTogether.Models.EfModels;
using Microsoft.EntityFrameworkCore;
using System;

namespace EatTogether.Models.Repositories
{
    public interface IPreOrderRepository
    {
        // Create
        Task<List<PreOrder>> GetByStatusAsync(int doneOrCancel); // string → int
        Task AddAsync(PreOrder preOrder);
        Task<int> CountTodayAsync(DateTime date);

        // List
        Task UpdateDetailStatusAsync(int detailId, int status);

        // Payment
        Task CancelUnservedDetailsAsync(int preOrderId);
        Task<PreOrder?> GetByIdAsync(int id);




        Task<List<PreOrder>> GetAllAsync();
        
        Task UpdateStatusAsync(int id, int doneOrCancel);        // string → int
        Task DeleteAsync(int id);
        
    }

    public class PreOrderRepository : IPreOrderRepository
    {
        private readonly EatTogetherDBContext _context;
        public PreOrderRepository(EatTogetherDBContext db) => _context = db;

        // Create
        public async Task<List<PreOrder>> GetByStatusAsync(int doneOrCancel) =>
            await _context.PreOrders
                     .Include(p => p.PreOrderDetails)
                     .Include(p => p.Table)
                     .Where(p => p.DoneOrCancel == doneOrCancel)
                     .ToListAsync();
        public async Task AddAsync(PreOrder preOrder)
        {
            _context.PreOrders.Add(preOrder);
            await _context.SaveChangesAsync();
        }
        public async Task<int> CountTodayAsync(DateTime date)
        {
            return await _context.PreOrders
                .Where(p => p.OrderAt.Date == date.Date)
                .CountAsync();
        }

        // List
        public async Task UpdateDetailStatusAsync(int detailId, int status)
        {
            var detail = await _context.PreOrderDetails.FindAsync(detailId);
            if (detail is null) return;

            detail.DoneOrCancel = status;
            await _context.SaveChangesAsync();

            // 檢查該 PreOrder 的所有 Detail 是否全部完成或取消
            var allDetails = await _context.PreOrderDetails
                .Where(d => d.PreOrderId == detail.PreOrderId)
                .ToListAsync();

            bool allDone = allDetails.All(d => d.DoneOrCancel == 1 || d.DoneOrCancel == 2);

            if (allDone)
            {
                var preOrder = await _context.PreOrders.FindAsync(detail.PreOrderId);
                if (preOrder != null)
                {
                    preOrder.DoneOrCancel = PreOrderStatus.Done;  // 1
                    await _context.SaveChangesAsync();
                }
            }
        }

        // Payment
        public async Task CancelUnservedDetailsAsync(int preOrderId)
        {
            var details = await _context.PreOrderDetails
                .Where(d => d.PreOrderId == preOrderId && d.DoneOrCancel == 0)
                .ToListAsync();

            foreach (var d in details)
                d.DoneOrCancel = 2;  // 改成已取消

            await _context.SaveChangesAsync();
        }
        public async Task<PreOrder?> GetByIdAsync(int id) =>
            await _context.PreOrders
                     .Include(p => p.PreOrderDetails)
                     .Include(p => p.Table)
                     .FirstOrDefaultAsync(p => p.Id == id);





        public async Task<List<PreOrder>> GetAllAsync() =>
            await _context.PreOrders
                     .Include(p => p.PreOrderDetails)
                     .Include(p => p.Table)
                     .OrderByDescending(p => p.OrderAt)
                     .ToListAsync();

        

        

        

        public async Task UpdateStatusAsync(int id, int doneOrCancel) // 改這裡
        {
            var entity = await _context.PreOrders.FindAsync(id);
            if (entity is null) return;
            entity.DoneOrCancel = doneOrCancel; // 改這裡
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.PreOrders.FindAsync(id);
            if (entity is null) return;
            _context.PreOrders.Remove(entity);
            await _context.SaveChangesAsync();
        }

        
    }
}
